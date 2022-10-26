# Learn Terraform - Provision AKS Cluster

This repo is a companion repo to the [Provision an AKS Cluster learn guide](https://learn.hashicorp.com/terraform/kubernetes/provision-aks-cluster), containing Terraform configuration files to provision an AKS cluster on Azure.


az ad sp create-for-rbac --skip-assignment

{
  "appId": "38480402-5471-4e18-b487-3aeea38eea38",
  "displayName": "azure-cli-2022-10-25-05-35-49",
  "password": "WO58Q~eLqrevUR5DC6A5G1SksN3mBlumoLq.kbYo",
  "tenant": "82ae2f14-5dad-48d4-8198-2b9882f14596"
}


kubernetes_cluster_name = "sought-manatee-aks"
resource_group_name = "sought-manatee-rg"


az aks get-credentials --resource-group sought-manatee-rg --name sought-manatee-aks