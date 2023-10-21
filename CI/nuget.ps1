param(
        [string]$ApiKey,
	[string]$Version,
        [string]$ReportingRepo = $ARTIFACTORY,
        [string]$DatabaseRepo = $ARTIFACTORY,
        [string]$CalculationRepo = $ARTIFACTORY,
        [string]$SqlMigrationsRepo = $ARTIFACTORY,
        [string]$CostingRepo = $ARTIFACTORY

)
$ErrorActionPreference = "Stop";

function Create-Zip {
        param (
                [string]$ArchivePath,
                [string]$ArchiveSource
        )

        7z a -r $ArchivePath $ArchiveSource
        if (-not $?){
                throw "Can't create archive"
        }
}

function Nuget-Pack {
        param (
                [string]$NuspecPath,
                [string]$Version,
                [string]$OutputDirectory
        )

        nuget.exe pack $NuspecPath -OutputDirectory $OutputDirectory -Version $Version
        if (-not $?){
                throw "Can't create nuget package"
        }
}

function Nuget-Publish {
        param (
                [string]$NugetPath,
                [string]$ApiKey,
                [string]$Source
        )

        nuget.exe push $NugetPath $ApiKey -Source $Source
        if (-not $?){
                throw "Can't push package"
        }
}

# creating nuget package of SqlMigrations
Nuget-Pack -NuspecPath CI\$DB_MIGRATIONS_PROJECT_NAME.nuspec -OutputDirectory build -Version $Version
Nuget-Publish -NugetPath build\$DB_MIGRATIONS_PROJECT_NAME.$Version.nupkg -ApiKey $ApiKey -Source $SqlMigrationsRepo

# creating nuget package of Database
Nuget-Pack -NuspecPath CI\$DB_PROJECT_NAME.nuspec -OutputDirectory build -Version $Version
Nuget-Publish -NugetPath build\$DB_PROJECT_NAME.$Version.nupkg -Source $DatabaseRepo -ApiKey $ApiKey
