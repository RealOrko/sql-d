#!/usr/bin/env bash

set -euo pipefail

DOTNETPATH=$(which dotnet)
if [ ! -f "$DOTNETPATH" ]; then
	echo "Please install Microsoft/dotnetcore from: https://www.microsoft.com/net/core"
	exit 1
fi

echo ''
echo '[SQL-D]:INSTALL/'
echo ''

set +u

if [ -z "$GITHUB_RUN_NUMBER" ]; then
	export GITHUB_RUN_NUMBER=0
fi 

set -u

echo ''
echo '[SQL-D]:BUILD/'
echo ''
echo "Current Run Number: $GITHUB_RUN_NUMBER"
echo ''

dotnet tool install --global dotnet-zip
dotnet tool install --global dotnet-tarball
dotnet tool install --global dotnet-rpm
dotnet tool install --global dotnet-deb

dotnet restore $PWD/tests/sql-d/SqlD.Tests.csproj
dotnet build $PWD/tests/sql-d/SqlD.Tests.csproj --no-restore -c Release
dotnet test $PWD/tests/sql-d/SqlD.Tests.csproj --no-build --no-restore -c Release --logger "trx;LogFileName=test-results.trx"

