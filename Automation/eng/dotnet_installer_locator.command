#!/bin/bash
cd "$(dirname "$0")"
cd ..
cd ..
dotnet run --project ./Automation/DotnetInstallerLocator/DotnetInstallerLocator.csproj --configuration Release