@echo off

set PvzReDir=

echo Cleaning buidings...
dotnet clean -c Release

echo Compiling...
dotnet build -c Release

@pause
