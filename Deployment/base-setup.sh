#!/usr/bin/env bash

# Update package index and install prerequisites
apt-get update && apt-get install -y \
    apt-transport-https \
    ca-certificates \
    curl \
    git \
    software-properties-common \
    wget

# Install .NET SDK
wget https://packages.microsoft.com/config/ubuntu/$(lsb_release -rs)/packages-microsoft-prod.deb -O packages-microsoft-prod.deb
dpkg -i packages-microsoft-prod.deb
apt-get update
apt-get install -y dotnet-sdk-6.0

# Prepare application directory
mkdir -p /var/docker/metar
git clone https://github.com/Thran34/StudentsManager.git /var/docker/metar
