#!/usr/bin/env bash

set -euo pipefail

DOTNETPATH=$(which dotnet)
if [ ! -f "$DOTNETPATH" ]; then
	echo "Please install Microsoft/dotnetcore from: https://www.microsoft.com/net/core"
	exit 1
fi

set +u

if [ -z "$GITHUB_RUN_NUMBER" ]; then 
	export GITHUB_RUN_NUMBER=0
fi

if [ -z "$GITHUB_REF" ]; then 
	export GITHUB_BRANCH_NAME=$(git rev-parse --abbrev-ref HEAD | sed 's/master//g' | sed 's/main//g')
else 
	export GITHUB_BRANCH_NAME=$(echo $GITHUB_REF | sed 's/refs\/heads\///g' | sed 's/master//g' | sed 's/main//g')
fi

set -u

major_version=$(cat $PWD/version.props | grep -oPm1 "(?<=<MajorVersion>)[^<]+")
minor_version=$(cat $PWD/version.props | grep -oPm1 "(?<=<MinorVersion>)[^<]+")
revision_version=$(echo "$GITHUB_RUN_NUMBER-$GITHUB_BRANCH_NAME" | sed 's/\-$//g')

echo "$major_version.$minor_version.$revision_version"
