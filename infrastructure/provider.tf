terraform {
  required_providers {
    kubernetes = {
      source  = "hashicorp/kubernetes"
      version = ">= 2.0.0"
    }
  }
  backend "local" {
    path = "terraform.tfstate"
  }
}

provider "kubernetes" {
  config_paths = [
    "../k8s/pod.yml",
    "../k8s/service.yml",
  ]
}
