#!/usr/bin/env bash

Die () {
	echo "$1"
	exit 1
}

longopts='ConsulHost:,ConsulDC:,RedisServer:,Port:'

Port="6379"

OPTS=$(getopt -o '' -l $longopts -n 'redis.sh' -- "$@" || Die "Failed parsing options!")
eval set -- "$OPTS"
while true; do
	case "$1" in
		--ConsulHost ) ConsulHost="$2"; shift 2;;
		--ConsulDC ) ConsulDC="$2"; shift 2;;
        --RedisServer ) RedisServer="$2"; shift 2;;
        --Port ) Port="$2"; shift 2;;
		--) shift; break;;
	esac
done

redisConsulKey="Costing/ConnectionStrings/RedisCache"
redisConfiguration=`curl -X GET "$ConsulHost/v1/kv/$redisConsulKey?dc=$ConsulDC" | python -c 'import sys, json; print json.load(sys.stdin)[0]["Value"]' | base64 -d`
redisConfiguration=${redisConfiguration#*,}
redisHost=${redisConfiguration%%:*}
redisPort=${redisConfiguration#*:}
echo "RedisHost: $redisHost RedisPort: $redisPort"

if [ -z $redisHost ] ; then
    redisHost=$RedisServer
    redisPort=$Port
fi

echo "clear Redis"
redis-cli -h $redisHost -p $redisPort "FLUSHALL"

exit $?
