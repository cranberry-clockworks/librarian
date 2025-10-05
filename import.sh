#!/bin/bash
cat i.json | dotnet run Dict.cs -- pack | dotnet run Dict.cs -- import -d "Norwegian" -n "Basic (and reversed card)"
