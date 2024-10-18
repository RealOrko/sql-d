#!/usr/bin/env bash

set -euo pipefail

DOTNETPATH=$(which dotnet)
if [ ! -f "$DOTNETPATH" ]; then
	echo "Please install Microsoft/dotnetcore from: https://www.microsoft.com/net/core"
	exit 1
fi

echo ''
echo '[SQL-D]:PACKAGE/'
echo ''

set +u

if [ -z "$GITHUB_RUN_NUMBER" ]; then 
	export GITHUB_RUN_NUMBER=0
fi

if [ -z "$GITHUB_REF" ]; then 
	export GITHUB_BRANCH_NAME=$(git rev-parse --abbrev-ref HEAD | sed 's/master//g' | sed 's/main//g')
else 
	export GITHUB_BRANCH_NAME=$(echo $GITHUB_REF | sed 's/refs\/heads\///g' | sed 's/master//g' | sed 's/main//g')
fi

set -u

export SQLD_VERSION=$(echo "$GITHUB_RUN_NUMBER-$GITHUB_BRANCH_NAME" | sed 's/\-$//g')

git clean -x -f -d

dotnet restore $PWD/src/sqld/SqlD.csproj
dotnet msbuild $PWD/src/sqld/SqlD.csproj /t:Build /p:Configuration=Release /p:RevisionVersion=$SQLD_VERSION 
dotnet msbuild $PWD/src/sqld/SqlD.csproj /t:CreateZip /p:Configuration=Release /p:RevisionVersion=$SQLD_VERSION
dotnet msbuild $PWD/src/sqld/SqlD.csproj /t:CreateTarball /p:Configuration=Release /p:RevisionVersion=$SQLD_VERSION

dotnet restore $PWD/src/sqld.ui/SqlD.UI.csproj
dotnet msbuild $PWD/src/sqld.ui/SqlD.UI.csproj /t:Build /p:Configuration=Release /p:RevisionVersion=$SQLD_VERSION 
dotnet msbuild $PWD/src/sqld.ui/SqlD.UI.csproj /t:CreateZip /p:Configuration=Release /p:RevisionVersion=$SQLD_VERSION
dotnet msbuild $PWD/src/sqld.ui/SqlD.UI.csproj /t:CreateTarball /p:Configuration=Release /p:RevisionVersion=$SQLD_VERSION
dotnet msbuild $PWD/src/sqld.ui/SqlD.UI.csproj /t:CreateRpm /p:Configuration=Release /p:RevisionVersion=$SQLD_VERSION
dotnet msbuild $PWD/src/sqld.ui/SqlD.UI.csproj /t:CreateDeb /p:Configuration=Release /p:RevisionVersion=$SQLD_VERSION

mkdir -p $PWD/packages/

find $PWD/src -name '*.nupkg' -exec cp "{}" $PWD/packages/  \;
find $PWD/src -name '*.zip' -exec cp "{}" $PWD/packages/  \;
find $PWD/src -name '*.tar.gz' -exec cp "{}" $PWD/packages/  \;
find $PWD/src -name '*.rpm' -exec cp "{}" $PWD/packages/  \;
find $PWD/src -name '*.deb' -exec cp "{}" $PWD/packages/  \;

ls -la $PWD/packages/
