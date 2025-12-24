#!/usr/bin/env bash
set -e

dotnet build -c Release \
  -p:ContinuousIntegrationBuild=true \
  -p:Deterministic=true \
  -p:DebugType=none \
  -p:DebugSymbols=false

read -p "Build finished. Press Enter to exit..."
