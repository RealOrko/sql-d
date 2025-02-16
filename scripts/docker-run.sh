#!/usr/bin/env bash

set -euo pipefail

echo ''
echo '[SQL-D]:DOCKER/RUN/SQLD'
echo ''

mkdir -p $PWD/.var

docker run -d \
  -p 8000:5000 \
  -p 8001:50100 \
  -p 8002:50095 \
  -v $PWD/.var:/tmp \
  sql-d \
  --name=sql-d \
  --network=host \
  --name=sql-d

echo "You can access sql-d using http://localhost:8000"
echo "To kill please use ./scripts/docker-kill.sh"
echo "Also remember to remove ./var for state"
