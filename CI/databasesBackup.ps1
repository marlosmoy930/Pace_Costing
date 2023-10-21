param(
    [string]$ShareUrl,
    [string]$ShareUsername,
    [string]$SharePassword,
    [string]$ConsulDc,

    [string]$ConsulHost = "http://192.168.240.253:8500"
)

$ErrorActionPreference = "Stop";

function InvokeQuery($Query, $ErrorAction = "Stop")
{
    Invoke-Sqlcmd -ServerInstance $SqlServerAddress -Username $SqlServerUsername -Password $SqlServerPass -Query $Query -ErrorAction $ErrorAction
}

function TakeSqlBackup($SqlDbName)
{
    Write-Host "Taking SQL backup $ShareUrl\$SqlDbName.bak"
    
    $tempBackupFolder = "C:\Program Files\Microsoft SQL Server\MSSQL11.MSSQLSERVER\MSSQL\Backup"

    Remove-Item "$ShareUrl\$SqlDbName.bak" -Force -ErrorAction Continue

    InvokeQuery -Query "EXEC sp_configure 'show advanced options', 1"
    InvokeQuery -Query "RECONFIGURE"
    InvokeQuery -Query "EXEC sp_configure 'xp_cmdshell', 1"

    InvokeQuery -Query "ALTER DATABASE $SqlDbName SET SINGLE_USER WITH ROLLBACK IMMEDIATE"
    
    InvokeQuery -Query "BACKUP DATABASE $SqlDbName TO DISK = '$tempBackupFolder\$SqlDbName.bak' WITH INIT"
    
    InvokeQuery -Query "ALTER DATABASE $SqlDbName SET MULTI_USER"
    InvokeQuery -Query "EXEC xp_cmdshell 'robocopy ""$tempBackupFolder"" $ShareUrl $SqlDbName.bak /IS'"
}

function TakeMongoBackup($backup)
{
    Write-Host "Taking mongo backup $ShareUrl\$backup"

    if (-not (Test-Path Z:\)) { net use Z: $ShareUrl /user:$ShareUsername $SharePassword }
    if (Test-Path Z:\$backup) { Remove-Item "Z:\$backup" -Recurse -Force }

    $ErrorActionPreference = "Continue";
    mongodump --host "$mongoServer" --quiet --out $ShareUrl --gzip --db $backup
    $ErrorActionPreference = "Stop";
}

function GetValueFromConsul($key){
    $resp = Invoke-RestMethod -Uri "$ConsulHost/v1/kv/$($key)?dc=$ConsulDc"

    Return [System.Text.Encoding]::UTF8.GetString([System.Convert]::FromBase64String($resp.Value));
}

function GetSqlServerEntityValue($key)
{
    $ErrorActionPreference = "Stop";
    
    $connectionString =  GetValueFromConsul("Costing/ConnectionStrings/RelationDb")
    $connectionStringBuilder = New-Object System.Data.SqlClient.SqlConnectionStringBuilder($connectionString)
    $entityValue = $connectionStringBuilder.$key

    if (-not $entityValue){
        throw "Parsing Sql Server DB connection entity key _ $key _ failed! Key does not exists!"
    }
    else {
        $ErrorActionPreference = "Continue";
        Return $entityValue
    }
}

function GetMongodbName($key) {
    $ErrorActionPreference = "Stop";
    $rest =  GetValueFromConsul("Costing/ConnectionStrings/$key")
    $dbName = $rest.Replace("mongodb://","").Split('/')[1]
    if (-not $dbName) {
        throw "Parsing of MongoDB name for key _ $key _ has failed"
    }
    else {
        $ErrorActionPreference ="Continue"
        Return $dbName
    }
}

function GetMongodbServer($key) {
    $ErrorActionPreference = "Stop";
    $rest =  GetValueFromConsul("Costing/ConnectionStrings/$key")
    $serverName= $rest.Replace("mongodb://","").Split('/')[0].Split(',')[0]
    if (-not $serverName) {
        throw "Parsing of MongoDB name for key _ $key _ has failed"
    }
    else {
        Write-Host "Server: $serverName"
        $ErrorActionPreference ="Continue"
        Return $serverName
    }
}

Push-Location
if ( $ConsulDc -eq "ss" ) 
{
    $mongoServer = GetValueFromConsul("Costing/MongoServer")
    
    $SqlServerUsername = GetValueFromConsul("Costing/SqlServerUsername")
    $SqlServerPass = GetValueFromConsul("Costing/SqlServerPassword")
    $SqlServerAddress = GetValueFromConsul("Costing/SqlServerAddress")
    
    $targetDbName = GetValueFromConsul("Costing/SqlDbName")
    $targetAllocations = GetValueFromConsul("Costing/MongoAllocationsDbName")
    $targetMainDb = GetValueFromConsul("Costing/MongoDbName")
    $targetLrt = GetValueFromConsul("Costing/MongoLrtDbName")
}
else {
    $mongoServer = GetMongodbServer("AllocationsMongoDb")
    
    $targetDbName = GetSqlServerEntityValue("Initial Catalog")
    $SqlServerUsername = GetSqlServerEntityValue("User ID")
    $SqlServerPass = GetSqlServerEntityValue("Password")
    $SqlServerAddress = GetSqlServerEntityValue("Data Source")
    
    $targetAllocations = GetMongodbName("AllocationsMongoDb")
    $targetMainDb = GetMongodbName("MongoDb")
    $targetLrt = GetMongodbName("LaborRatesMongoDb")
}

Write-Host "Starting backing up $targetDbName on server $SqlServerAddress via $SqlServerUsername@$SqlServerPass"

$sw = [Diagnostics.Stopwatch]::StartNew()
TakeSqlBackup -SqlDbName $targetDbName

Write-Host "Sql backup took $($sw.Elapsed)"

TakeMongoBackup -backup $targetAllocations
TakeMongoBackup -backup $targetMainDb
TakeMongoBackup -backup $targetLrt

Write-Host "Mongo backup took $($sw.Elapsed)"
Pop-Location
