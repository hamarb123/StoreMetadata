#!/bin/bash
cd "$(dirname "$0")"
cd ..
cd ..
read -p "Enter the folder that contains hamarb123.github.io: " folder
dotnet run --project ./Automation/GithubPagesChangelogFilesGenerator/GithubPagesChangelogFilesGenerator.csproj --configuration Release -- "$folder" 2dcraft
dotnet run --project ./Automation/GithubPagesChangelogFilesGenerator/GithubPagesChangelogFilesGenerator.csproj --configuration Release -- "$folder" store
dotnet run --project ./Automation/GithubPagesChangelogFilesGenerator/GithubPagesChangelogFilesGenerator.csproj --configuration Release -- "$folder" utils
echo "Finished."
