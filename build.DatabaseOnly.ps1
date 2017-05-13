$nugetExe = (get-childItem (".\src\.nuget\NuGet.exe")).FullName
&$nugetExe "install" ".\src\.nuget\packages.config" "-outputDirectory" ".\src\packages" "-source" "https://api.nuget.org/v3/index.json;https://www.nuget.org/api/v2" 

# '[p]sake' is the same as 'psake' but $Error is not polluted
remove-module [p]sake

# find psake's path
$psakeModule = (Get-ChildItem (".\psake.psm1")).FullName | Sort-Object $_ | select -last 1
 
Import-Module $psakeModule

# you can write statements in multiple lines using `
Invoke-psake -buildFile .\default.ps1 `
			 -taskList databaseonly `
			 -framework 4.0

Write-Host "Build exit code:" $LastExitCode

# Propagating the exit code so that builds actually fail when there is a problem
exit $LastExitCode