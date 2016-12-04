Param([string]$preRelease,[string]$major,[string]$minor)
$children = gci ./ -recurse *.nuspec 
foreach( $child in $children)
{
    $nuspecFile = gi $child.fullName
    [xml] $content = Get-Content $nuspecFile
    $end = Get-Date
    $start = Get-Date "12/1/2016"

    $today = Get-Date
    $today = $today.ToShortDateString()
    $today = Get-Date $today

    $versionPart = $content.package.metadata.version.Split('.')
    $revisionNumber = New-TimeSpan -Start $start -End $end
    $minutes = New-TimeSpan -Start $today -End $end
    $buildNumber = [math]::Round($minutes.TotalMinutes)

    if(!$major) {$major = $versionPart[0]}
    if(!$minor) {$minor = $versionPart[1]}

    if($preRelease){
        $content.package.metadata.version = $major +"." + $minor + "." + $revisionNumber.Days + "-$preRelease" + ("{0:00000}" -f $buildNumber)
        }
    else{
        $content.package.metadata.version = $major +"." + $minor + ".0"
    }

    Write-Output $child.fullName + "Setting nuget package version: " + $content.package.metadata.version
	$content.Save($child.FullName)
}
