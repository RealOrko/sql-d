#!/usr/bin/env bash

set -euo pipefail

echo ''
echo '[SQL-D]:DOCKER/KILL/SQLD'
echo ''

mkdir -p $PWD/.var

docker stop $(docker ps -q --filter ancestor=sql-d )

echo "You should probably run $'docker system prune -a --force || true'"
