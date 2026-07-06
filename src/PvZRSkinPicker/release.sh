#!/usr/bin/env bash
set -e

trap 'echo "Error occurred! Press Enter to exit..."; read' ERR

dotnet clean -c Release

dotnet build -c Release -v:diag

read -p "Build finished. Press Enter to exit..."
