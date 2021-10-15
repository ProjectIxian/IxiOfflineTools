#!/bin/sh -e
PROJECT="IXI Offline Tools"
SLN_FILE="IxiOfflineTools.sln"

echo Rebuilding $PROJECT...

echo Cleaning previous build
msbuild $SLN_FILE /p:Configuration=Release /target:Clean

echo Removing packages
rm -rf packages

echo Restoring packages
nuget restore $SLN_FILE

echo Building $PROJECT
msbuild $SLN_FILE /p:Configuration=Release

echo Done rebuilding $PROJECT