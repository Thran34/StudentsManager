variable "project_id" {
  type    = string
  default = "ferrous-destiny-423011-p0"
}

variable "zone" {
  type    = string
  default = "us-central1-a"
}

source "googlecompute" "example" {
  account_file = "creds.json"
  project_id   = var.project_id
  source_image = "ubuntu-minimal-2204-jammy-v20240430"
  zone         = var.zone
  communicator = "ssh"
  ssh_username = "ubuntu"
  image_name   = "app-image-{{timestamp}}"
  image_family = "app-image"
  disk_size    = 50
}

build {
  sources = [
    "source.googlecompute.example"
  ]

  provisioner "shell" {
    inline = [
      "sudo apt-get update",
      "sudo apt-get -y upgrade",
      "sudo apt-get -y install aptitude",
      "sudo aptitude -y install git",
      "sudo aptitude -y install dotnet-sdk-6.0",
      "cd ~/",
      "mkdir app",
      "cd app",
      "git clone https://github.com/Thran34/StudentsManager.git",
      "cd StudentsManager",
      "dotnet restore",
      "dotnet build --no-restore",
      "dotnet publish -c Release -o ~/app/StudentsManager/publish",
      "echo '[Unit]' | sudo tee /etc/systemd/system/studentsmanager.service",
      "echo 'Description=Students Manager .NET Application' | sudo tee -a /etc/systemd/system/studentsmanager.service",
      "echo '[Service]' | sudo tee -a /etc/systemd/system/studentsmanager.service",
      "echo 'WorkingDirectory=/home/ubuntu/app/StudentsManager/publish' | sudo tee -a /etc/systemd/system/studentsmanager.service",
      "echo 'ExecStart=/usr/bin/dotnet /home/ubuntu/app/StudentsManager/publish/StudentsManager.dll' | sudo tee -a /etc/systemd/system/studentsmanager.service",
      "echo 'Restart=always' | sudo tee -a /etc/systemd/system/studentsmanager.service",
      "echo 'RestartSec=10' | sudo tee -a /etc/systemd/system/studentsmanager.service",
      "echo 'SyslogIdentifier=students-manager' | sudo tee -a /etc/systemd/system/studentsmanager.service",
      "echo '[Install]' | sudo tee -a /etc/systemd/system/studentsmanager.service",
      "echo 'WantedBy=multi-user.target' | sudo tee -a /etc/systemd/system/studentsmanager.service",
      "sudo systemctl enable studentsmanager.service",
      "sudo systemctl start studentsmanager.service"
    ]
  }
}
