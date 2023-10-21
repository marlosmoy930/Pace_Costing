#!/usr/bin/env bash

Die () {
	echo "$1"
	exit 1
}

set -e

longopts='ConsulHost:,ConsulDC:,QueuesPrefix:'

OPTS=$(getopt -o '' -l $longopts -n 'removeq.sh' -- "$@" || Die "Failed parsing options!")
eval set -- "$OPTS"
while true; do
	case "$1" in
		--ConsulHost ) ConsulHost="$2"; shift 2;;
		--ConsulDC ) ConsulDC="$2"; shift 2;;
		--QueuesPrefix ) QueuesPrefix="$2"; shift 2;;
		--) shift; break;;
	esac
done

GetValueFromConsul () {
	key=$1
	resp=`curl -X GET "$ConsulHost/v1/kv/$key?dc=$ConsulDC" | python -c 'import sys, json; print json.load(sys.stdin)[0]["Value"]' | base64 -d`
	echo $resp
}

rabbitMqHostKey="Costing/RabbitMq/Host"
rabbitMqUsernameKey="Costing/RabbitMq/UserName"
rabbitMqPasswordKey="Costing/RabbitMq/Password"

#rabbitMqHost=`GetValueFromConsul $rabbitMqHostKey | grep -Po 'rabbitmq://\K[^/]*'`
rabbitMqHost="rabbitmq.$QueuesPrefix.captn.evaluate-it.cloud"
username=`GetValueFromConsul $rabbitMqUsernameKey`
password=`GetValueFromConsul $rabbitMqPasswordKey`

echo "RabbitMqHost: $rabbitMqHost"
echo "QeueusPrefix: $QueuesPrefix"

queuesNames=`rabbitmqadmin --host="$rabbitMqHost" --port=443 --ssl --vhost=/ --username="$username" --password="$password" -f tsv -q list queues name`

for queue in ${queuesNames[*]}
do
	printf 'Dropping queue %s. ' "$queue"
	{
	    rabbitmqadmin --host="$rabbitMqHost" --port=443 --ssl --vhost=/ --username="$username" --password="$password" delete queue name=${queue}
	} || { 
		echo "Exception during deleting"
	}
done
