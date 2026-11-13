#!/usr/bin/env pwsh

$version = "1.0.0-rc1"

& dotnet pack $PSScriptRoot/src/Radish.Windowing -c Release -o $PSScriptRoot/publish
& dotnet nuget push "publish/Radish.Windowing.$version.nupkg" --source "$UPLOAD_ENDPOINT"

& dotnet pack $PSScriptRoot/src/Radish.Windowing.SDL3 -c Release -o $PSScriptRoot/publish
& dotnet nuget push "publish/Radish.Windowing.SDL3.$version.nupkg" --source "$UPLOAD_ENDPOINT"
