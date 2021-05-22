#!/bin/bash
cd "$(dirname "$0")"
dotnet run --project ./Automation/Compressor/Compressor.csproj --configuration Release -- ./v2/hamarb123/2dcraft/changelogs
dotnet run --project ./Automation/Compressor/Compressor.csproj --configuration Release -- ./v2/hamarb123/2dcraft/version_manifests
dotnet run --project ./Automation/Compressor/Compressor.csproj --configuration Release -- ./v2/hamarb123/store/changelogs
dotnet run --project ./Automation/Compressor/Compressor.csproj --configuration Release -- ./v2/hamarb123/store/version_manifests
dotnet run --project ./Automation/Compressor/Compressor.csproj --configuration Release -- ./v2/hamarb123/utils/changelogs
dotnet run --project ./Automation/Compressor/Compressor.csproj --configuration Release -- ./v2/hamarb123/utils/version_manifests
echo "Finished."
