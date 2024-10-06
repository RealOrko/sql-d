#!/usr/bin/env bash

set -euo pipefail

DOTNETPATH=$(which dotnet)
if [ ! -f "$DOTNETPATH" ]; then
	echo "Please install Microsoft/dotnetcore from: https://www.microsoft.com/net/core"
	exit 1
fi

echo ''
echo '[SQL-D]:PUBLISH/'
echo ''

export GITHUB_RUN_NUMBER='12'
export GITHUB_BRANCH_NAME=$(git rev-parse --abbrev-ref HEAD)

dotnet msbuild $PWD/src/sql-d/SqlD.csproj /t:Build /p:Configuration=Release /p:RevisionVersion=$GITHUB_RUN_NUMBER 
dotnet msbuild $PWD/src/sql-d/SqlD.csproj /t:CreateZip /p:Configuration=Release /p:RevisionVersion=$GITHUB_RUN_NUMBER
dotnet msbuild $PWD/src/sql-d/SqlD.csproj /t:CreateTarball /p:Configuration=Release /p:RevisionVersion=$GITHUB_RUN_NUMBER
dotnet msbuild $PWD/src/sql-d/SqlD.csproj /t:CreateRpm /p:Configuration=Release /p:RevisionVersion=$GITHUB_RUN_NUMBER
dotnet msbuild $PWD/src/sql-d/SqlD.csproj /t:CreateDeb /p:Configuration=Release /p:RevisionVersion=$GITHUB_RUN_NUMBER

dotnet msbuild $PWD/src/sql-d.ui/SqlD.UI.csproj /t:Build /p:Configuration=Release /p:RevisionVersion=$GITHUB_RUN_NUMBER 
dotnet msbuild $PWD/src/sql-d.ui/SqlD.UI.csproj /t:CreateZip /p:Configuration=Release /p:RevisionVersion=$GITHUB_RUN_NUMBER
dotnet msbuild $PWD/src/sql-d.ui/SqlD.UI.csproj /t:CreateTarball /p:Configuration=Release /p:RevisionVersion=$GITHUB_RUN_NUMBER
dotnet msbuild $PWD/src/sql-d.ui/SqlD.UI.csproj /t:CreateRpm /p:Configuration=Release /p:RevisionVersion=$GITHUB_RUN_NUMBER
dotnet msbuild $PWD/src/sql-d.ui/SqlD.UI.csproj /t:CreateDeb /p:Configuration=Release /p:RevisionVersion=$GITHUB_RUN_NUMBER

rm -rf $PWD/published/
mkdir -p $PWD/published/

find $PWD/src -name '*.nupkg' -exec cp "{}" $PWD/published/  \;
find $PWD/src -name '*.zip' -exec cp "{}" $PWD/published/  \;
find $PWD/src -name '*.tar.gz' -exec cp "{}" $PWD/published/  \;
find $PWD/src -name '*.rpm' -exec cp "{}" $PWD/published/  \;
find $PWD/src -name '*.deb' -exec cp "{}" $PWD/published/  \;
