#!/usr/bin/env bash

set -euo pipefail

echo ''
echo '[SQL-D]:BUILD/SQLD/DOCKER'
echo ''

docker build -f $PWD/docker/Dockerfile -t sql-d $PWD/
