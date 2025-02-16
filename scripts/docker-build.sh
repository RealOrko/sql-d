#!/usr/bin/env bash

set -euo pipefail

echo ''
echo '[SQL-D]:DOCKER/BUILD/SQLD'
echo ''

docker stop $(docker ps -a -q) || true
docker system prune -a --force || true
docker build -f $PWD/docker/Dockerfile -t sql-d $PWD/
