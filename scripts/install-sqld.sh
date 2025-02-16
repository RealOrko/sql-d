#!/usr/bin/env bash

set -euo pipefail

helm upgrade --install -f $PWD/helm/values.yaml sql-d $PWD/helm #--debug
