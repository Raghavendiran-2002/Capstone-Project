

az group create --name rg_raghav --location eastus

az storage account create --name storecert -g rg_raghav --location eastus --sku Standard_LRS --allow-blob-public-access true

az functionapp create -g rg_raghav --consumption-plan-location eastus --runtime dotnet --runtime-version 6 --functions-version 4 --name certFunctionRaghav --storage-account storecert

npm install -g azure-functions-core-tools@4 --unsafe-perm true

func init --worker-runtime dotnet

func new --template "HTTP trigger" --name GenerateCertificate

dotnet add package PdfSharp

dotnet add package Azure.Storage.Blobs

func azure functionapp publish certFunctionRaghav

az group delete --name rg_raghav

Note : Enable Cors in App Function, Allow public access in Azure Storage

(Search in left) App Function : Cors -> Add localIp -> http://127.0.0.1:5500 -> Save Storage : Configuration -> Allow Blobs Anonymous Access -> Enable Date Storage - > Select Certificates -> change access level - > public access

