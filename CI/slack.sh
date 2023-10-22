#!/usr/bin/env bash

Die () {
	echo "$1"
	exit 1
}

longopts='Text:,SlackUri:'

OPTS=$(getopt -o '' -l $longopts -n 'slack.sh' -- "$@" || Die "Failed parsing options!")
eval set -- "$OPTS"
while true; do
	case "$1" in
		--Text ) Text="$2"; shift 2;;
		--SlackUri ) SlackUri="$2"; shift 2;;
		--) shift; break;;
	esac
done

lastCommitMessage=`git log -1 --pretty=%B`
echo "$lastCommitMessage"

body=`printf '{"text": "%s, blame %s, last update is %s", "channel": "#environment-rebuilds", "username": "Murrr", "icon_emoji": ":smile_cat:"}' \
	"$Text" "$GITLAB_USER_EMAIL" "$lastCommitMessage"`

curl -X POST "$SlackUri" -H "Content-Type: application/json" -d "$body"

exit $?
