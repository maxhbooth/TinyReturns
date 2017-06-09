param(
	[String]$projectVersion=$(throw "-projectVersion is required.")
)

cls

# Sample Call...
#	.\TeamCity.masterbranch.ps1 -projectVersion 1.0.0.1


$nugetExe = (get-childItem (".\src\.NuGet\NuGet.exe")).FullName
&$nugetExe "restore" ".\src\build\packages.config" "-outputDirectory" ".\src\packages"

# '[p]sake' is the same as 'psake' but $Error is not polluted
remove-module [p]sake

# find psake's path
$psakeModule = (Get-ChildItem (".\src\Packages\psake*\tools\psake.psm1")).FullName | Sort-Object $_ | select -last 1
 
Import-Module $psakeModule

# you can write statements in multiple lines using `
Invoke-psake -buildFile .\src\Build\default.ps1 `
			-taskList teamcity `
             -parameters @{
                "projectVersion" = $projectVersion
                } `
			-framework 4.6

Write-Host "Build exit code:" $LastExitCode

# Propagating the exit code so that builds actually fail when there is a problem
exit $LastExitCode