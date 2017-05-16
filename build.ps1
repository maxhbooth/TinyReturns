param(
	[string]$buildEnv="local",
	[Int32]$buildNumber=0,
	[String]$branchName="localBuild",
	[String]$gitCommitHash="unknownHash",
	[Switch]$isMainBranch=$False)

cls

$nugetExe = (get-childItem (".\src\.NuGet\NuGet.exe")).FullName
&$nugetExe "restore" ".\src\build\packages.config" "-outputDirectory" ".\src\packages"

# '[p]sake' is the same as 'psake' but $Error is not polluted
remove-module [p]sake

# find psake's path
$psakeModule = (Get-ChildItem (".\src\Packages\psake*\tools\psake.psm1")).FullName | Sort-Object $_ | select -last 1
 
Import-Module $psakeModule

# you can write statements in multiple lines using `
Invoke-psake -buildFile .\src\Build\default.ps1 `
			 -taskList default `
			 -framework 4.0

Write-Host "Build exit code:" $LastExitCode

# Propagating the exit code so that builds actually fail when there is a problem
exit $LastExitCode