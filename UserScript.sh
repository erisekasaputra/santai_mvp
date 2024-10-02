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
sudo usermod -aG docker ec2-user

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
GIT_REPO_URL="https://erisekasaputra282000@gmail.com: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@github.com/erisekasaputra/santai_mvp.git"
git clone $GIT_REPO_URL /home/ec2-user/santai_mvp

# Navigate to the cloned repository directory
cd /home/ec2-user/santai_mvp

# Get EC2 instance metadata to retrieve instance identity (specifically instance tags or name)
INSTANCE_ID=$(curl -s http://169.254.169.254/latest/meta-data/instance-id)
INSTANCE_NAME=$(aws ec2 describe-instances --instance-ids $INSTANCE_ID --query "Reservations[*].Instances[*].Tags[?Key=='Name'].Value" --output text --region us-east-1)

# Convert instance name to lowercase for case-insensitive comparison
INSTANCE_NAME_LOWER=$(echo "$INSTANCE_NAME" | tr '[:upper:]' '[:lower:]')

# Check the instance name and run the appropriate docker-compose file
if [[ "$INSTANCE_NAME_LOWER" == *"account"* ]]; then 
    docker-compose -f docker-compose-account.yml up -d --build
elif [[ "$INSTANCE_NAME_LOWER" == *"catalog"* ]]; then 
    docker-compose -f docker-compose-catalog.yml up -d --build
elif [[ "$INSTANCE_NAME_LOWER" == *"filehub"* ]]; then 
    docker-compose -f docker-compose-filehub.yml up -d --build 
elif [[ "$INSTANCE_NAME_LOWER" == *"identity"* ]]; then 
    docker-compose -f docker-compose-identity.yml up -d --build
elif [[ "$INSTANCE_NAME_LOWER" == *"master"* ]]; then 
    docker-compose -f docker-compose-master.yml up -d --build
elif [[ "$INSTANCE_NAME_LOWER" == *"notification"* ]]; then 
    docker-compose -f docker-compose-notification.yml up -d --build
elif [[ "$INSTANCE_NAME_LOWER" == *"order"* ]]; then 
    docker-compose -f docker-compose-ordering.yml up -d --build
fi

# Optionally, log the output for debugging
echo "Setup complete!" >> /var/log/user-data.log 