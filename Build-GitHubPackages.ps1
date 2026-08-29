param(
    [Parameter(Mandatory = $false)]
    [string]$Version = '5.0.0',

    [Parameter(Mandatory = $false)]
    [string]$OutputDirectory = './artifacts/packages'
)

$ErrorActionPreference = 'Stop'
Set-StrictMode -Version Latest

$repoRoot = $PSScriptRoot
Set-Location $repoRoot

$projectPath = Join-Path $repoRoot 'src/LagoVista.Core/LagoVista.Core.csproj'
$nuspecPath = Join-Path $repoRoot 'src/LagoVista.Core/Package.nuspec'
$outputPath = [System.IO.Path]::GetFullPath((Join-Path $repoRoot $OutputDirectory))

if (-not (Test-Path $projectPath)) { throw "Project not found: $projectPath" }
if (-not (Test-Path $nuspecPath)) { throw "NuSpec not found: $nuspecPath" }

if (Test-Path $outputPath) {
    Remove-Item -Recurse -Force $outputPath
}
New-Item -ItemType Directory -Force -Path $outputPath | Out-Null

Write-Host "Building LagoVista.Core package $Version"
Write-Host "Output: $outputPath"

[xml]$xml = Get-Content $nuspecPath -Raw
$metadata = $xml.package.metadata
if ($null -eq $metadata) {
    throw "NuSpec '$nuspecPath' does not contain package/metadata."
}

if ($metadata.id -ne 'LagoVista.Core') {
    throw "Expected package id 'LagoVista.Core' but found '$($metadata.id)'."
}

$internalDependencies = @($xml.SelectNodes('//dependency') | Where-Object {
    $_.id -and $_.id.StartsWith('LagoVista.', [System.StringComparison]::OrdinalIgnoreCase)
})
if ($internalDependencies.Count -gt 0) {
    $names = ($internalDependencies | ForEach-Object { $_.id }) -join ', '
    throw "LagoVista.Core canary must not have internal LagoVista dependencies. Found: $names"
}

# Stamp only the CI checkout. The committed NuSpec remains historical source metadata.
$metadata.version = $Version
$settings = New-Object System.Xml.XmlWriterSettings
$settings.Indent = $true
$settings.Encoding = New-Object System.Text.UTF8Encoding($false)
$settings.NewLineChars = "`r`n"
$settings.NewLineHandling = [System.Xml.NewLineHandling]::Replace

$writer = [System.Xml.XmlWriter]::Create($nuspecPath, $settings)
try {
    $xml.Save($writer)
}
finally {
    $writer.Dispose()
}

Write-Host 'Restoring LagoVista.Core...'
dotnet restore $projectPath
if ($LASTEXITCODE -ne 0) { throw "dotnet restore failed with exit code $LASTEXITCODE." }

Write-Host 'Building LagoVista.Core...'
dotnet build $projectPath --configuration Release --no-restore
if ($LASTEXITCODE -ne 0) { throw "dotnet build failed with exit code $LASTEXITCODE." }

Write-Host "Packing LagoVista.Core $Version..."
nuget pack $nuspecPath -Version $Version -OutputDirectory $outputPath -NonInteractive
if ($LASTEXITCODE -ne 0) {
    throw "nuget pack failed with exit code $LASTEXITCODE."
}

$packagePath = Join-Path $outputPath "LagoVista.Core.$Version.nupkg"
if (-not (Test-Path $packagePath)) {
    throw "Expected package was not produced: $packagePath"
}

Write-Host "Created $packagePath"
