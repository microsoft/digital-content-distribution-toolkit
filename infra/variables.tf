variable "client_id" {}
variable "client_secret" {}

variable "resource_group_name" {
  default = "fieldtrial_dev_rg"
}
variable "resource_group_location" {
  default = "southeastasia"
}

variable "subscription_id" {
  default = "79b6781b-bc04-4e86-95d0-0e81a597feb5"
}

variable "tenant_id" {
  default = "72f988bf-86f1-41af-91ab-2d7cd011db47"
}

variable "cosmos_db_account_name" {
  default = "blendnetcrm"
}
variable "failover_location" {
  default = "eastasia"
}

variable "agent_count" {
    default = 3
}

variable "ssh_public_key" {
    default = "~/.ssh/id_rsa.pub"
}

variable "dns_prefix" {
    default = "blendnetdev"
}

variable cluster_name {
    default = "blendnetdev"
}


variable location {
    default = "southeastasia"
}

variable log_analytics_workspace_name {
    default = "BNLogAnalyticsWorkspaceName"
}

# refer https://azure.microsoft.com/global-infrastructure/services/?products=monitor for log analytics available regions
variable log_analytics_workspace_location {
    default = "eastasia"
}

# refer https://azure.microsoft.com/pricing/details/monitor/ for log analytics pricing 
variable log_analytics_workspace_sku {
    default = "PerGB2018"
}