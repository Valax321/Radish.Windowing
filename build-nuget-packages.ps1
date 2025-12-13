#!/usr/bin/env pwsh

& dotnet pack $PSScriptRoot/src/Radish.Windowing -c Release -o $PSScriptRoot/publish
& dotnet pack $PSScriptRoot/src/Radish.Windowing.SDL3 -c Release -o $PSScriptRoot/publish
