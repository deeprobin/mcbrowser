#!/bin/bash
SCRIPTS_DIR=$( cd -- "$( dirname -- "${BASH_SOURCE[0]}" )" &> /dev/null && pwd )
REPO_DIR="${SCRIPTS_DIR}/../"

# Misc
LOADER_CHARACTERS=('⣾', '⣽', '⣻', '⢿', '⡿', '⣟', '⣯', '⣷')
RED='\033[0;31m'
CYAN='\033[0;36m'
GREEN='\033[0;32m'
BROWN='\033[0;33m'
YELLOW='\033[1;33m'
NO_COLOR='\033[0m'

# Install states
#INSTALL_STATE_UNKNOWN=-1
INSTALL_STATE_OK=0
INSTALL_STATE_PENDING=1
INSTALL_STATE_INSTALLING=2
INSTALL_STATE_MANUAL=3
INSTALL_STATE_ERR=4

INSTALL_STATE_DOCKER=$INSTALL_STATE_OK

show_status() {
    if [ $1 -eq $INSTALL_STATE_OK ]; then
        echo -e "${GREEN}[✓] ${CYAN}Docker ${GREEN}is installed.${NO_COLOR}"
    elif [ $1 -eq $INSTALL_STATE_PENDING ]; then
        echo -e "${YELLOW}[🞄] ${CYAN}Docker installation pending.${NO_COLOR}"
    elif [ $1 -eq $INSTALL_STATE_INSTALLING ]; then
        echo -e "${YELLOW}[⣾] ${CYAN}Docker${NO_COLOR} is being installed.${NO_COLOR}"
    elif [ $1 -eq $INSTALL_STATE_ERR ]; then
        echo -e "${RED}[✗] ${CYAN}Docker ${GREEN}installation failed.${NO_COLOR}"
    fi
}

show_status $INSTALL_STATE_DOCKER
#while true; do
#    for char in "${LOADER_CHARACTERS[@]}"; do
#        echo -ne "\033[$1D$char\r"
#        sleep 0.1
#    done
#done

if command -v docker &> /dev/null; then
    echo "Docker is already installed."
else
    echo "Docker is not installed. Installing..."
    curl -sL https://get.docker.com | bash
fi

if command -v dotnet &> /dev/null; then
    echo ".NET is already installed."
else
    echo ".NET is not installed. Installing..."
    curl -sL https://dot.net/v1/dotnet-install.sh | bash
fi