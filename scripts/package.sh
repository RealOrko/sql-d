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

# For testing locally, please commit GITHUB_RUN_NUMBER, GITHUB_BRANCH_NAME commented 
# export GITHUB_RUN_NUMBER='12'
# export GITHUB_BRANCH_NAME=$(git rev-parse --abbrev-ref HEAD | sed 's/master//g' | sed 's/main//g')

# For pipeline runs in Github Actions, please always leave uncommented
export GITHUB_BRANCH_NAME=$(echo $GITHUB_REF | sed 's/refs\/heads\///g' | sed 's/master//g' | sed 's/main//g')
export SQLD_VERSION=$(echo "$GITHUB_RUN_NUMBER-$GITHUB_BRANCH_NAME" | sed 's/\-$//g')

git clean -x -f -d

dotnet restore $PWD/src/sql-d/SqlD.csproj
dotnet msbuild $PWD/src/sql-d/SqlD.csproj /t:Build /p:Configuration=Release /p:RevisionVersion=$SQLD_VERSION 
dotnet msbuild $PWD/src/sql-d/SqlD.csproj /t:CreateZip /p:Configuration=Release /p:RevisionVersion=$SQLD_VERSION
dotnet msbuild $PWD/src/sql-d/SqlD.csproj /t:CreateTarball /p:Configuration=Release /p:RevisionVersion=$SQLD_VERSION
dotnet msbuild $PWD/src/sql-d/SqlD.csproj /t:CreateRpm /p:Configuration=Release /p:RevisionVersion=$SQLD_VERSION
dotnet msbuild $PWD/src/sql-d/SqlD.csproj /t:CreateDeb /p:Configuration=Release /p:RevisionVersion=$SQLD_VERSION

dotnet restore $PWD/src/sql-d.ui/SqlD.UI.csproj
dotnet msbuild $PWD/src/sql-d.ui/SqlD.UI.csproj /t:Build /p:Configuration=Release /p:RevisionVersion=$SQLD_VERSION 
dotnet msbuild $PWD/src/sql-d.ui/SqlD.UI.csproj /t:CreateZip /p:Configuration=Release /p:RevisionVersion=$SQLD_VERSION
dotnet msbuild $PWD/src/sql-d.ui/SqlD.UI.csproj /t:CreateTarball /p:Configuration=Release /p:RevisionVersion=$SQLD_VERSION
dotnet msbuild $PWD/src/sql-d.ui/SqlD.UI.csproj /t:CreateRpm /p:Configuration=Release /p:RevisionVersion=$SQLD_VERSION
dotnet msbuild $PWD/src/sql-d.ui/SqlD.UI.csproj /t:CreateDeb /p:Configuration=Release /p:RevisionVersion=$SQLD_VERSION

mkdir -p $PWD/packages/

find $PWD/src -name '*.nupkg' -exec cp "{}" $PWD/packages/  \;
find $PWD/src -name '*.zip' -exec cp "{}" $PWD/packages/  \;
find $PWD/src -name '*.tar.gz' -exec cp "{}" $PWD/packages/  \;
find $PWD/src -name '*.rpm' -exec cp "{}" $PWD/packages/  \;
find $PWD/src -name '*.deb' -exec cp "{}" $PWD/packages/  \;

ls -la $PWD/packages/
