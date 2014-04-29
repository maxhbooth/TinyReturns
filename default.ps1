
properties {
	Framework '4.0'

	$projectName = "TinyReturns"
	$baseDir = resolve-path .
	$buildConfig = "Release"
	$databaseChangeOwner = "Paul Herrera"

	$buildFolder = "$baseDir\package"
	$srcFolder = "$baseDir\src"
	$docsFolder = "$baseDir\docs"
	
	$buildTargetFolder = "$buildFolder\$buildConfig"

	$databaseServer = "(local)\sqlexpress"
	$databaseName = $projectName

	$standardDatabaseObjectsFolder = "$baseDir\data\mssql\StandardObjects"
	$databaseScripts = "$baseDir\data\mssql\TinyReturns\TransitionsScripts"

	$dbdeployExec = "$baseDir\lib\dbdeploy\dbdeploy.exe"

	$doDatabaseScriptPath = "$buildFolder\DatabaseUpgrade_GIPS_Local_$dateStamp.sql"
	$undoDatabaseScriptPath = "$buildFolder\DatabaseRollback_GIPS_Local_$dateStamp.sql"
	
	$solutionFile = "$srcFolder\TinyReturns.sln"

	$packageNunitExec = "$srcFolder\packages\xunit.runners.1.9.2\tools\xunit.console.clr4.exe"
}

task default -depends CleanSolution, UpdateNuGetPackages, BuildSolution, RebuildDatabase, RunUnitTests, RunIntegrationTests, PopulateDatabase

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
	
	&$nuGetExec restore "$baseDir\src\.nuget\packages.config" -PackagesDirectory "$baseDir\src\packages"
}

task BuildSolution -depends CleanSolution {
	Exec { msbuild "$solutionFile" /t:Build /p:Configuration=Release /v:quiet /p:OutDir="$buildTargetFolder\" }
	
	Copy-Item "$srcFolder\Logging\Log4NetConfig.xml" "$buildTargetFolder"
}

task RebuildDatabase {
	DropSqlDatabase $databaseServer $databaseName
	CreateSqlDatabase $databaseServer $databaseName
	
	RunDatabaseScriptsFromFolder $databaseServer $databaseName $standardDatabaseObjectsFolder
	
	Exec { &$dbDeployExec `
		-scriptfiles $databaseScripts `
		-dofile $doDatabaseScriptPath `
		-undofile $undoDatabaseScriptPath `
		-connection "Initial Catalog=$databaseName;Data Source=$databaseServer;Integrated Security=SSPI;" `
		-type mssql `
		-deltaset "$projectName" `
		-tablename DatabaseVersion `
		-owner $databaseChangeOwner
	}
	
	ExecuteSqlFile $databaseServer $databaseName $doDatabaseScriptPath
}

task RunUnitTests -depends BuildSolution {
	Exec { &$packageNunitExec "$buildTargetFolder\Dimensional.TinyReturns.UnitTests.dll" /html "$buildFolder\Dimensional.TinyReturns.UnitTests.dll.html" }
}

task RunIntegrationTests -depends BuildSolution {
	Exec { &$packageNunitExec "$buildTargetFolder\Dimensional.TinyReturns.IntegrationTests.dll" /html "$buildFolder\Dimensional.TinyReturns.IntegrationTests.dll.html" }
}

task PopulateDatabase -depends RebuildDatabase {
	$consoleExec = "$buildTargetFolder\Dimensional.TinyReturns.CitiFileImporterConsole.exe"
	
	$netOfFeesFile = "$docsFolder\CitiFileFullNetOfFees.csv"
	$grossOfFeesFile = "$docsFolder\CitiFileFullGrossOfFees.csv"
	$indexFile = "$docsFolder\CitiFileFullIndex.csv"
	
	Write-Host "Executing: $consoleExec $netOfFeesFile $grossOfFeesFile $indexFile"
	
	cd "$buildTargetFolder"
	
	Exec { &$consoleExec "$netOfFeesFile" "$grossOfFeesFile" "$indexFile" }
	
	cd "$baseDir"
}