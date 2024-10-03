#!/bin/bash
# Log output to a file for debugging
exec > /var/log/user-data.log 2>&1

# Update system package manager
sudo yum update -y

# Install Docker
sudo yum install -y docker

# Enable Docker service to start on boot
sudo systemctl enable docker

# Start Docker service
sudo systemctl start docker

# Add the current user (ec2-user) to the docker group
sudo usermod -aG docker $USER

newgrp docker

# Install Docker Compose
sudo curl -L "https://github.com/docker/compose/releases/download/v2.24.0/docker-compose-$(uname -s)-$(uname -m)" -o /usr/local/bin/docker-compose

# Make Docker Compose executable
sudo chmod +x /usr/local/bin/docker-compose

# Create a symbolic link for Docker Compose
sudo ln -s /usr/local/bin/docker-compose /usr/bin/docker-compose

# Install Git
sudo yum install -y git

# Install Telnet (optional based on your use case)
sudo yum install -y telnet

# Clone the GitHub repository using Personal Access Token (PAT)
GIT_REPO_URL="https://erisekasaputra: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@github.com/erisekasaputra/santai_mvp.git"
git clone $GIT_REPO_URL /home/ec2-user/santai_mvp

# Check if git clone succeeded
if [ $? -eq 0 ]; then
    echo "Git clone successful" >> /var/log/user-data.log
else
    echo "Git clone failed" >> /var/log/user-data.log
fi 

cd /home/ec2-user/santai_mvp 

sudo docker-compose -f docker-compose-{module}.yml up -d --build

# Optionally, log the output for debugging
echo "Setup complete!" >> /var/log/user-data.log
