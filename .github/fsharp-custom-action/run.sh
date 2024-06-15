#!/bin/bash

# Use this script to control the F# program.
#
# Requirements:
#
# - bash
# - git
# - dotnet
# - yq

set -euo pipefail

echo "[+] Running script..."

ls -la

GIT_TAGS=$(git tag --sort -creatordate)

echo "[+] GIT_TAGS: ${GIT_TAGS}"

# dotnet run --project . "${GIT_TAGS}"
/proj/main "${GIT_TAGS}"

yq -i '. * load("src/workflow.new.yml")' "${REPO_PATH}.github/workflows/${WORKFLOW_FILE_NAME}"

echo "[+] Script done!"