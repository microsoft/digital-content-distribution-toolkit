# terraform {
#   backend "azurerm" {
#     resource_group_name   = var.resource_group_name
#     storage_account_name  = "blendnetterraformstate"
#     container_name        = "tfstate"
#     key                   = "blendnet.microsoft.tfstate"
#   }
# }