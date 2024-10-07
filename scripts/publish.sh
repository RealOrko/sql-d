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

dotnet restore $PWD/src/sql-d.ui/SqlD.UI.csproj
dotnet build $PWD/src/sql-d.ui/SqlD.UI.csproj
dotnet publish $PWD/src/sql-d.ui/SqlD.UI.csproj -o $PWD/published/
