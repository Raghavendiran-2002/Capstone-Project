provider "azurerm" {
  features {}
}


resource "azurerm_public_ip" "raghav_ip" {
  name                = "raghavPublicIP"
  location            = "eastus"
  resource_group_name = "rg_raghav"
  allocation_method   = "Static"
  sku                 = "Standard"
}

resource "azurerm_network_security_group" "raghav_nsg" {
  name                = "raghavNSG"
  location            = "eastus"
  resource_group_name = "rg_raghav"

  security_rule {
    name                       = "Allow_SSH"
    priority                   = 1001
    direction                  = "Inbound"
    access                     = "Allow"
    protocol                   = "Tcp"
    source_port_range          = "*"
    destination_port_range     = "22"
    source_address_prefix      = "*"
    destination_address_prefix = "*"
  }
}

resource "azurerm_virtual_network" "raghav_vnet" {
  name                = "raghavVNET"
  address_space       = ["10.0.0.0/16"]
  location            = "eastus"
  resource_group_name = "rg_raghav"
}

resource "azurerm_subnet" "raghav_subnet" {
  name                 = "raghavSubnet"
  resource_group_name  = "rg_raghav"
  virtual_network_name = azurerm_virtual_network.raghav_vnet.name
  address_prefixes     = ["10.0.1.0/24"]
}

resource "azurerm_network_interface" "raghav_nic" {
  name                = "raghavNIC"
  location            = "eastus"
  resource_group_name = "rg_raghav"

  ip_configuration {
    name                          = "internal"
    subnet_id                     = azurerm_subnet.raghav_subnet.id
    private_ip_address_allocation = "Dynamic"
    public_ip_address_id          = azurerm_public_ip.raghav_ip.id
  }

}

resource "azurerm_linux_virtual_machine" "raghav_vm" {
  name                = "raghav-vm"
  resource_group_name = "rg_raghav"
  location            = "eastus"
  size                = "Standard_DS1_v2"

  admin_username = "Raghavendiran"
  network_interface_ids = [
    azurerm_network_interface.raghav_nic.id,
  ]

  admin_ssh_key {
    username   = "Raghavendiran"
    public_key = file("~/.ssh/id_rsa.pub")
  }

  identity {
    type = "SystemAssigned"
  }

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

  computer_name = "raghav-vm"
}

output "public_ip_address" {
  value = azurerm_public_ip.raghav_ip.ip_address
}

output "ssh_connection_string" {
  value = "ssh Raghavendiran@${azurerm_public_ip.raghav_ip.ip_address}"
}
