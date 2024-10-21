#!/usr/bin/env bash

set -euo pipefail

DOTNETPATH=$(which dotnet)
if [ ! -f "$DOTNETPATH" ]; then
	echo "Please install Microsoft/dotnetcore from: https://www.microsoft.com/net/core"
	exit 1
fi

echo ''
echo '[SQL-D]:UNINSTALL/'
echo ''

sudo dpkg --purge sqld.ui || true
sudo rm -rf /usr/share/SqlD.UI/
