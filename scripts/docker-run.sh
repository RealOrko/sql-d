#!/usr/bin/env bash

set -euo pipefail

DOTNETPATH=$(which dotnet)
if [ ! -f "$DOTNETPATH" ]; then
	echo "Please install Microsoft/dotnetcore from: https://www.microsoft.com/net/core"
	exit 1
fi

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

docker run -it -p:50110:50110 sql-d:latest 
