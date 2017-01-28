$scriptPath = Split-Path $MyInvocation.MyCommand.Path
Set-Location $scriptPath

dotnet restore
dotnet build ./**/project.json -c release 

. ./UpdateNuspecVersion.ps1 -preRelease alpha -major 0 -minor 8

$children = gci ./ -recurse *.nuspec
foreach( $child in $children)
{
	Write-Output $child.FullName
	nuget pack -OutputDirectory D:\LocalNuget $child.FullName
}