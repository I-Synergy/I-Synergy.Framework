#!/usr/bin/env bash
# Probe NuGet flat container for the newest .NET 11 (rc/preview) version of each package
# that currently carries a 10.x / 11.x framework-aligned version in Directory.Packages.props.
set -uo pipefail

pkgs=$(grep -o 'Include="[^"]*"' Directory.Packages.props \
  | sed 's/Include="//; s/"//' \
  | grep -E '^(Microsoft\.AspNetCore|Microsoft\.EntityFrameworkCore|Microsoft\.Extensions|Npgsql\.EntityFrameworkCore|Microsoft\.Maui|System\.)' \
  | sort -u)

for p in $pkgs; do
  id=$(echo "$p" | tr '[:upper:]' '[:lower:]')
  json=$(curl -s -m 20 "https://api.nuget.org/v3-flatcontainer/$id/index.json")
  [ -z "$json" ] && { echo "$p|NORESPONSE"; continue; }
  # newest 11.0.0 rc/preview, or 11.0.0 stable if present; also report release-train (11.x.y) for extension packages
  best=$(echo "$json" | grep -o '"11\.0\.0-rc\.[^"]*"' | tr -d '"' | sort -V | tail -1)
  prev=$(echo "$json" | grep -o '"11\.0\.0-preview\.[^"]*"' | tr -d '"' | sort -V | tail -1)
  stable=$(echo "$json" | grep -o '"11\.0\.0"' | tr -d '"' | tail -1)
  train=$(echo "$json" | grep -oE '"11\.[1-9][0-9]*\.[0-9]+(-[^"]*)?"' | tr -d '"' | sort -V | tail -1)
  echo "$p|rc=$best|preview=$prev|stable=$stable|train=$train"
done
