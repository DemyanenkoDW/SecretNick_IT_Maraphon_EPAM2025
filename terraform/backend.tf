terraform {
  backend "s3" {
    bucket       = "santa-prod-demyanenko"
    key          = "terraform/terraform.tfstate"
    region       = "eu-central-1"
    use_lockfile = true
    encrypt      = true
  }
}
