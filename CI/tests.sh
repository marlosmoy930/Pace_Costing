#!/usr/bin/env bash

exitCode=0

for testproj in $(find ${pwd} -type f -name "*.csproj" -name "*Test*")
do
    if ! dotnet test $testproj ;
    then
        exitCode=1
    fi
done

exit $exitCode
