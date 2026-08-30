param(
    [Parameter(Mandatory = $false)]
    [string]$Version = '5.0.0',

    [Parameter(Mandatory = $false)]
    [string]$OutputDirectory = './artifacts/packages',

    [Parameter(Mandatory = $false)]
    [string]$CatalogPath = './artifacts/package-catalog.json'
)

$ErrorActionPreference = 'Stop'
Set-StrictMode -Version Latest

$repoRoot = $PSScriptRoot
Set-Location $repoRoot

$solutionPath = Join-Path $repoRoot 'Core.sln'
$outputPath = [System.IO.Path]::GetFullPath((Join-Path $repoRoot $OutputDirectory))
$catalogFullPath = [System.IO.Path]::GetFullPath((Join-Path $repoRoot $CatalogPath))

if (-not (Test-Path $solutionPath)) { throw "Solution not found: $solutionPath" }

if (Test-Path $outputPath) {
    Remove-Item -Recurse -Force $outputPath
}
New-Item -ItemType Directory -Force -Path $outputPath | Out-Null
New-Item -ItemType Directory -Force -Path (Split-Path $catalogFullPath -Parent) | Out-Null

$packageRoots = @('src', 'tests')
$nuspecFiles = @(
    $packageRoots |
        ForEach-Object {
            $root = Join-Path $repoRoot $_
            if (Test-Path $root) {
                Get-ChildItem -Path $root -Filter 'Package.nuspec' -File -Recurse
            }
        } |
        Sort-Object FullName
)
if ($nuspecFiles.Count -eq 0) {
    throw 'No Package.nuspec files were found beneath src or tests.'
}

$packages = @()
$packageIds = @{}

foreach ($nuspec in $nuspecFiles) {
    [xml]$xml = Get-Content $nuspec.FullName -Raw
    $metadata = $xml.package.metadata
    if ($null -eq $metadata -or [string]::IsNullOrWhiteSpace([string]$metadata.id)) {
        throw "NuSpec '$($nuspec.FullName)' does not contain a package id."
    }

    $packageId = [string]$metadata.id
    if ($packageIds.ContainsKey($packageId)) {
        throw "Duplicate package id '$packageId' found in '$($nuspec.FullName)' and '$($packageIds[$packageId])'."
    }

    $projectFiles = @(Get-ChildItem -Path $nuspec.Directory.FullName -Filter '*.csproj' -File)
    if ($projectFiles.Count -ne 1) {
        throw "Expected exactly one project beside '$($nuspec.FullName)', found $($projectFiles.Count)."
    }

    $packageIds[$packageId] = $nuspec.FullName
    $packages += [pscustomobject]@{
        Id = $packageId
        NuSpecPath = $nuspec.FullName
        ProjectPath = $projectFiles[0].FullName
        Xml = $xml
    }
}

Write-Host "Discovered $($packages.Count) repository packages:"
$packages | Sort-Object Id | ForEach-Object { Write-Host "  $($_.Id)" }

$xmlSettings = New-Object System.Xml.XmlWriterSettings
$xmlSettings.Indent = $true
$xmlSettings.Encoding = New-Object System.Text.UTF8Encoding($false)
$xmlSettings.NewLineChars = "`r`n"
$xmlSettings.NewLineHandling = [System.Xml.NewLineHandling]::Replace

# Stamp only the build checkout. A dependency is repository-internal only when
# its package id is another Package.nuspec discovered in this repository.
foreach ($package in $packages) {
    $metadata = $package.Xml.package.metadata
    $metadata.version = $Version

    foreach ($dependency in @($package.Xml.SelectNodes('//dependency'))) {
        $dependencyId = [string]$dependency.id
        if (-not [string]::IsNullOrWhiteSpace($dependencyId) -and $packageIds.ContainsKey($dependencyId)) {
            $dependency.version = $Version
        }
    }

    # NuSpec file paths were historically authored for Windows. Normalize the
    # disposable checkout so packing works on the Linux build agent.
    foreach ($file in @($package.Xml.SelectNodes('//files/file'))) {
        if (-not [string]::IsNullOrWhiteSpace([string]$file.src)) {
            $source = ([string]$file.src).Replace('\', '/')
            $source = [regex]::Replace($source, '(^|/)release(/|$)', '$1Release$2', [System.Text.RegularExpressions.RegexOptions]::IgnoreCase)
            $file.src = $source
        }
        if (-not [string]::IsNullOrWhiteSpace([string]$file.target)) {
            $file.target = ([string]$file.target).Replace('\', '/')
        }
    }

    $writer = [System.Xml.XmlWriter]::Create($package.NuSpecPath, $xmlSettings)
    try {
        $package.Xml.Save($writer)
    }
    finally {
        $writer.Dispose()
    }
}

Write-Host "Restoring Core.sln for package set $Version..."
dotnet restore $solutionPath
if ($LASTEXITCODE -ne 0) { throw "dotnet restore failed with exit code $LASTEXITCODE." }

Write-Host 'Building Core.sln...'
dotnet build $solutionPath --configuration Release --no-restore
if ($LASTEXITCODE -ne 0) { throw "dotnet build failed with exit code $LASTEXITCODE." }

$catalogPackages = @()

foreach ($package in ($packages | Sort-Object Id)) {
    Write-Host "Packing $($package.Id) $Version..."
    $packageDirectory = Split-Path $package.NuSpecPath -Parent
    dotnet pack $package.ProjectPath --configuration Release --no-build --output $outputPath -p:NuspecFile=$($package.NuSpecPath) -p:NuspecBasePath=$packageDirectory -p:PackageVersion=$Version
    if ($LASTEXITCODE -ne 0) {
        throw "dotnet pack failed for '$($package.Id)' with exit code $LASTEXITCODE."
    }

    $packageFile = "$($package.Id).$Version.nupkg"
    $packagePath = Join-Path $outputPath $packageFile
    if (-not (Test-Path $packagePath)) {
        throw "Expected package was not produced: $packagePath"
    }

    $frameworks = @(
        $package.Xml.SelectNodes('//dependencies/group') |
            ForEach-Object { [string]$_.targetFramework } |
            Where-Object { -not [string]::IsNullOrWhiteSpace($_) } |
            Sort-Object -Unique
    )

    $dependencies = @()
    foreach ($dependency in @($package.Xml.SelectNodes('//dependency'))) {
        $dependencyId = [string]$dependency.id
        $dependencies += [ordered]@{
            id = $dependencyId
            version = [string]$dependency.version
            kind = if ($packageIds.ContainsKey($dependencyId)) { 'repository' } else { 'external' }
        }
    }

    $catalogPackages += [ordered]@{
        id = $package.Id
        version = $Version
        file = $packageFile
        targetFrameworks = $frameworks
        dependencies = $dependencies
    }
}

$sourceRepository = if ($env:GITHUB_REPOSITORY) { $env:GITHUB_REPOSITORY } else { 'LagoVista/Core' }
$sourceCommit = if ($env:GITHUB_SHA) { $env:GITHUB_SHA } else { (git rev-parse HEAD).Trim() }
$sourceRef = if ($env:GITHUB_REF_NAME) { $env:GITHUB_REF_NAME } else { (git branch --show-current).Trim() }

$catalog = [ordered]@{
    schemaVersion = 1
    generatedUtc = [DateTime]::UtcNow.ToString('o')
    source = [ordered]@{
        repository = $sourceRepository
        commit = $sourceCommit
        ref = $sourceRef
    }
    packages = $catalogPackages
}

$catalog | ConvertTo-Json -Depth 10 | Set-Content -Path $catalogFullPath -Encoding utf8

Write-Host "Created $($catalogPackages.Count) packages in $outputPath"
Write-Host "Created $catalogFullPath"
