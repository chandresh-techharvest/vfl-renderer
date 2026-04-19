#!/bin/bash

# Generate PDF documentation using Pandoc
# Requires: pandoc, texlive-latex-base, texlive-fonts-recommended, texlive-latex-extra
#
# Install on Ubuntu/Debian:
#   sudo apt-get install -y pandoc texlive-latex-base texlive-fonts-recommended texlive-latex-extra
#
# Install on macOS (Homebrew):
#   brew install pandoc mactex
#
# Usage:
#   ./scripts/build-docs-pdf.sh

set -e

SCRIPT_DIR="$(cd "$(dirname "$0")" && pwd)"
REPO_ROOT="$(dirname "$SCRIPT_DIR")"

pandoc --toc --number-sections \
  "${REPO_ROOT}/docs/combined/VFL-Renderer-Docs-1.0.0.md" \
  -o "${REPO_ROOT}/docs/VFL-Renderer-Docs-1.0.0.pdf" \
  --pdf-engine=pdflatex \
  -V geometry:margin=1in \
  -V fontsize=11pt \
  -H <(echo '\usepackage[T1]{fontenc}\usepackage{pmboxdraw}')

echo "PDF generated: docs/VFL-Renderer-Docs-1.0.0.pdf"