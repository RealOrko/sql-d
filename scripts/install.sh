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

sudo dpkg --purge sqld.ui || true
sudo dpkg -i $PWD/packages/SqlD.UI.1.0.0-refactor.deb
sudo systemctl enabled sqld.ui
sudo systemctl start sqld.ui
