[CmdletBinding()]
param(
    [ValidateSet('Stable', 'Workstream')]
    [string]$BuildType = 'Stable',

    [ValidateSet('Local', 'Cloud', 'Both')]
    [string]$PublishTarget = 'Both',

    [string]$LocalRepositoryPath
)

$ErrorActionPreference = 'Stop'
Set-StrictMode -Version Latest

$platformBuild = [System.IO.Path]::GetFullPath((Join-Path $PSScriptRoot '../platform/build/Invoke-NuvIoTBuild.ps1'))
if (-not (Test-Path -LiteralPath $platformBuild -PathType Leaf)) {
    throw "V1 platform build entrypoint not found at expected sibling path: $platformBuild"
}

$arguments = @{
    BuildType = $BuildType
    SourceRoot = $PSScriptRoot
    PublishTarget = $PublishTarget
}
if (-not [string]::IsNullOrWhiteSpace($LocalRepositoryPath)) {
    $arguments.LocalRepositoryPath = $LocalRepositoryPath
}

& $platformBuild @arguments
if ($LASTEXITCODE -and $LASTEXITCODE -ne 0) {
    exit $LASTEXITCODE
}
