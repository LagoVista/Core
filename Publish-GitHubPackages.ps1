param(
    [Parameter(Mandatory = $true)]
    [string]$Version
)

$ErrorActionPreference = 'Stop'
Set-StrictMode -Version Latest
if ($null -ne $PSStyle) { $PSStyle.OutputRendering = 'PlainText' }

function Write-PackageStatus {
    param(
        [Parameter(Mandatory = $true)][string]$PackageId,
        [Parameter(Mandatory = $true)][string]$PackageVersion,
        [Parameter(Mandatory = $true)][string]$State,
        [Parameter(Mandatory = $false)][string]$Message
    )

    $payload = [ordered]@{
        type = 'package'
        state = $State
        packageId = $PackageId
        version = $PackageVersion
        message = $Message
    }
    Write-Output ('BUILD_STATUS:' + ($payload | ConvertTo-Json -Compress))
}

$repoRoot = $PSScriptRoot
Set-Location $repoRoot

if ([string]::IsNullOrWhiteSpace($env:NUGET_GITHUB_USERNAME) -or
    [string]::IsNullOrWhiteSpace($env:NUGET_GITHUB_TOKEN)) {
    throw 'NUGET_GITHUB_USERNAME and NUGET_GITHUB_TOKEN are required to publish and verify packages.'
}

$catalogPath = Join-Path $repoRoot 'artifacts/package-catalog.json'
$packagesPath = Join-Path $repoRoot 'artifacts/packages'
$packageSource = 'https://nuget.pkg.github.com/nuviot/index.json'

& (Join-Path $repoRoot 'Build-GitHubPackages.ps1') -Version $Version
if ($LASTEXITCODE -ne 0) { throw "Build-GitHubPackages.ps1 failed with exit code $LASTEXITCODE." }

if (-not (Test-Path $catalogPath)) { throw "Package catalog not found: $catalogPath" }
$catalog = Get-Content $catalogPath -Raw | ConvertFrom-Json
if ($null -eq $catalog.packages -or @($catalog.packages).Count -eq 0) { throw 'Package catalog contains no packages.' }

foreach ($package in @($catalog.packages)) {
    $packagePath = Join-Path $packagesPath $package.file
    if (-not (Test-Path $packagePath)) {
        Write-PackageStatus -PackageId $package.id -PackageVersion $package.version -State 'failed' -Message "Package file not found: $packagePath"
        throw "Package file not found: $packagePath"
    }

    Write-PackageStatus -PackageId $package.id -PackageVersion $package.version -State 'publishing' -Message 'Uploading package to GitHub Packages'
    Write-Host "Publishing $($package.id) $($package.version)..."
    dotnet nuget push $packagePath --source $packageSource --api-key $env:NUGET_GITHUB_TOKEN --skip-duplicate
    if ($LASTEXITCODE -ne 0) {
        Write-PackageStatus -PackageId $package.id -PackageVersion $package.version -State 'failed' -Message "dotnet nuget push exited with code $LASTEXITCODE"
        throw "dotnet nuget push failed for '$($package.id)' with exit code $LASTEXITCODE."
    }
    Write-PackageStatus -PackageId $package.id -PackageVersion $package.version -State 'published' -Message 'Package upload completed or version already existed'
}

$basicCredential = [Convert]::ToBase64String(
    [Text.Encoding]::UTF8.GetBytes("$($env:NUGET_GITHUB_USERNAME):$($env:NUGET_GITHUB_TOKEN)"))
$headers = @{
    Authorization = "Basic $basicCredential"
    'User-Agent' = 'softwarelogistics-build-server/0.1'
}

$serviceIndexUrl = $packageSource
$serviceIndex = Invoke-RestMethod -Uri $serviceIndexUrl -Headers $headers -Method Get
$packageBaseResource = @($serviceIndex.resources) |
    Where-Object { [string]$_.'@type' -like 'PackageBaseAddress/*' } |
    Select-Object -First 1

if ($null -eq $packageBaseResource -or [string]::IsNullOrWhiteSpace([string]$packageBaseResource.'@id')) {
    throw "GitHub Packages service index does not expose PackageBaseAddress: $serviceIndexUrl"
}

$packageBaseUrl = [string]$packageBaseResource.'@id'
if (-not $packageBaseUrl.EndsWith('/')) { $packageBaseUrl += '/' }

$verifyRoot = Join-Path $repoRoot 'artifacts/verify-downloads'
if (Test-Path $verifyRoot) { Remove-Item -Recurse -Force $verifyRoot }
New-Item -ItemType Directory -Force -Path $verifyRoot | Out-Null

foreach ($package in @($catalog.packages)) {
    $packageId = ([string]$package.id).ToLowerInvariant()
    $packageVersion = ([string]$package.version).ToLowerInvariant()
    $packageUri = "$packageBaseUrl$packageId/$packageVersion/$packageId.$packageVersion.nupkg"
    $downloadPath = Join-Path $verifyRoot $package.file
    $verified = $false
    $lastError = $null

    for ($attempt = 1; $attempt -le 8; $attempt++) {
        Write-PackageStatus -PackageId $package.id -PackageVersion $package.version -State 'verifying' -Message "Downloading package from GitHub Packages (attempt $attempt/8)"
        Write-Host "Verifying $($package.id) $($package.version) from GitHub Packages (attempt $attempt/8)..."
        try {
            if (Test-Path $downloadPath) { Remove-Item -Force $downloadPath }
            Invoke-WebRequest -Uri $packageUri -Headers $headers -Method Get -MaximumRedirection 5 -OutFile $downloadPath
            if ((Test-Path $downloadPath) -and (Get-Item $downloadPath).Length -gt 0) {
                $verified = $true
                break
            }
            $lastError = 'Downloaded package was missing or empty.'
        }
        catch {
            $lastError = $_.Exception.Message
        }

        if ($attempt -lt 8) {
            Write-Host 'Package is not remotely available yet; waiting 5 seconds for feed propagation.'
            Start-Sleep -Seconds 5
        }
    }

    if (-not $verified) {
        Write-PackageStatus -PackageId $package.id -PackageVersion $package.version -State 'failed' -Message "Package was not downloadable after 8 attempts: $lastError"
        Write-Host "RELEASE_ERROR: $($package.id) $($package.version) was not downloadable from GitHub Packages after 8 attempts: $lastError"
        exit 1
    }

    Write-PackageStatus -PackageId $package.id -PackageVersion $package.version -State 'verified' -Message 'Downloaded and verified from GitHub Packages'
    Write-Host "Verified $($package.id) $($package.version) from GitHub Packages."
}

Write-Host "Published and verified $(@($catalog.packages).Count) packages at version $Version."
