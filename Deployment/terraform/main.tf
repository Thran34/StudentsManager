provider "google" {
  credentials = file("creds.json")
  project     = "ferrous-destiny-423011-p0"
  region      = "us-central1"
}

data "google_compute_image" "latest_app" {
  family = "app-image"
  most_recent = true
}

resource "google_compute_instance" "vm_instance" {
  name         = "net-app-vm"
  machine_type = "e2-medium"
  zone         = "us-central1-a"

  boot_disk {
    initialize_params {
      image = data.google_compute_image.latest_app.self_link
    }
  }

  network_interface {
    network = "default"
    access_config {}
  }
}

resource "google_compute_firewall" "default" {
  name    = "allow-tcp-80"
  network = "default"

  allow {
    protocol = "tcp"
    ports    = ["80", "443"]
  }

  source_ranges = ["0.0.0.0/0"]
}
