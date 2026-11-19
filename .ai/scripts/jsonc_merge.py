#!/usr/bin/env python3
"""
jsonc_merge.py — comment-preserving edits for JSON with comments (JSONC).

VS Code settings and tasks files and Claude Code settings may carry // and
/* */ comments and trailing commas. This module edits such text without
re-serialising it. A tokenizer and parser locate the object or array to change;
each edit is then one splice (insert, replace or remove) into the original text.
Comments, key order, quoting and indentation outside the splice are untouched.

Invalid input raises JsoncError. Callers must then write nothing.

Paths are tuples of steps: a str selects an object member, an int selects an
array element. The empty tuple is the root value.

Stdlib only.
"""

from __future__ import annotations

import json
import re
from dataclasses import dataclass, field
from typing import Any, Optional, Union

__all__ = [
    "JsoncError",
    "load",
    "add_member",
    "set_member",
    "remove_member",
    "append_element",
    "remove_element",
]

PathStep = Union[str, int]

_ATOM = re.compile(r"-?(?:0|[1-9]\d*)(?:\.\d+)?(?:[eE][+-]?\d+)?|true|false|null")
_WHITESPACE = " \t\r\n﻿"


class JsoncError(ValueError):
    """The text is not valid JSONC, or a path does not match the document's shape."""


@dataclass(frozen=True)
class _Token:
    kind: str  # one of: { } [ ] : , string atom eof
    start: int
    end: int


@dataclass
class _Entry:
    key: Optional[str]  # property name; None for array elements
    key_start: int  # start of the key token, or of the value for array elements
    value: "_Node"
    comma_end: Optional[int]  # end of the comma that follows this entry, if any


@dataclass
class _Node:
    kind: str  # "object", "array" or "scalar"
    start: int
    end: int
    entries: list = field(default_factory=list)
    open_end: int = 0  # position just after "{" or "["
    close_start: int = 0  # position of "}" or "]"


# ─── Tokenizer and parser ────────────────────────────────────────────────────


def _line_col(text: str, pos: int) -> tuple[int, int]:
    line = text.count("\n", 0, pos) + 1
    col = pos - (text.rfind("\n", 0, pos) + 1) + 1
    return line, col


def _fail(text: str, pos: int, message: str) -> JsoncError:
    line, col = _line_col(text, pos)
    return JsoncError(f"line {line}, column {col}: {message}")


def _tokenize(text: str) -> list[_Token]:
    tokens: list[_Token] = []
    i, n = 0, len(text)
    while i < n:
        c = text[i]
        if c in _WHITESPACE:
            i += 1
        elif text.startswith("//", i):
            j = text.find("\n", i)
            i = n if j == -1 else j
        elif text.startswith("/*", i):
            j = text.find("*/", i + 2)
            if j == -1:
                raise _fail(text, i, "unterminated block comment")
            i = j + 2
        elif c in "{}[]:,":
            tokens.append(_Token(c, i, i + 1))
            i += 1
        elif c == '"':
            j = i + 1
            while True:
                if j >= n:
                    raise _fail(text, i, "unterminated string")
                ch = text[j]
                if ch == "\\":
                    j += 2
                    continue
                if ch == '"':
                    break
                if ch == "\n":
                    raise _fail(text, i, "unterminated string")
                j += 1
            end = j + 1
            try:
                json.loads(text[i:end])
            except ValueError as exc:
                raise _fail(text, i, f"invalid string literal ({exc})") from None
            tokens.append(_Token("string", i, end))
            i = end
        else:
            m = _ATOM.match(text, i)
            if m is None:
                raise _fail(text, i, f"unexpected character {c!r}")
            tokens.append(_Token("atom", i, m.end()))
            i = m.end()
    tokens.append(_Token("eof", n, n))
    return tokens


class _Parser:
    def __init__(self, text: str) -> None:
        self.text = text
        self.tokens = _tokenize(text)
        self.pos = 0

    def _peek(self) -> _Token:
        return self.tokens[self.pos]

    def _advance(self) -> _Token:
        tok = self.tokens[self.pos]
        self.pos += 1
        return tok

    def _unexpected(self, tok: _Token, expected: str) -> JsoncError:
        found = "end of input" if tok.kind == "eof" else repr(self.text[tok.start:tok.end])
        return _fail(self.text, tok.start, f"expected {expected}, found {found}")

    def parse_root(self) -> _Node:
        node = self._value()
        tok = self._peek()
        if tok.kind != "eof":
            raise _fail(self.text, tok.start, "unexpected content after the root value")
        return node

    def _value(self) -> _Node:
        tok = self._peek()
        if tok.kind == "{":
            return self._object()
        if tok.kind == "[":
            return self._array()
        if tok.kind in ("string", "atom"):
            self._advance()
            return _Node("scalar", tok.start, tok.end)
        raise self._unexpected(tok, "a value")

    def _separator(self, closer: str) -> Optional[int]:
        """Consume a comma and return its end, or return None when the closer follows."""
        tok = self._peek()
        if tok.kind == ",":
            self._advance()
            return tok.end
        if tok.kind == closer:
            return None
        raise self._unexpected(tok, f"',' or '{closer}'")

    def _object(self) -> _Node:
        open_tok = self._advance()
        node = _Node("object", open_tok.start, open_tok.end, [], open_tok.end)
        keys: set = set()
        while True:
            tok = self._peek()
            if tok.kind == "}":
                self._advance()
                node.close_start = tok.start
                node.end = tok.end
                return node
            if tok.kind != "string":
                raise self._unexpected(tok, "a property name or '}'")
            key = json.loads(self.text[tok.start:tok.end])
            if key in keys:
                raise _fail(self.text, tok.start, f"duplicate key {key!r}")
            keys.add(key)
            self._advance()
            colon = self._peek()
            if colon.kind != ":":
                raise self._unexpected(colon, "':'")
            self._advance()
            value = self._value()
            comma_end = self._separator("}")
            node.entries.append(_Entry(key, tok.start, value, comma_end))

    def _array(self) -> _Node:
        open_tok = self._advance()
        node = _Node("array", open_tok.start, open_tok.end, [], open_tok.end)
        while True:
            tok = self._peek()
            if tok.kind == "]":
                self._advance()
                node.close_start = tok.start
                node.end = tok.end
                return node
            value = self._value()
            comma_end = self._separator("]")
            node.entries.append(_Entry(None, value.start, value, comma_end))


def _parse(text: str) -> _Node:
    return _Parser(text).parse_root()


def _to_py(node: _Node, text: str) -> Any:
    if node.kind == "object":
        return {e.key: _to_py(e.value, text) for e in node.entries}
    if node.kind == "array":
        return [_to_py(e.value, text) for e in node.entries]
    return json.loads(text[node.start:node.end])


def load(text: str) -> Any:
    """Return the Python value of a JSONC document. Raises JsoncError on invalid input."""
    return _to_py(_parse(text), text)


# ─── Text helpers ────────────────────────────────────────────────────────────


def _newline(text: str) -> str:
    return "\r\n" if "\r\n" in text else "\n"


def _indent_unit(text: str) -> str:
    if re.search(r"^\t", text, re.MULTILINE):
        return "\t"
    widths = [len(m.group(1)) for m in re.finditer(r"^( +)\S", text, re.MULTILINE)]
    return " " * min(widths) if widths else "    "


def _line_indent(text: str, pos: int) -> str:
    start = text.rfind("\n", 0, pos) + 1
    return re.match(r"[ \t]*", text[start:]).group(0)


def _starts_line(text: str, pos: int) -> bool:
    start = text.rfind("\n", 0, pos) + 1
    return text[start:pos].strip(" \t") == ""


def _child_indent(text: str, node: _Node) -> str:
    """Indentation for a new member or element of ``node``, matching its last sibling."""
    if node.entries and _starts_line(text, node.entries[-1].key_start):
        return _line_indent(text, node.entries[-1].key_start)
    return _line_indent(text, node.start) + _indent_unit(text)


def _render(value: Any, text: str, indent: str) -> str:
    """Serialise a Python value; continuation lines are indented to ``indent``."""
    body = json.dumps(value, indent=_indent_unit(text), ensure_ascii=False)
    return body.replace("\n", _newline(text) + indent)


def _splice(text: str, start: int, end: int, replacement: str) -> str:
    return text[:start] + replacement + text[end:]


def _tidy(text: str, start: int, end: int) -> tuple[int, int]:
    """Widen a removal over its whole line when the line holds nothing else.

    When other content shares the line, the blanks after the removed span go with it so
    the next entry keeps its place.
    """
    e = end
    while e < len(text) and text[e] in " \t":
        e += 1
    line_start = text.rfind("\n", 0, start) + 1
    if text[line_start:start].strip(" \t"):
        return start, e
    if text.startswith("\r\n", e):
        return line_start, e + 2
    if text.startswith("\n", e):
        return line_start, e + 1
    if e >= len(text):
        return line_start, e
    return start, e


def _after_line_comment(text: str, pos: int) -> int:
    """Return the end of the line when only a same-line comment follows ``pos``, else ``pos``.

    Keeps a comment that trails a comma on the entry's own line with that entry.
    """
    eol = text.find("\n", pos)
    if eol == -1:
        return pos
    rest = text[pos:eol].strip()
    if rest.startswith("//") or (rest.startswith("/*") and rest.endswith("*/") and rest.count("*/") == 1):
        return eol - 1 if text[eol - 1:eol] == "\r" else eol
    return pos


def _insertion(text: str, node: _Node, member: str, first: bool) -> tuple[int, int, str]:
    """Return the splice that adds ``member`` to ``node`` (an object or array).

    A container whose last entry carries a trailing comma keeps that style: the new
    last entry gets a trailing comma too. A comment on the same line as the previous
    last entry stays with that entry.
    """
    nl = _newline(text)
    child = _child_indent(text, node)
    if node.entries:
        if first:
            pos = node.entries[0].key_start
            return pos, pos, member + "," + nl + child
        last = node.entries[-1]
        if last.comma_end is not None:
            pos = _after_line_comment(text, last.comma_end)
            return pos, pos, nl + child + member + ","
        end = _after_line_comment(text, last.value.end)
        return last.value.end, end, "," + text[last.value.end:end] + nl + child + member
    base = _line_indent(text, node.start)
    if text[node.open_end:node.close_start].strip() == "":
        return node.open_end, node.close_start, nl + child + member + nl + base
    # Only comments inside: keep them above the new member.
    close_line = text.rfind("\n", 0, node.close_start) + 1
    if close_line > node.open_end and text[close_line:node.close_start].strip(" \t") == "":
        return close_line, close_line, child + member + nl
    return node.close_start, node.close_start, nl + child + member + nl + base


def _removal(text: str, entries: list, index: int) -> tuple[int, int]:
    entry = entries[index]
    if entry.comma_end is not None:
        # A comment trailing the entry on its own line goes with it.
        return _tidy(text, entry.key_start, _after_line_comment(text, entry.comma_end))
    if index > 0:
        # Last entry without a comma: drop the previous entry's comma with it.
        return entries[index - 1].value.end, entry.value.end
    return entry.key_start, entry.value.end


# ─── Path navigation ─────────────────────────────────────────────────────────


def _describe(path: tuple) -> str:
    return "/".join(str(step) for step in path) or "<root>"


def _resolve(root: _Node, path: tuple) -> _Node:
    node = root
    for depth, step in enumerate(path):
        if isinstance(step, str):
            if node.kind != "object":
                raise JsoncError(f"{_describe(path[:depth])} is not an object")
            matches = [e for e in node.entries if e.key == step]
            if not matches:
                raise JsoncError(f"no key {step!r} at {_describe(path[:depth])}")
            node = matches[0].value
        else:
            if node.kind != "array" or not 0 <= step < len(node.entries):
                raise JsoncError(f"no element {step} at {_describe(path[:depth])}")
            node = node.entries[step].value
    return node


def _container(root: _Node, path: tuple, kind: str) -> _Node:
    node = _resolve(root, path)
    if node.kind != kind:
        noun = "an object" if kind == "object" else "an array"
        raise JsoncError(f"{_describe(path)} is not {noun}")
    return node


def _index_of(node: _Node, key: str, path: tuple) -> int:
    for index, entry in enumerate(node.entries):
        if entry.key == key:
            return index
    raise JsoncError(f"no key {key!r} at {_describe(path)}")


# ─── Public edits ────────────────────────────────────────────────────────────


def add_member(text: str, path: tuple, key: str, value: Any, *, first: bool = False) -> str:
    """Add ``key`` to the object at ``path``. The key must not exist yet.

    With ``first=True`` the member is placed before the existing members.
    """
    node = _container(_parse(text), path, "object")
    if any(e.key == key for e in node.entries):
        raise JsoncError(f"key {key!r} already exists at {_describe(path)}")
    member = json.dumps(key, ensure_ascii=False) + ": " + _render(value, text, _child_indent(text, node))
    return _splice(text, *_insertion(text, node, member, first))


def set_member(text: str, path: tuple, key: str, value: Any) -> str:
    """Replace the value of an existing member; the key and surrounding text are kept."""
    node = _container(_parse(text), path, "object")
    entry = node.entries[_index_of(node, key, path)]
    indent = _line_indent(text, entry.key_start)
    return _splice(text, entry.value.start, entry.value.end, _render(value, text, indent))


def remove_member(text: str, path: tuple, key: str) -> str:
    """Remove an existing member and its comma."""
    node = _container(_parse(text), path, "object")
    index = _index_of(node, key, path)
    return _splice(text, *_removal(text, node.entries, index), "")


def append_element(text: str, path: tuple, value: Any) -> str:
    """Append ``value`` to the array at ``path``."""
    node = _container(_parse(text), path, "array")
    rendered = _render(value, text, _child_indent(text, node))
    return _splice(text, *_insertion(text, node, rendered, False))


def remove_element(text: str, path: tuple, index: int) -> str:
    """Remove the array element at ``index``."""
    node = _container(_parse(text), path, "array")
    if not 0 <= index < len(node.entries):
        raise JsoncError(f"no element {index} at {_describe(path)}")
    return _splice(text, *_removal(text, node.entries, index), "")
