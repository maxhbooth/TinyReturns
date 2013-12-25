properties {
	$projectName = "TinyReturns"
	$baseDir = resolve-path .
	$databaseChangeOwner = "Paul Herrera"

	$buildFolder = "$baseDir\package"

	$databaseServer = "(local)\sqlexpress"
	$databaseName = $projectName

	$standardDatabaseObjectsFolder = "$baseDir\data\mssql\StandardObjects"
	$databaseScripts = "$baseDir\data\mssql\TinyReturns\TransitionsScripts"

	$dbdeployExec = "$baseDir\lib\dbdeploy\dbdeploy.exe"

	$doDatabaseScriptPath = "$buildFolder\DatabaseUpgrade_GIPS_Local_$dateStamp.sql"
	$undoDatabaseScriptPath = "$buildFolder\DatabaseRollback_GIPS_Local_$dateStamp.sql"
}

task default -depends CleanSolution, RebuildDatabase

formatTaskName {
	param($taskName)
	write-host "********************** $taskName **********************" -foregroundcolor Green
}

task CleanSolution {
	if (Test-Path $buildFolder) {
		rd $buildFolder -rec -force | out-null
	}

	mkdir $buildFolder | out-null
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
