#!/bin/bash

VALUE=$1

# Clean the existing packages
find ./src -name *.nupkg | xargs -L1 rm

if [ -z $VERSION_SUFFIX ]; then
  VERSION=$(grep -Po ./src/Amp/Amp.csproj -e '(?<=<VersionPrefix>)[^<]+')
  echo "Using Version $VERSION"
  dotnet pack --configuration=Release /property:Version=$VERSION
else
  echo "Using Version Suffix $VALUE"
  dotnet pack --configuration=Release --version-suffix="$VALUE"
fi
