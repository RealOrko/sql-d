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

export PACKAGE_VERSION=$($PWD/scripts/version.sh)

dotnet restore $PWD/src/sql-d/SqlD.csproj
dotnet msbuild $PWD/src/sql-d/SqlD.csproj /t:Build /p:Configuration=Release /p:PackageVersion=$PACKAGE_VERSION 
dotnet msbuild $PWD/src/sql-d/SqlD.csproj /t:CreateZip /p:Configuration=Release /p:PackageVersion=$PACKAGE_VERSION
dotnet msbuild $PWD/src/sql-d/SqlD.csproj /t:CreateTarball /p:Configuration=Release /p:PackageVersion=$PACKAGE_VERSION

dotnet restore $PWD/src/sql-d.ui/SqlD.UI.csproj
dotnet msbuild $PWD/src/sql-d.ui/SqlD.UI.csproj /t:Build /p:Configuration=Release /p:PackageVersion=$PACKAGE_VERSION 
dotnet msbuild $PWD/src/sql-d.ui/SqlD.UI.csproj /t:CreateZip /p:Configuration=Release /p:PackageVersion=$PACKAGE_VERSION
dotnet msbuild $PWD/src/sql-d.ui/SqlD.UI.csproj /t:CreateTarball /p:Configuration=Release /p:PackageVersion=$PACKAGE_VERSION
dotnet msbuild $PWD/src/sql-d.ui/SqlD.UI.csproj /t:CreateRpm /p:Configuration=Release /p:PackageVersion=$PACKAGE_VERSION
dotnet msbuild $PWD/src/sql-d.ui/SqlD.UI.csproj /t:CreateDeb /p:Configuration=Release /p:PackageVersion=$PACKAGE_VERSION

rm -rf $PWD/packages/ || true
mkdir -p $PWD/packages/

find $PWD/src -name '*.nupkg' -exec cp "{}" $PWD/packages/  \;
find $PWD/src -name '*.zip' -exec cp "{}" $PWD/packages/  \;
find $PWD/src -name '*.tar.gz' -exec cp "{}" $PWD/packages/  \;
find $PWD/src -name '*.rpm' -exec cp "{}" $PWD/packages/  \;
find $PWD/src -name '*.deb' -exec cp "{}" $PWD/packages/  \;

ls -la $PWD/packages/
