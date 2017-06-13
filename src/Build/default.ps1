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

	$buildLibFolder = "$buildFolder\lib"
	$buildDataFolder = "$buildFolder\data"
	$buildWebFolder = "$buildFolder\Web"

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

 task teamcity -depends CleanSolution, UpdateNuGetPackages, BuildSolution, `
 RebuildDatabase, RunUnitTests, RunIntegrationTests, PopulateReturnsData,RebuildDatabase,  ZipFile



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

task copyBuildFiles -depends BuildSolution {
    #buildsolution runs cleansolution, which removes all these directories for us,
    # so we don't have conflicts.
    
    mkdir $buildWebFolder | out-null
	
	$sourceFiles = "$buildTargetFolder\_PublishedWebsites\Web\*"
	Write-Host "Copying files from '$sourceFiles' to '$buildWebFolder'"
	copy-item $sourceFiles "$buildWebFolder" -recurse


    mkdir $buildTargetFolder\_PublishedWebsites\Web\bin\roslyn |out-null

	$roslyn = "$buildTargetFolder\roslyn\*"
	Write-Host "Copying files from '$roslyn' to '$buildWebFolder'"
	copy-item $roslyn "$buildTargetFolder\_PublishedWebsites\Web\bin\roslyn" -recurse

	mkdir $buildLibFolder | out-null

	$destXunitFolder = "$buildLibFolder\xunit"
	mkdir $destXunitFolder | out-null
	copy-item "$srcFolder\packages\xunit.runner.console.2.0.0\tools\*" $destXunitFolder -recurse
	
    $destRoundhouseFolder = "$buildLibFolder\roundhouse"
	mkdir $destRoundhouseFolder | out-null
	copy-item "$srcFolder\packages\roundhouse.0.8.6\bin\*" $destRoundhouseFolder  -recurse

	$msSqlFolder = "$buildDataFolder\mssql"
	mkdir $msSqlFolder | out-null
	copy-item "$dataFolder\mssql\*" $msSqlFolder -recurse

}

task ZipFile -depends copyBuildFiles -requiredVariables projectVersion{

    $zipExec = "$srcFolder\packages\7-Zip.CommandLine.9.20.0\tools\7za.exe"

    $versionStamp = $projectVersion -replace "\.", "_"

    Exec { &$zipExec a "-x!*.zip" "-x!*.dat" "$buildFolder\TinyReturns_App_$versionStamp.zip" "$buildFolder\*" }

}