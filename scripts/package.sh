#!/usr/bin/env bash

set -euo pipefail

. $PWD/scripts/includes/lib-retry.sh

DOTNETPATH=$(which dotnet)
if [ ! -f "$DOTNETPATH" ]; then
	echo "Please install Microsoft/dotnetcore from: https://www.microsoft.com/net/core"
	exit 1
fi

echo ''
echo '[SQL-D]:PACKAGE/SQLD'
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

dotnet restore $PWD/src/sql-d.ui/SqlD.UI.csproj
dotnet msbuild $PWD/src/sql-d.ui/SqlD.UI.csproj /t:Build /p:Configuration=Release /p:RevisionVersion=$SQLD_VERSION 
dotnet msbuild $PWD/src/sql-d.ui/SqlD.UI.csproj /t:CreateZip /p:Configuration=Release /p:RevisionVersion=$SQLD_VERSION
dotnet msbuild $PWD/src/sql-d.ui/SqlD.UI.csproj /t:CreateTarball /p:Configuration=Release /p:RevisionVersion=$SQLD_VERSION

# Please see: https://github.com/quamotion/dotnet-packaging/issues/99
export LD_DEBUG=all
retry dotnet msbuild $PWD/src/sql-d.ui/SqlD.UI.csproj /t:CreateRpm /p:Configuration=Release /p:RevisionVersion=$SQLD_VERSION
retry dotnet msbuild $PWD/src/sql-d.ui/SqlD.UI.csproj /t:CreateDeb /p:Configuration=Release /p:RevisionVersion=$SQLD_VERSION  
unset LD_DEBUG

mkdir -p $PWD/packages/

find $PWD/src -name '*.nupkg' -exec cp "{}" $PWD/packages/  \;
find $PWD/src -name '*.zip' -exec cp "{}" $PWD/packages/  \;
find $PWD/src -name '*.tar.gz' -exec cp "{}" $PWD/packages/  \;
find $PWD/src -name '*.rpm' -exec cp "{}" $PWD/packages/  \;
find $PWD/src -name '*.deb' -exec cp "{}" $PWD/packages/  \;

ls -la $PWD/packages/
