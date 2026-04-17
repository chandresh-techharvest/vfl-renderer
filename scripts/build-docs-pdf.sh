#!/bin/bash

# Generate PDF documentation using Pandoc
pandoc --toc --number-sections docs/combined/VFL-Renderer-Docs-1.0.0.md -o docs/VFL-Renderer-Docs-1.0.0.pdf