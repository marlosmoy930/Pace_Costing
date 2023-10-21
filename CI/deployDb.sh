#!/usr/bin/env bash

set -e

Die () {
	echo "$1"
	exit 1
}

longopts='BackEndVersion:,ConsulHost:,ConsulDC:,DbRepo:,DbBranch:'

DbBranch="dev"

OPTS=$(getopt -o '' -l $longopts -n 'deployDb.sh' -- "$@" || Die "Failed parsing options!")
eval set -- "$OPTS"
while true; do
	case "$1" in 
		--BackEndVersion ) BackEndVersion="$2"; shift 2;;
		--ConsulHost ) ConsulHost="$2"; shift 2;;
		--ConsulDC ) ConsulDC="$2"; shift 2;;
		--DbRepo ) DbRepo="$2"; shift 2;;
		--DbBranch ) DbBranch="$2"; shift 2;;
		--) shift; break ;;
	esac
done

GetValueFromConsul () {
	key=$1
	resp=`curl -X GET "$ConsulHost/v1/kv/$key?dc=$ConsulDC" | python -c 'import sys, json; print json.load(sys.stdin)[0]["Value"]' | base64 -d`
	echo $resp
}

if [ $CI_BUILD_REF_NAME = "master" ] ; then
    DbBranch="master"
fi

PackagePath="Deploy/db"
DbMocksRepo="-b $DbBranch http://gitlab-ci-token:$CI_JOB_TOKEN@vm199251.projects.local/CSC/CAP-N-DB_Mocks.git "
PackageName="Dxc.Captn.Database.$BackEndVersion"

costingDbConsulKey="Costing/ConnectionStrings/RelationDb"
mongoDbConsulKey="Costing/ConnectionStrings/MongoDb"
laborRatesMongoDbConsulKey="Costing/ConnectionStrings/LaborRatesMongoDb"
allocationsMongoConsulDbKey="Costing/ConnectionStrings/AllocationsMongoDb"

deploymentConnectionString=`GetValueFromConsul $costingDbConsulKey`
echo "DeploymentConnectionString: $deploymentConnectionString"

deploymentDbName=`echo "'$deploymentConnectionString'" | grep -Po 'Initial Catalog=\K[^;"\s]*'`
if [ ${#deploymentDbName} == 0 ] ; then Die "Incorrect connectiong string" ; fi

deploymentDbUser=`echo "'$deploymentConnectionString'" | grep -Po 'User ID=\K[^;\s"]*'`
deploymentDbPass=`echo "'$deploymentConnectionString'" | grep -Po 'Password=\K[^;"\s]*'`
deploymentDbServer=`echo "'$deploymentConnectionString'" | grep -Po 'Data Source=\K[^;"\s]*'`

deploymentAllocationsConnectionString=`GetValueFromConsul $allocationsMongoConsulDbKey`
if [ ${#deploymentAllocationsConnectionString} == 0 ] ; then Die "Parsing of MongoDB name for key _ '$allocationsMongoConsulDbKey' _ has failed" ; fi 
deploymentMongoConnectionString=`GetValueFromConsul $mongoDbConsulKey`
if [ ${#deploymentMongoConnectionString} == 0 ] ; then Die "Parsing of MongoDB server name for key _ '$mongoDbConsulKey' _ has failed" ; fi 
deploymentLaborRatesConnectionString=`GetValueFromConsul $laborRatesMongoDbConsulKey`
if [ ${#deploymentLaborRatesConnectionString} == 0 ] ; then Die "Parsing of MongoDB server name for key _ '$laborRatesMongoDbConsulKey' _ has failed" ; fi

rm -rf $PackagePath
mkdir -p $PackagePath

# downloading database package 
wget $DbRepo/$PackageName.nupkg -P $PackagePath
unzip -o $PackagePath/$PackageName.nupkg -d $PackagePath/$PackageName 
rm -rf $PUBLISH_FOLDER_NAME/$PackageName.nupkg

# publising database to the server
dacpacPath="$PackagePath/Dxc.Captn.Database.$BackEndVersion/Dxc.Captn.Database/CscGet.Database.dacpac"
sqlpackage /Action:Publish /SourceFile:$dacpacPath /TargetConnectionString:"$deploymentConnectionString"  /p:CreateNewDatabase="True"

git clone $DbMocksRepo "$PackagePath/CAP-N-DB_Mocks"
chmod 755 "$PackagePath/CAP-N-DB_Mocks/Clean_and_populate_dictionaries.sh"
$PackagePath/CAP-N-DB_Mocks/Clean_and_populate_dictionaries.sh --Username $deploymentDbUser --Password $deploymentDbPass --Server $deploymentDbServer --Database $deploymentDbName --MongoConnection $deploymentMongoConnectionString --MongoAllocationsConnection $deploymentAllocationsConnectionString --MongoLogConnection $deploymentAllocationsConnectionString --MongoLaborRatesConnection $deploymentLaborRatesConnectionString
