/*
resource "kubernetes_deployment" "helloworld" {
  metadata {
    name = "helloworld"
  }
  spec {
    selector {
      match_labels = {
        "app" = "helloworld"
      }
    }
    replicas = 2
    template {
      metadata {
        labels = {
          "app" = "helloworld"
        }
      }
      spec {
        container {
          name  = "helloworld"
          image = "karthequian/helloworld:latest"
          port {
            container_port = 80
          }
        }
      }
    }

  }
}

resource "kubernetes_pod" "mcbrowser" {
  metadata {
    name = "mcbrowser"
    labels = {
      "app"       = "mcbrowser",
      "component" = "mcbrowser"
    }
  }

  spec {
    container {
      name  = "mcbrowser"
      image = "mcbrowser"
      port {
        container_port = 80
      }

      resources {
        limits = {
          cpu    = "1"
          memory = "512Mi"
        }
        requests = {
          cpu = "0.5"
        }
      }
    }
  }
}
*/
