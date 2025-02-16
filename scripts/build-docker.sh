#!/usr/bin/env bash

set -euo pipefail

echo ''
echo '[SQL-D]:BUILD/DOCKER'
echo ''

docker build -f $PWD/docker/Dockerfile -t sql-d $PWD/
