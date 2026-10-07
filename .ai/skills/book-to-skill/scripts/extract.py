#!/usr/bin/env python3
# -*- coding: utf-8 -*-
"""Extract plain text and metadata from a PDF or EPUB for the book-to-skill skill.

Usage:
    python3 .ai/skills/book-to-skill/scripts/extract.py <path-to-pdf-or-epub>

Writes, relative to the current working directory:
    .ai/tmp/book_skill_work/full_text.txt   full text with page/section markers
    .ai/tmp/book_skill_work/metadata.json   title, counts, chapter offsets

EPUB files are parsed with the standard library only (zipfile + html.parser).
PDF files are converted with the `pdftotext` binary (poppler/xpdf), which must
be on PATH. No network access is performed.
"""

import io
import json
import re
import shutil
import subprocess
import sys
import zipfile
from html.parser import HTMLParser
from pathlib import Path
from urllib.parse import unquote
from xml.etree import ElementTree

WORK_DIR = Path(".ai/tmp/book_skill_work")
TEXT_FILE = WORK_DIR / "full_text.txt"
META_FILE = WORK_DIR / "metadata.json"

PDF_MAGIC = b"%PDF"
ZIP_MAGIC = b"PK\x03\x04"

PDFTOTEXT_HINT = (
    "install poppler (or xpdf) and make sure `pdftotext` is on PATH - "
    "apt-get install poppler-utils (Debian/Ubuntu), brew install poppler (macOS), "
    "choco install poppler or winget install oschwartz10612.Poppler (Windows)"
)


class ExtractionError(Exception):
    """A user-facing extraction failure (bad input, missing dependency, no text)."""


# --------------------------------------------------------------------------- #
# Shared helpers
# --------------------------------------------------------------------------- #

def clean_text(text):
    """Normalise whitespace while preserving paragraph breaks."""
    text = text.replace("\r\n", "\n").replace("\r", "\n")
    text = re.sub(r"[ \t\f\v]+", " ", text)
    text = "\n".join(line.strip() for line in text.split("\n"))
    text = re.sub(r"\n{3,}", "\n\n", text)
    return text.strip()


def assemble_sections(sections):
    """Join sections with a blank line; return (text, [(start, end), ...]).

    Each span covers one section together with the separator that precedes it,
    so the spans tile the whole text and can be sliced directly.
    """
    parts = []
    spans = []
    offset = 0
    for section in sections:
        if parts:
            parts.append("\n\n")
            offset += 2
        start = offset
        parts.append(section)
        offset += len(section)
        spans.append((start, offset))
    return "".join(parts), spans


def detect_format(path):
    """Return 'pdf' or 'epub' for path, or raise ExtractionError."""
    if not path.exists():
        raise ExtractionError("file not found: %s" % path)
    if not path.is_file():
        raise ExtractionError("not a file: %s" % path)
    with path.open("rb") as handle:
        head = handle.read(8)
    suffix = path.suffix.lower()
    if suffix == ".pdf":
        if not head.startswith(PDF_MAGIC):
            raise ExtractionError("%s has a .pdf extension but no PDF header" % path)
        return "pdf"
    if suffix == ".epub":
        if not head.startswith(ZIP_MAGIC):
            raise ExtractionError("%s has an .epub extension but is not a zip archive" % path)
        return "epub"
    raise ExtractionError(
        "unsupported format: %s - book-to-skill accepts .pdf and .epub files only" % path
    )


def write_outputs(text, metadata):
    """Create the work directory and write the two output files."""
    WORK_DIR.mkdir(parents=True, exist_ok=True)
    with open(TEXT_FILE, "w", encoding="utf-8", newline="\n") as handle:
        handle.write(text)
    with open(META_FILE, "w", encoding="utf-8", newline="\n") as handle:
        handle.write(json.dumps(metadata, indent=2, ensure_ascii=False) + "\n")


# --------------------------------------------------------------------------- #
# EPUB extraction
# --------------------------------------------------------------------------- #

_HTML_SKIP_TAGS = {"script", "style", "head", "template", "svg", "noscript"}
_HTML_BLOCK_TAGS = {
    "address", "article", "aside", "blockquote", "br", "caption", "dd", "div",
    "dl", "dt", "figcaption", "figure", "footer", "form", "h1", "h2", "h3",
    "h4", "h5", "h6", "header", "hr", "li", "main", "nav", "ol", "p", "pre",
    "section", "table", "tbody", "td", "tfoot", "th", "thead", "tr", "ul",
}
_HTML_HEADING_TAGS = ("h1", "h2", "h3", "h4")
_ENCODING_RE = re.compile(rb"""(?:charset|encoding)=["']?([A-Za-z0-9_.\-]+)""")


class ChapterParser(HTMLParser):
    """Collect visible text, paragraph breaks and headings from one document."""

    def __init__(self):
        super().__init__(convert_charrefs=True)
        self._parts = []
        self._skip_depth = 0
        self._headings = []
        self._heading_tag = None
        self._heading_parts = []
        self._in_title = False
        self._title_parts = []

    def handle_starttag(self, tag, attrs):
        if tag == "title":
            self._in_title = True
            return
        if tag in _HTML_SKIP_TAGS:
            self._skip_depth += 1
            return
        if self._skip_depth:
            return
        if tag in _HTML_HEADING_TAGS and self._heading_tag is None:
            self._heading_tag = tag
            self._heading_parts = []
        if tag in _HTML_BLOCK_TAGS:
            self._parts.append("\n\n")

    def handle_endtag(self, tag):
        if tag == "title":
            self._in_title = False
            return
        if tag in _HTML_SKIP_TAGS:
            self._skip_depth = max(0, self._skip_depth - 1)
            return
        if self._skip_depth:
            return
        if tag == self._heading_tag:
            heading = re.sub(r"\s+", " ", "".join(self._heading_parts)).strip()
            if heading:
                self._headings.append(heading)
            self._heading_tag = None
        if tag in _HTML_BLOCK_TAGS:
            self._parts.append("\n\n")

    def handle_data(self, data):
        if self._in_title:
            self._title_parts.append(data)
            return
        if self._skip_depth:
            return
        if self._heading_tag is not None:
            self._heading_parts.append(data)
        self._parts.append(data)

    @property
    def text(self):
        return clean_text("".join(self._parts))

    @property
    def first_heading(self):
        return self._headings[0] if self._headings else ""

    @property
    def document_title(self):
        return re.sub(r"\s+", " ", "".join(self._title_parts)).strip()


def decode_html(data):
    """Decode XHTML bytes using the declared encoding, with sane fallbacks."""
    declared = _ENCODING_RE.search(data[:4096])
    candidates = []
    if declared:
        candidates.append(declared.group(1).decode("ascii", "ignore"))
    candidates.extend(["utf-8", "cp1252", "latin-1"])
    for encoding in candidates:
        try:
            return data.decode(encoding)
        except (LookupError, UnicodeDecodeError):
            continue
    return data.decode("utf-8", "replace")


def _local_name(tag):
    return tag.rsplit("}", 1)[-1]


def _find_opf_name(archive, names):
    """Locate the OPF package document via META-INF/container.xml (or by scan)."""
    container = "META-INF/container.xml"
    if container in names:
        try:
            root = ElementTree.fromstring(archive.read(container))
        except ElementTree.ParseError:
            root = None
        if root is not None:
            for element in root.iter():
                if _local_name(element.tag) == "rootfile":
                    full_path = element.get("full-path")
                    if full_path:
                        return full_path
    for name in sorted(names):
        if name.lower().endswith(".opf"):
            return name
    return None


def _read_opf(archive, opf_name):
    """Return (title, [document href, ...]) from the OPF package document."""
    try:
        root = ElementTree.fromstring(archive.read(opf_name))
    except ElementTree.ParseError as exc:
        raise ExtractionError("malformed EPUB package document %s: %s" % (opf_name, exc))
    except KeyError as exc:
        raise ExtractionError("malformed EPUB: package document missing from archive (%s)" % exc)

    title = ""
    manifest = {}
    spine = []
    for element in root.iter():
        name = _local_name(element.tag)
        if name == "title" and not title and element.text and element.text.strip():
            title = element.text.strip()
        elif name == "item":
            item_id = element.get("id")
            href = element.get("href")
            if item_id and href:
                manifest[item_id] = href
        elif name == "itemref":
            idref = element.get("idref")
            if idref:
                spine.append(idref)

    base_dir = opf_name.rsplit("/", 1)[0] if "/" in opf_name else ""
    docs = []
    for idref in spine:
        href = manifest.get(idref)
        if not href:
            continue
        href = unquote(href.split("#", 1)[0])
        if base_dir:
            href = base_dir + "/" + href
        docs.append(href)
    return title, docs


def _fallback_document_names(names):
    """Best-effort reading list for archives without a usable OPF."""
    readable = [
        name for name in names
        if name.lower().endswith((".xhtml", ".html", ".htm"))
        and not name.startswith("__MACOSX")
    ]
    return sorted(readable)


def extract_epub(path):
    """Return (title, text, spine_item_count, chapters) for an EPUB file."""
    try:
        archive = zipfile.ZipFile(path)
    except (zipfile.BadZipFile, OSError) as exc:
        raise ExtractionError("malformed EPUB archive %s: %s" % (path, exc))

    with archive:
        names = archive.namelist()
        title = path.stem
        docs = []
        opf_name = _find_opf_name(archive, names)
        if opf_name:
            opf_title, docs = _read_opf(archive, opf_name)
            if opf_title:
                title = opf_title
        if not docs:
            docs = _fallback_document_names(names)
        if not docs:
            raise ExtractionError("malformed EPUB archive: no XHTML documents found in %s" % path)

        sections = []
        chapter_titles = []
        for index, name in enumerate(docs, start=1):
            try:
                raw = archive.read(name)
            except KeyError:
                continue
            parser = ChapterParser()
            parser.feed(decode_html(raw))
            parser.close()
            body = parser.text
            if not body:
                continue
            chapter_title = parser.first_heading or parser.document_title
            chapter_title = re.sub(r"\s+", " ", chapter_title or ("Section %d" % index)).strip()
            sections.append("===== Section %d: %s =====\n\n%s" % (index, chapter_title, body))
            chapter_titles.append(chapter_title)

    if not sections:
        raise ExtractionError(
            "no text could be extracted from %s (all spine documents are empty)" % path
        )

    text, spans = assemble_sections(sections)
    chapters = [
        {"title": chapter_title, "start": start, "end": end}
        for chapter_title, (start, end) in zip(chapter_titles, spans)
    ]
    return title, text, len(docs), chapters


# --------------------------------------------------------------------------- #
# PDF extraction
# --------------------------------------------------------------------------- #

def extract_pdf(path):
    """Return (title, text, page_count, chapters) for a PDF, via pdftotext."""
    pdftotext = shutil.which("pdftotext")
    if not pdftotext:
        raise ExtractionError("`pdftotext` not found on PATH; %s" % PDFTOTEXT_HINT)

    result = subprocess.run(
        [pdftotext, "-enc", "UTF-8", str(path), "-"],
        stdout=subprocess.PIPE,
        stderr=subprocess.PIPE,
    )
    if result.returncode != 0:
        detail = result.stderr.decode("utf-8", "replace").strip()
        raise ExtractionError(
            "pdftotext failed on %s (exit code %d): %s"
            % (path, result.returncode, detail or "no error output")
        )

    raw = result.stdout.decode("utf-8", "replace").lstrip("﻿")
    raw = raw.replace("\r\n", "\n").replace("\r", "\n")
    pages = raw.split("\f")
    if pages and not pages[-1].strip():
        pages.pop()  # pdftotext terminates every page, including the last, with \f

    title = path.stem
    bodies = []
    sections = []
    for index, page in enumerate(pages, start=1):
        body = clean_text(page)
        bodies.append(body)
        if index == 1:
            lines = [line for line in body.split("\n") if line.strip()]
            if lines:
                title = lines[0][:120]
        marker = "===== Page %d =====" % index
        sections.append("%s\n\n%s" % (marker, body) if body else marker)

    if not any(bodies):
        raise ExtractionError(
            "no text could be extracted from %s (the PDF may be a scan; OCR is required)" % path
        )

    text, spans = assemble_sections(sections)
    chapters = [
        {"title": "Page %d" % index, "start": start, "end": end}
        for index, (start, end) in enumerate(spans, start=1)
    ]
    return title, text, len(pages), chapters


# --------------------------------------------------------------------------- #
# Entry point
# --------------------------------------------------------------------------- #

def main(argv):
    if len(argv) != 2:
        print("Usage: python3 extract.py <path-to-pdf-or-epub>", file=sys.stderr)
        return 2

    path = Path(argv[1])
    try:
        fmt = detect_format(path)
        if fmt == "pdf":
            title, text, pages, chapters = extract_pdf(path)
        else:
            title, text, pages, chapters = extract_epub(path)

        # Word counts and the emptiness check ignore the navigational markers.
        body_text = re.sub(r"(?m)^=====.*=====\s*$", "", text)
        words = len(body_text.split())
        if not words:
            raise ExtractionError("no text could be extracted from %s" % path)

        metadata = {
            "title": title,
            "pages": pages,
            "words": words,
            "estimated_tokens": int(words / 0.75 + 0.5),
            "size_bytes": path.stat().st_size,
            "source_path": str(path.resolve()),
            "format": fmt,
            "chapters": chapters,
        }
        write_outputs(text, metadata)
    except ExtractionError as exc:
        print("ERROR: %s" % exc, file=sys.stderr)
        return 1

    print("Extracted %s: %s" % (fmt.upper(), title))
    print(
        "  pages: %d | words: %d | estimated_tokens: %d | size_bytes: %d"
        % (pages, words, metadata["estimated_tokens"], metadata["size_bytes"])
    )
    print("  chapters: %d" % len(chapters))
    print("  text: %s (%d chars)" % (TEXT_FILE, len(text)))
    print("  metadata: %s" % META_FILE)
    return 0


if sys.platform == "win32":
    # Console/no-console streams may default to a legacy code page; force UTF-8.
    for _name in ("stdout", "stderr"):
        _stream = getattr(sys, _name)
        if hasattr(_stream, "buffer"):
            setattr(sys, _name, io.TextIOWrapper(_stream.buffer, encoding="utf-8", errors="replace"))


if __name__ == "__main__":
    sys.exit(main(sys.argv))
