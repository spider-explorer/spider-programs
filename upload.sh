#! /usr/bin/env bash
set -uvx
set -e
gh auth login --with-token < ~/settings/github-all-tokne.txt
gh.exe release upload 64bit $1 --clobber
