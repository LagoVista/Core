param(
    [Parameter(Mandatory = $false)]
    [string]$Version = '3.0.3391.1320',

    [Parameter(Mandatory = $false)]
    [string]$OutputDirectory = './artifacts/packages'
)

$ErrorActionPreference = 'Stop'
Set-StrictMode -Version Latest

$repoRoot = $PSScriptRoot
Set-Location $repoRoot

$outputPath = [System.IO.Path]::GetFullPath((Join-Path $repoRoot $OutputDirectory))
if (Test-Path $outputPath) {
    Remove-Item -Recurse -Force $outputPath
}
New-Item -ItemType Directory -Force -Path $outputPath | Out-Null

Write-Host "Building compatibility package baseline $Version"
Write-Host "Output: $outputPath"

# The compatibility baseline deliberately mirrors the known-green synchronized
# dependency universe. NuSpecs are modified only in the ephemeral CI checkout;
# source package definitions remain untouched in git.
$nuspecs = @(Get-ChildItem -Path (Join-Path $repoRoot 'src') -Filter 'Package.nuspec' -File -Recurse)
if ($nuspecs.Count -eq 0) {
    throw 'No Package.nuspec files were found beneath src.'
}

foreach ($nuspec in $nuspecs) {
    [xml]$xml = Get-Content $nuspec.FullName -Raw
    $metadata = $xml.package.metadata
    if ($null -eq $metadata) {
        throw "NuSpec '$($nuspec.FullName)' does not contain package/metadata."
    }

    $metadata.version = $Version

    $dependencies = @($xml.SelectNodes('//dependency'))
    foreach ($dependency in $dependencies) {
        if ($dependency.id -and $dependency.id.StartsWith('LagoVista.', [System.StringComparison]::OrdinalIgnoreCase)) {
            $dependency.version = $Version
        }
    }

    $settings = New-Object System.Xml.XmlWriterSettings
    $settings.Indent = $true
    $settings.Encoding = New-Object System.Text.UTF8Encoding($false)
    $settings.NewLineChars = "`r`n"
    $settings.NewLineHandling = [System.Xml.NewLineHandling]::Replace

    $writer = [System.Xml.XmlWriter]::Create($nuspec.FullName, $settings)
    try {
        $xml.Save($writer)
    }
    finally {
        $writer.Dispose()
    }
}

Write-Host 'Restoring solution...'
dotnet restore ./Core.sln
if ($LASTEXITCODE -ne 0) { throw "dotnet restore failed with exit code $LASTEXITCODE." }

Write-Host 'Building solution...'
dotnet build ./Core.sln --configuration Release --no-restore
if ($LASTEXITCODE -ne 0) { throw "dotnet build failed with exit code $LASTEXITCODE." }

foreach ($nuspec in $nuspecs) {
    Write-Host "Packing $($nuspec.FullName)"
    nuget pack $nuspec.FullName -Version $Version -OutputDirectory $outputPath -NonInteractive
    if ($LASTEXITCODE -ne 0) {
        throw "nuget pack failed for '$($nuspec.FullName)' with exit code $LASTEXITCODE."
    }
}

$packages = @(Get-ChildItem -Path $outputPath -Filter '*.nupkg' -File | Where-Object { $_.Name -notlike '*.symbols.nupkg' })
if ($packages.Count -eq 0) {
    throw 'The pack completed without producing any .nupkg files.'
}

Write-Host "Created $($packages.Count) packages for baseline $Version."
$packages | Sort-Object Name | ForEach-Object { Write-Host "  $($_.Name)" }
