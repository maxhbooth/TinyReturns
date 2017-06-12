#Get-Job | Remove-Job | out-null

#$job = Start-Job -ScriptBlock { Enter-PSSession -ComputerName astof-retcal02d }
#$job | Wait-Job 
#Receive-Job $job

#$job = Invoke-Command -ScriptBlock { Enter-PSSession -ComputerName astof-retcal02d }

#$job = Invoke-Command -ScriptBlock { Enter-PSSession $sess} -ComputerName $hostname -AsJob
#$job | Wait-Job 
#sl c:\
#cd \
#hostnamejo
Get-PSSession | Remove-PSSession



$sess = New-PSSession -ComputerName astof-retcal02d 
$ProjectName = 'tinyreturns'
$baseDir = (resolve-path temp\$ProjectName)
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
hostname
sl "C:\temp\TinyReturns"
rm *.zip

$siteLocation = "C:\UtilityApps\TinyReturns"

If (Test-Path "$siteLocation") {
	Write-Host "Deletilsng contents: $siteLocation"
	Remove-Item "$siteLocation\*" -recurse
}
Else {
	Write-Host "Creating folder: $siteLocation"
	New-Item -ItemType directory -Path $siteLocation
}

Copy-Item "C:\temp\TinyReturns\*" $siteLocation -recurse

}

#Enter-PSSession $sess






#write-host "Local Path: " $args[0]
#write-host "Script: " $args[1]
#Join-Path -Path $args[0] -childpath $args[1]
#$path = Join-Path -Path $args[0] -childpath $args[1]
#&$path