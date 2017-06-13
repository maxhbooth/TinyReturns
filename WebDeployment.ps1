GET-PSSession | Remove-PSSession
$ProjectName = 'TinyReturns'           #name of project
$baseDir = (resolve-path .)
$baseDir = "$baseDir\temp\tinyreturns"  #where the files in the team city agent are located/

$targetServerName = '***REMOVED***'   #machine name where we are connecting to

$sess = New-PSSession -ComputerName $targetServerName 

$remoteServerPath = '\\' + $targetServerName + '\c$\temp\' + $ProjectName + '\'

If (Test-Path "$remoteServerPath") {
	Write-Host "Deleting contents: $remoteServerPath"
	Remove-Item "$remoteServerPath\*" -recurse
}
Else {
	Write-Host "Creating folder: $remoteServerPath"
	New-Item -ItemType directory -Path $remoteServerPath
}

#copies from team city agent into temporary workign directory
Write-Host "copying from $baseDir\* to  $remoteServerPath"
Copy-Item "$baseDir\*" $remoteServerPath -recurse


Invoke-Command -Session $sess -ArgumentList ($ProjectName)  -Scriptblock {
$ProjectName = $($args[0])
sl "C:\temp\$ProjectName"

rm *.zip #removes artifact zip.  Should have files already unpacked by team city

$siteLocation = "C:\UtilityApps\$ProjectName"

If (Test-Path "$siteLocation") {
	Write-Host "Deleting contents: $siteLocation"
	Remove-Item "$siteLocation\*" -recurse
}
Else {
	Write-Host "Creating folder: $siteLocation"
	New-Item -ItemType directory -Path $siteLocation
}

#Move from temp to the site folder.
Copy-Item "C:\temp\$ProjectName\*" $siteLocation -recurse


#Set up Site using iis
Import-Module WebAdministration
$iisAppName = "$ProjectName"
$directoryPath = "C:\UtilityApps\$ProjectName\Release\_PublishedWebsites\Web" #path where the website is at

cd IIS:\Sites\

if (Test-Path $iisAppName -pathType container)
{
    return
}

$iisApp = New-Item $iisAppName -bindings @{protocol="http *";bindingInformation=":1704:"} -physicalPath $directoryPath


}

#Enter-PSSession $sess

