#!/usr/bin/env bash

set -e

for dir in ./Components/*Test
do
    dotnet test ${dir}/*.csproj;
done

dotnet test IntegrationTest/*.csproj
