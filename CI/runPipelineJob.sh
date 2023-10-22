#!/usr/bin/env bash

set -e

Die () {
	echo "$1"
	exit 1
}

longopts='ProjectId:,Branch:,Job:,Token:'

Token="zt628xSx9EUJM48muxdo"

OPTS=$(getopt -o '' -l $longopts -n 'runPipelineJob.sh' -- "$@" || Die "Failed parsing options!")
eval set -- "$OPTS"
while true; do
	case "$1" in
		--ProjectId ) ProjectId="$2"; shift 2;;
		--Branch ) Branch="$2"; shift 2;;
		--Job ) Job="$2"; shift 2;;
		--Token ) Token="$2"; shift 2;;
		--) shift; break;;
	esac
done

echo "Project Url $CI_PROJECT_URL"
echo "Project ID $ProjectId"
echo "Branch $Branch"
echo "Job $Job"

gitlabHost=`echo "$CI_PROJECT_URL" | python -c "from urlparse import urlparse; import sys; path=urlparse(sys.stdin.read().strip()); print path.scheme + '://' + path.netloc"`
apiRoot="$gitlabHost/api/v4"

echo "gitlab api - $apiRoot"

echo "URI $apiRoot/projects/$ProjectId/pipelines"

lastPipelineId=`curl -X GET "$apiRoot/projects/$ProjectId/pipelines?ref=$Branch&order_by=id" -H "Private-Token: $Token" -H "Cache-Control: no-cache" | python -c 'import sys, json; print json.load(sys.stdin)[0]["id"]'`
echo "PipelineId: $lastPipelineId"

jobs=`curl -g -X GET "$apiRoot/projects/$ProjectId/pipelines/$lastPipelineId/jobs?scope[]=manual&scope[]=skipped" -H "Private-Token: $Token" -H "Cache-Control: no-cache"`
jobToStartId=`echo $jobs | jq "map(select(.name == \"$Job\")) | sort_by(.id) | reverse | .[0].id"`

if [ ! -z $jobToStartId ] && [ "$jobToStartId" != "null" ] ; then
	echo "Job To start Id: $jobToStartId"
	jobStatus=`curl -X POST "$apiRoot/projects/$ProjectId/jobs/$jobToStartId/play" -H "Private-Token: $Token" -H "Cache-Control: no-cache"`
else
	jobs=`curl -g -X GET "$apiRoot/projects/$ProjectId/pipelines/$lastPipelineId/jobs?scope[]=failed&scope[]=success&scope[]=canceled" -H "Private-Token: $Token" -H "Cache-Control: no-cache"`
	jobToStartId=`echo $jobs | jq "map(select(.name == \"$Job\")) | sort_by(.id) | reverse | .[0].id"`
	echo "Job To start Id: $jobToStartId"
	jobStatus=`curl -X POST "$apiRoot/projects/$ProjectId/jobs/$jobToStartId/retry" -H "Private-Token: $Token" -H "Cache-Control: no-cache"`
fi

echo "$jobStatus"
jobStatusId=`echo $jobStatus | python -c 'import sys, json; print json.load(sys.stdin)["id"]'`

interval=10
Timeout=1200
failcode=1

printf "Waiting..."
sleep $interval
printf "."

for ((waited=0;waited<$Timeout;waited+=$interval)); do
	currentJobStatus=`curl -s -X GET "$apiRoot/projects/$ProjectId/jobs/$jobStatusId" -H "Private-Token: $Token" -H "Cache-Control: no-cache" | python -c 'import sys, json; print json.load(sys.stdin)["status"]'`
	if [ $currentJobStatus = "success" ] ; then
		failcode=0
		break
	elif [ $currentJobStatus = "failed" ] || [ $currentJobStatus = "canceled" ] ; then
		jobTrace=`curl -s -X GET "$apiRoot/projects/$ProjectId/jobs/$jobStatusId/trace" -H "Private-Token: $Token" -H "Cache-Control: no-cache"`
		echo "$jobTrace"
		failcode=1
		break
	fi

	sleep $interval
	printf "."
done

echo

if [ $failcode -gt 0 ] ; then
	echo "$jobTrace"
	exit 1
fi
