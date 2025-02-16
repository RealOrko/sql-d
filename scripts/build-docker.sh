#!/usr/bin/env bash

set -euo pipefail

docker build -f $PWD/docker/Dockerfile -t sql-d $PWD/
