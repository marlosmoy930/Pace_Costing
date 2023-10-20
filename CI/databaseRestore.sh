#!/usr/bin/env bash

set -e

Die () {
	echo "$1"
	exit 1
}

longopts='ShareUrl:,ShareUsername:,SharePassword:,ConsulDC:,BackupFile:,ConsulHost:,BackupDbName:,BackupMongoDbName:,BackupMongoAllocationsDbName:,BackupMongoLrtDbName:,RestoreMongo'

BackupFile="CscGet_Production.bak"
ConsulHost="http://192.168.240.253:8500"
BackupDbName="CscGet_Production"
BackupMongoDbName="CscGet_release"
BackupMongoAllocationsDbName="CscGetAllocations_release"
BackupMongoLrtDbName="CscGetLaborRates_release"
RestoreMongo=false

OPTS=$(getopt -o '' -l $longopts -n 'databaseRestore.sh' -- "$@" || Die "Failed parsing options!")
eval set -- "$OPTS"
while true; do
	case "$1" in 
		--ShareUrl ) ShareUrl="$2"; shift 2;;
		--ShareUsername ) ShareUsername="$2"; shift 2;;
		--SharePassword ) SharePassword="$2"; shift 2;;
		--ConsulDC ) ConsulDC="$2"; shift 2;;
		--BackupFile ) BackupFile="$2"; shift 2;;
		--ConsulHost ) ConsulHost="$2"; shift 2;;
		--BackupDbName ) BackupDbName="$2"; shift 2;;
		--BackupMongoDbName ) BackupMongoDbName="$2"; shift 2;;
		--BackupMongoAllocationsDbName ) BackupMongoAllocationsDbName="$2"; shift 2;;
		--BackupMongoLrtDbName ) BackupMongoLrtDbName="$2"; shift 2;;
		--RestoreMongo ) RestoreMongo=true; shift 1;;
		--) shift; break ;;
	esac
done

GetValueFromConsul () {
	key=$1
	resp=`curl -X GET "$ConsulHost/v1/kv/$key?dc=$ConsulDC" | python -c 'import sys, json; print json.load(sys.stdin)[0]["Value"]' | base64 -d`
	echo $resp
}

Execute_DB_query () {
	queryOptions="-S $sqlServerName -U $sqlUserId -P $sqlUserPassword -d master -t 300"

	opts=`getopt -o bq: -n 'databaseRestore.sh' -- "$@"`
	eval set -- "$opts"
	
	while true; do
		case "$1" in
			-q ) query="$2"; shift 2;;
			-b ) queryOptions+=" -b"; shift 1;;
			--) shift; break;;
		esac
	done

	echo $query
	sqlcmd $queryOptions -Q "$query" || Die "Failed executing operation under DB"	
}

RestoreSqlBackup() {
	BackupFile="$1"
	BackedUpDatabase="$2"
	TargetDatabase="$3"
	
	dataTargetFolder='C:\Program Files\Microsoft SQL Server\MSSQL11.MSSQLSERVER\MSSQL\DATA'

	query="EXEC sp_configure 'show advanced options', 1;
		RECONFIGURE;
		EXEC sp_configure 'xp_cmdshell', 1;
		RECONFIGURE;
		EXEC xp_cmdshell 'NET USE Z: $ShareUrl $SharePassword /USER:$ShareUsername'"
	Execute_DB_query -q "${query[@]}" -b

	echo "Dropping $TargetDatabase"
	query="ALTER DATABASE [$TargetDatabase] SET SINGLE_USER WITH ROLLBACK IMMEDIATE;
		DROP DATABASE [$TargetDatabase]"
	Execute_DB_query -q "${query[@]}"

	query="RESTORE DATABASE [$TargetDatabase] FROM DISK = 'Z:\\$BackupFile' WITH MOVE '$BackedUpDatabase' TO '$dataTargetFolder\\$TargetDatabase.mdf', MOVE '${BackedUpDatabase}_log' TO '$dataTargetFolder\\$TargetDatabase.ldf', REPLACE;
		ALTER DATABASE [$TargetDatabase] SET MULTI_USER"
	Execute_DB_query -q "${query[@]}" -b
	echo "DB '$TargetDatabase' was restored"
}

RestoreMongoBackup() {
	backup="$1"
	target="$2"

	backupFile="/mnt/DatabaseBackups/$backup"

	if [ ! -f $backupFile ]; then
    	echo "File '$backupFile' not found!. Restore MongoDb for '$target' failed."
		return 0
	fi

	mongo $target --host "$mongoServer" --eval "db.dropDatabase()"
	if [ $RestoreMongo = true ]; then
		mongorestore --host "$mongoServer" --drop --gzip --db $target "$backupFile" || Die "Failed executing operation under MongoDB"
	fi
}

GetMongodbServer() {
	key="Costing/ConnectionStrings/$1"
	connectionString=`GetValueFromConsul $key`
	servers=`echo "'$connectionString'" | grep -Po 'mongodb://\K[^/]*'`
	serverName=${servers%%,*}

	echo $serverName
}

GetMongodbName() {
	key="Costing/ConnectionStrings/$1"
	connectionString=`GetValueFromConsul $key`
	server=${connectionString//'mongodb://'/''}
	serverName=`echo "'$server'" | grep -Po '/\K[^\x27]*'`

	echo $serverName
}

MigrateCscGetMongoCollections() {
	target=$1
	mongo $target --host "$mongoServer" ./CI/dropAndMigrateMongoCollections.js
}

consulKey="Costing/ConnectionStrings/RelationDb"
connectionString=`GetValueFromConsul $consulKey`
sqlTargetDatabase=`echo "'$connectionString'" | grep -Po 'Initial Catalog=\K[^;"\s]*'`

if [ ${#sqlTargetDatabase} == 0 ] ; then Die "Incorrect connectiong string" ; fi

sqlUserId=`echo "'$connectionString'" | grep -Po 'User ID=\K[^;\s"]*'`
sqlUserPassword=`echo "'$connectionString'" | grep -Po 'Password=\K[^;"\s]*'`
sqlServerName=`echo "'$connectionString'" | grep -Po 'Data Source=\K[^;"\s]*'`

allocationsMongoDbKey="AllocationsMongoDb"
mongoDbKey="MongoDb"
lrtMongoDbKey="LaborRatesMongoDb"

mongoServer=`GetMongodbServer $allocationsMongoDbKey`
if [ ${#mongoServer} == 0 ] ; then Die "Parsing of MongoDB server name for key _ '$allocationsMongoDbKey' _ has failed" ; fi 

mongoTargetAllocations=`GetMongodbName $allocationsMongoDbKey`
if [ ${#mongoTargetAllocations} == 0 ] ; then Die "Parsing of MongoDB name for key _ '$allocationsMongoDbKey' _ has failed" ; fi 
mongoTargetMainDb=`GetMongodbName $mongoDbKey`
if [ ${#mongoTargetMainDb} == 0 ] ; then Die "Parsing of MongoDB server name for key _ '$mongoDbKey' _ has failed" ; fi 
mongoTargetLrt=`GetMongodbName $lrtMongoDbKey`
if [ ${#mongoTargetLrt} == 0 ] ; then Die "Parsing of MongoDB server name for key _ '$lrtMongoDbKey' _ has failed" ; fi

echo "Starting restore $BackupDbName to $sqlTargetDatabase on server $sqlServerName via $sqlUserId@$sqlUserPassword"

RestoreSqlBackup $BackupFile $BackupDbName $sqlTargetDatabase

echo "Sql Restored from '$BackupDbName' to '$sqlTargetDatabase'"

RestoreMongoBackup $BackupMongoDbName $mongoTargetMainDb
MigrateCscGetMongoCollections $mongoTargetMainDb
RestoreMongoBackup $BackupMongoAllocationsDbName $mongoTargetAllocations
RestoreMongoBackup $BackupMongoLrtDbName $mongoTargetLrt
