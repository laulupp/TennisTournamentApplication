#!/bin/bash

i=300
while[ ! -f /tmp/db_ready ] && [ $i -gt 0];
do
	echo Waiting for db to be ready
	i=$(( $i-1 ))
	sleep 1
done

if [ $i -eq 0 ]; then
	exit 1;
fi