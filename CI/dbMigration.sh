#!/usr/bin/env bash

shortopts="h:d:r:v:"
longopts="ConsulHost:,ConsulDC:,SqlMigrationsRepo:,BackEndVersion:"

OPTS=`getopt -o $shortopts --long $longopts -n 'dbMigration.sh' -- "$@"`

if [ $? != 0 ] ; then echo "Failed parsing options." ; exit 1; fi 

eval set -- "$OPTS"

while true; do
	case "$1" in
		-h|--ConsulHost ) ConsulHost="$2"; shift 2;;
		-d|--ConsulDC ) ConsulDC="$2"; shift 2;;
		-r|--SqlMigrationsRepo ) SqlMigrationsRepo="$2"; shift 2;;
		-v|--BackEndVersion ) BackEndVersion="$2"; shift 2;;
		--) shift; break;;
	esac
done

PROJECT_NAME="CscGet.Database.Migrations"
PUBLISH_FOLDER_NAME="./Deploy"
PACKAGE_NAME="Dxc.Captn.SqlMigrations.$BackEndVersion"
sourcePath="$PUBLISH_FOLDER_NAME/$PACKAGE_NAME/Dxc.Captn.SqlMigrations"

wget -N $SqlMigrationsRepo/$PACKAGE_NAME.nupkg -P $PUBLISH_FOLDER_NAME
unzip -o $PUBLISH_FOLDER_NAME/$PACKAGE_NAME.nupkg -d $PUBLISH_FOLDER_NAME/$PACKAGE_NAME 
rm -rf $PUBLISH_FOLDER_NAME/$PACKAGE_NAME.nupkg

sudo dotnet $sourcePath/$PROJECT_NAME.dll -h $ConsulHost -d $ConsulDC --runtime linux-musl-x64

success=$?
if [ $success != 0 ]; then 
	echo "Migration failed" ; 
	exit $success; 
fi
