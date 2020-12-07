# provider "azurerm" {
#     # The "feature" block is required for AzureRM provider 2.x. 
#     # If you are using version 1.x, the "features" block is not allowed.
#     version = "~>2.0"
#     features {}
# }

provider "azurerm" {
  features {} 
  version = "~> 2.0" #version = “~> 1.44”  #version = "~> 1.34.0" #version = "~> 2.0"
  subscription_id = "${var.subscription_id}"
  tenant_id = "${var.tenant_id}"
}

resource "azurerm_resource_group" "rg" {
  name = "${var.resource_group_name}"
  location = "${var.resource_group_location}"
}

terraform {
  backend "azurerm" {
    resource_group_name   = "fieldtrial_dev_rg" #"${var.resource_group_name}"
    storage_account_name  = "blendnetterraformstate"
    container_name        = "tfstate"
    key                   = "blendnet.microsoft.tfstate"
  }
}