#!/usr/bin/env bash
set -e

dotnet clean -c Release

dotnet build -c Release \
  -p:ContinuousIntegrationBuild=true \
  -p:Deterministic=true \
  -v:diag

read -p "Build finished. Press Enter to exit..."
