#!/usr/bin/env pwsh

$version = "1.0.0-rc1"

& dotnet pack $PSScriptRoot/src/Radish.Windowing -c Release -o $PSScriptRoot/publish
if($LASTEXITCODE -ne 0) { exit 1 }

& dotnet nuget push "publish/Radish.Windowing.*.nupkg" --source "$env:UPLOAD_ENDPOINT"
if($LASTEXITCODE -ne 0) { exit 1 }

& dotnet pack $PSScriptRoot/src/Radish.Windowing.SDL3 -c Release -o $PSScriptRoot/publish
if($LASTEXITCODE -ne 0) { exit 1 }

& dotnet nuget push "publish/Radish.Windowing.SDL3.*.nupkg" --source "$env:UPLOAD_ENDPOINT"
if($LASTEXITCODE -ne 0) { exit 1 }
