#!/bin/bash

if [ -z $NUGET_API_KEY ]; then
  echo "NuGet API Key not set"
else
  NUPKG_PATH=$(ls src/Amp/bin/Release/*.nupkg)
  if [ -z $NUPKG_PATH ]; then
    echo "Missing NuPkg"
    exit 1
  elif [[ $NUPKG_PATH == *"local"* ]]; then
    echo "Invalid Local NuPkg"
    exit 2
  else
    echo "Uploading NuPkg"
    dotnet nuget push $NUPKG_PATH -k $NUGET_API_KEY -s https://nuget.org
  fi
fi
