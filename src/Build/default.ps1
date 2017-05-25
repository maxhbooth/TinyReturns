Framework '4.0'

properties {
	$projectName = "TinyReturns"
	$baseDir = resolve-path ".\..\.."
	$buildConfig = "Release"
	$databaseChangeOwner = "Paul Herrera"

	$buildFolder = "$baseDir\.build"
	$srcFolder = "$baseDir\src"
	$docsFolder = "$baseDir\docs"
	$dataFolder = "$baseDir\data"
	
	$buildTargetFolder = "$buildFolder\$buildConfig"

	$databaseServer = "(local)\sqlexpress"
	$databaseName = $projectName

	$standardDatabaseObjectsFolder = "$baseDir\data\mssql\StandardObjects"
	$databaseScripts = "$baseDir\data\mssql\TinyReturns\TransitionsScripts"

	$dbdeployExec = "$baseDir\lib\dbdeploy\dbdeploy.exe"
	$roundhouseExec = "$srcFolder\packages\roundhouse.0.8.6\bin\rh.exe"

	$doDatabaseScriptPath = "$buildFolder\DatabaseUpgrade_GIPS_Local_$dateStamp.sql"
	$undoDatabaseScriptPath = "$buildFolder\DatabaseRollback_GIPS_Local_$dateStamp.sql"
	
	$solutionFile = "$srcFolder\TinyReturns.sln"

	$packageXunitExec = "$srcFolder\packages\xunit.runner.console.2.0.0\tools\xunit.console.exe"
}

task default -depends CleanSolution, UpdateNuGetPackages, BuildSolution, RebuildDatabase, RunUnitTests, RunIntegrationTests, PopulateReturnsData

task databaseonly -depends RebuildDatabase, PopulateReturnsData

formatTaskName {
	param($taskName)
	write-host "********************** $taskName **********************" -foregroundcolor Green
}

task CleanSolution {
	if (Test-Path $buildFolder) {
		rd $buildFolder -rec -force | out-null
	}

	mkdir $buildFolder | out-null

	Exec { msbuild "$solutionFile" /t:Clean /p:Configuration=$buildConfig /v:quiet }
}

task UpdateNuGetPackages {
	$nuGetExec = "$baseDir\src\.nuget\NuGet.exe"
	
	&$nuGetExec restore "$solutionFile" -PackagesDirectory "$baseDir\src\packages"
}

task BuildSolution -depends CleanSolution {
	Exec { msbuild "$solutionFile" /t:Build /p:Configuration=Release /v:quiet /p:OutDir="$buildTargetFolder\" }
	
	Copy-Item "$srcFolder\Logging\Log4NetConfig.xml" "$buildTargetFolder"
}

task RunUnitTests -depends BuildSolution {
	Exec { &$packageXunitExec "$buildTargetFolder\Dimensional.TinyReturns.UnitTests.dll" -html "$buildFolder\Dimensional.TinyReturns.UnitTests.dll.html" -parallel none }
}

task RunIntegrationTests -depends BuildSolution {
	Exec { &$packageXunitExec "$buildTargetFolder\Dimensional.TinyReturns.IntegrationTests.dll" -html "$buildFolder\Dimensional.TinyReturns.IntegrationTests.dll.html" -parallel none }
}

task RebuildDatabase {
	$dbFileDir = "$dataFolder\mssql\TinyReturns"
	$versionFile = "$dbFileDir\_BuildInfo.xml"
	$enviornment = "LOCAL"

	Exec {
		&$roundhouseExec /d=$databaseName /f=$dbFileDir /s=$databaseServer /vf=$versionFile /vx='//buildInfo/version' /env=$enviornment /drop /silent
	}

	Exec {
		&$roundhouseExec /d=$databaseName /f=$dbFileDir /s=$databaseServer /vf=$versionFile /vx='//buildInfo/version' /env=$enviornment /simple /silent
	}
}

task PopulateReturnsData -depends BuildSolution {
	Exec { &"$buildFolder\Release\Dimensional.TinyReturns.TestDataPopulatorConsole.exe" }
}
