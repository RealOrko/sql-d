#!/usr/bin/env bash

set -euo pipefail

echo ''
echo '[SQL-D]:INSTALL/DEBIAN/SYSTEMD'
echo ''

sudo dpkg --purge sqld.ui || true
sudo dpkg -i $PWD/packages/SqlD.UI.1.1.0-helm.deb
sudo systemctl enable sqld.ui
sudo systemctl start sqld.ui
sudo systemctl status sqld.ui
