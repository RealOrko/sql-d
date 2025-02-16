#!/usr/bin/env bash

set -euo pipefail

echo ''
echo '[SQL-D]:RUN/SQLD/DOCKER'
echo ''

mkdir -p $PWD/.var

docker run -it \
  -p 8000:5000 \
  -p 8001:50100 \
  -p 8002:50095 \
  -v $PWD/.var:/tmp \
  sql-d \
  --network=host \
  --name=sql-d
