#!/usr/bin/env bash

set -euo pipefail

echo ''
echo '[SQL-D]:INSTALL/HELM/SQLD'
echo ''

helm upgrade --install -f $PWD/helm/values.yaml sql-d $PWD/helm #--debug
