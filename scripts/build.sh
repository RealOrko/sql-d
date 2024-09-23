#!/usr/bin/env bash

set -euo pipefail

DOTNETPATH=$(which dotnet)
if [ ! -f "$DOTNETPATH" ]; then
	echo "Please install Microsoft/dotnetcore from: https://www.microsoft.com/net/core"
	exit 1
fi

echo ''
echo '[SQL-D]:BUILD/'
echo ''

rm -rf $PWD/tests/sql-d/bin
rm -rf $PWD/tests/sql-d/obj

dotnet restore $PWD/tests/sql-d/SqlD.Tests.csproj
dotnet build $PWD/tests/sql-d/SqlD.Tests.csproj
dotnet test $PWD/tests/sql-d/SqlD.Tests.csproj
