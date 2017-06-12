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



Invoke-Command -Session $sess -Scriptblock {$test = "team city works"





}

#Enter-PSSession $sess
