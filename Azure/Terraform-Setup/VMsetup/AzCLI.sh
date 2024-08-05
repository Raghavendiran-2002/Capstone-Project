az vm create --resource-group rg_raghav --name raghav-vm --image "Canonical:0001-com-ubuntu-server-jammy:22_04-lts-gen2:latest" --admin-username "raghavendiran" --assign-identity --generate-ssh-keys --public-ip-sku Standard

public_ip=$(az vm show --show-details --resource-group rg_raghav --name raghav-vm --query publicIps --output tsv)

echo "ssh raghavendiran@$public_ip"