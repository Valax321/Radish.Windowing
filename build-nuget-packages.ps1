#!/usr/bin/env pwsh

& dotnet pack $PSScriptRoot/src/Radish.Windowing -c Release --include-symbols -o $PSScriptRoot/publish
& dotnet pack $PSScriptRoot/src/Radish.Windowing.SDL3 -c Release --include-symbols -o $PSScriptRoot/publish
