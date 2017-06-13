Get-PSSession | Remove-PSSession


$sess = New-PSSession -ComputerName astof-retcal02d 
$ProjectName = 'tinyreturns'
$baseDir = (resolve-path .)
$baseDir = $baseDir\temp\tinyreturns
$targetServerName = 'astof-retcal02d'

$remoteServerPath = '\\' + $targetServerName + '\c$\temp\' + $ProjectName + '\'

If (Test-Path "$remoteServerPath") {
	Write-Host "Deleting contents: $remoteServerPath"
	Remove-Item "$remoteServerPath\*" -recurse
}
Else {
	Write-Host "Creating folder: $remoteServerPath"
	New-Item -ItemType directory -Path $remoteServerPath
}

Copy-Item "$baseDir\*" $remoteServerPath -recurse


Invoke-Command -Session $sess -ArgumentList ($ProjectName)  -Scriptblock {
sl "C:\temp\TinyReturns"
rm *.zip

$siteLocation = "C:\UtilityApps\TinyReturns"

If (Test-Path "$siteLocation") {
	Write-Host "Deleting contents: $siteLocation"
	Remove-Item "$siteLocation\*" -recurse
}
Else {
	Write-Host "Creating folder: $siteLocation"
	New-Item -ItemType directory -Path $siteLocation
}

Copy-Item "C:\temp\TinyReturns\*" $siteLocation -recurse


#Set up Site using iis
Import-Module WebAdministration
$iisAppName = "TinyReturns"
$directoryPath = "C:\UtilityApps\TinyReturns"

cd IIS:\Sites\

if (Test-Path $iisAppName -pathType container)
{
    return
}

$iisApp = New-Item $iisAppName -bindings @{protocol="http *";bindingInformation=":1704:"} -physicalPath $directoryPath


}

#Enter-PSSession $sess






#write-host "Local Path: " $args[0]
#write-host "Script: " $args[1]
#Join-Path -Path $args[0] -childpath $args[1]
#$path = Join-Path -Path $args[0] -childpath $args[1]
#&$path