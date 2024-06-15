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

echo "[+] GIT_TAGS: ${GIT_TAGS}"

# dotnet run --project . "${GIT_TAGS}"
/proj/Main "${GIT_TAGS}"

yq -y -i '. * load("/proj/workflow.new.yml")' ".github/workflows/${WORKFLOW_FILE_NAME}"

echo "[+] Script done!"