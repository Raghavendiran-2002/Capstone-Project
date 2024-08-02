provider "azurerm" {
  features {}
}

# Create a resource group
resource "azurerm_resource_group" "rg_raghav" {
  name     = "rg_raghav_azure"
  location = "East US"
  tags = {
    create-db = "true"
  }
}

# Create a virtual network
resource "azurerm_virtual_network" "vnet" {
  name                = "vnet-raghav"
  address_space       = ["10.0.0.0/16"]
  location            = azurerm_resource_group.rg_raghav.location
  resource_group_name = azurerm_resource_group.rg_raghav.name
}

# Create a subnet
resource "azurerm_subnet" "subnet" {
  name                 = "subnet-raghav"
  resource_group_name  = azurerm_resource_group.rg_raghav.name
  virtual_network_name = azurerm_virtual_network.vnet.name
  address_prefixes     = ["10.0.1.0/24"]
}

# Create a public IP address
resource "azurerm_public_ip" "public_ip" {
  name                = "public-ip-raghav"
  location            = azurerm_resource_group.rg_raghav.location
  resource_group_name = azurerm_resource_group.rg_raghav.name
  allocation_method   = "Dynamic"
  sku                 = "Standard"
}

# Create a network interface
resource "azurerm_network_interface" "nic" {
  name                = "nic-raghav"
  location            = azurerm_resource_group.rg_raghav.location
  resource_group_name = azurerm_resource_group.rg_raghav.name

  ip_configuration {
    name                          = "ipconfig1"
    subnet_id                     = azurerm_subnet.subnet.id
    private_ip_address_allocation = "Dynamic"
    public_ip_address_id          = azurerm_public_ip.public_ip.id
  }
}

# Create a virtual machine
resource "azurerm_linux_virtual_machine" "vm" {
  name                = "raghav-demo"
  resource_group_name = azurerm_resource_group.rg_raghav.name
  location            = azurerm_resource_group.rg_raghav.location
  size                = "Standard_DS1_v2"
  admin_username      = "Raghavendiran"

  network_interface_ids = [
    azurerm_network_interface.nic.id,
  ]

  os_disk {
    caching              = "ReadWrite"
    storage_account_type = "Standard_LRS"
  }

  source_image_reference {
    publisher = "Canonical"
    offer     = "0001-com-ubuntu-server-jammy"
    sku       = "22_04-lts-gen2"
    version   = "latest"
  }

  admin_ssh_key {
    username   = "Raghavendiran"
    public_key = file("~/.ssh/id_rsa.pub")
  }

  identity {
    type = "SystemAssigned"
  }
}

# Create an SQL server
resource "azurerm_sql_server" "sql_server_raghav" {
  name                         = "sql-server-raghav"
  resource_group_name          = azurerm_resource_group.rg_raghav.name
  location                     = azurerm_resource_group.rg_raghav.location
  version                      = "12.0"
  administrator_login          = "raghav"
  administrator_login_password = "pass@123"
}

# Create a firewall rule to allow all IP addresses (0.0.0.0)
resource "azurerm_sql_firewall_rule" "allow_all" {
  name                = "AllowAllIPs"
  resource_group_name = azurerm_resource_group.rg_raghav.name
  server_name         = azurerm_sql_server.sql_server_raghav.name
  start_ip_address    = "0.0.0.0"
  end_ip_address      = "0.0.0.0"
}

# Create a firewall rule to allow the VM's public IP address
resource "azurerm_sql_firewall_rule" "allow_vm_ip" {
  name                = "AllowVmPublicIp"
  resource_group_name = azurerm_resource_group.rg_raghav.name
  server_name         = azurerm_sql_server.sql_server_raghav.name
  start_ip_address    = azurerm_public_ip.public_ip.ip_address
  end_ip_address      = azurerm_public_ip.public_ip.ip_address
  depends_on          = [azurerm_linux_virtual_machine.vm]
}

# Create a SQL database with the AdventureWorksLT sample
resource "azurerm_sql_database" "raghav_db" {
  location            = azurerm_resource_group.rg_raghav.location
  name                = "raghav"
  resource_group_name = azurerm_resource_group.rg_raghav.name
  server_name         = azurerm_sql_server.sql_server_raghav.name
  edition             = "GeneralPurpose"
  zone_redundant      = true
}

# Output the public IP address of the VM
output "vm_public_ip" {
  value = azurerm_public_ip.public_ip.ip_address
}

# Output the ADO.NET connection string
output "ado_net_connection_string" {
  value = format("Server=tcp:%s.database.windows.net,1433;Initial Catalog=%s;Persist Security Info=False;User ID=%s;Password=%s;MultipleActiveResultSets=False;Encrypt=True;TrustServerCertificate=False;Connection Timeout=30;",
    azurerm_sql_server.sql_server_raghav.name,
    azurerm_sql_database.raghav_db.name,
    azurerm_sql_server.sql_server_raghav.administrator_login,
    "pass@123"  # Ideally, this should be managed securely, e.g., using Azure Key Vault
  )
}
