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

export PACKAGE_VERSION=$($PWD/scripts/version.sh)

sudo $PWD/scripts/uninstall.sh || true
sudo dpkg -i "$PWD/packages/SqlD.UI.$PACKAGE_VERSION.deb"
sudo systemctl enable sqld.ui
sudo systemctl start sqld.ui
sudo systemctl status sqld.ui
