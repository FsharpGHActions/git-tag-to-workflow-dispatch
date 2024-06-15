#!/bin/bash

# Use this script to control the F# program.
#
# Requirements:
#
# - bash
# - dotnet
# - yq

set -euo pipefail

echo "[+] Running script..."

cd "${REPO_PATH}"

ls -la

# fatal: detected dubious ownership in repository at '/github/workspace'
git config --global --add safe.directory /github/workspace
git tag --sort -creatordate
GIT_TAGS=$(git tag --sort -creatordate)

echo "[+] GIT_TAGS: ${GIT_TAGS}"

# dotnet run --project . "${GIT_TAGS}"
/proj/Main "${GIT_TAGS}"

yq -y -i '. * load("/proj/workflow.new.yml")' "${REPO_PATH}/.github/workflows/${WORKFLOW_FILE_NAME}"

echo "[+] Script done!"