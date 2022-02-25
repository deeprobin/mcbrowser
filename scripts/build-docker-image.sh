#!/bin/sh
SCRIPTS_DIR=$( cd -- "$( dirname -- "${BASH_SOURCE[0]}" )" &> /dev/null && pwd )
REPO_DIR="${SCRIPTS_DIR}/../"
DOCKER_FILE="${REPO_DIR}/sources/MinecraftServerlist/Dockerfile"

docker build $REPO_DIR -f $DOCKER_FILE -t mcbrowser:0.1.0