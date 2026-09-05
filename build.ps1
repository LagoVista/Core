[CmdletBinding()]
param()

$ErrorActionPreference = 'Stop'
Set-StrictMode -Version Latest

$platformBuild = [System.IO.Path]::GetFullPath((Join-Path $PSScriptRoot '../platform/build/Invoke-NuvIoTBuild.ps1'))
if (-not (Test-Path -LiteralPath $platformBuild -PathType Leaf)) {
    throw "V1 platform build entrypoint not found at expected sibling path: $platformBuild"
}

& $platformBuild -SourceRoot $PSScriptRoot
if ($LASTEXITCODE -and $LASTEXITCODE -ne 0) {
    exit $LASTEXITCODE
}