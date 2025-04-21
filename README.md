<!--
---
name: Azure Functions C# Event Grid Blob Trigger using Azure Developer CLI
description: This template repository contains an Azure Functions reference sample using the Blob trigger with Event Grid source type, written in C# (isolated process mode) and deployed to Azure using the Azure Developer CLI (azd). The sample uses managed identity and a virtual network to make sure deployment is secure by default.
page_type: sample
products:
- azure
- azure-functions
- azure-blob-storage
- azure-virtual-network
- entra-id
urlFragment: functions-quickstart-dotnet-azd-eventgrid-blob
languages:
- csharp
- bicep
- azdeveloper
---
-->

# Azure Functions C# Event Grid Blob Trigger using Azure Developer CLI

This template repository contains an Azure Functions reference sample using the Blob trigger with Event Grid source type, written in C# (isolated process mode) and deployed to Azure using the Azure Developer CLI (`azd`). When deployed to Azure the sample uses managed identity and a virtual network to make sure deployment is secure by default. You can opt out of a VNet being used in the sample by setting `SKIP_VNET` to true in the AZD parameters.

## Prerequisites

+ [.NET 8 SDK](https://dotnet.microsoft.com/download/dotnet/8.0)
+ [Azure Functions Core Tools](https://learn.microsoft.com/azure/azure-functions/functions-run-local?pivots=programming-language-csharp#install-the-azure-functions-core-tools)
+ To use Visual Studio to run and debug locally:
  + [Visual Studio 2022](https://visualstudio.microsoft.com/vs/).
  + Make sure to select the **Azure development** workload during installation.
+ To use Visual Studio Code to run and debug locally:
  + [Visual Studio Code](https://code.visualstudio.com/)
  + [Azure Storage extension](https://marketplace.visualstudio.com/items?itemName=ms-azuretools.vscode-azurestorage)
  + [Azure Functions extension](https://marketplace.visualstudio.com/items?itemName=ms-azuretools.vscode-azurefunctions)
  + [REST Client](https://marketplace.visualstudio.com/items/?itemName=humao.rest-client)
+ [Azurite](https://learn.microsoft.com/en-us/azure/storage/common/storage-use-azurite) to emulate Azure Storage services when running locally

Optional for uploading blobs:

+ [Azure CLI](https://learn.microsoft.com/en-us/cli/azure/install-azure-cli) or
+ [Azure Storage Explorer](https://azure.microsoft.com/en-us/products/storage/storage-explorer/#Download-4)

## Initialize the local project

To initialize a project from this `azd` template, clone the GitHub template repository locally using the `git clone` command:

    ```bash
    git clone https://github.com/Azure-Samples/functions-quickstart-dotnet-azd-eventgrid-blob.git
    cd functions-quickstart-dotnet-azd-eventgrid-blob
    ```

    You can also clone the repository from your own fork in GitHub.

# Start and prepare the local storage emulator

Azure Functions uses Azurite to emulate Azure Storage services when running locally. If you haven't done so, [install Azurite](https://learn.microsoft.com/en-us/azure/storage/common/storage-use-azurite#install-azurite).

Create two containers in the local storage emulator called `processed-pdf` and `unprocessed-pdf`. Follow these steps:

1. Ensure Azurite is running. For more details see [Run Azurite](https://learn.microsoft.com/en-us/azure/storage/common/storage-use-azurite#run-azurite)

2. Use Azure Storage Explorer, Azure CLI, or the VS Code Storage Extension to create the containers.

   **Using Azure CLI:**
   Run the following commands to create the containers:

    ```bash
    az storage container create --name processed-pdf --connection-string UseDevelopmentStorage=true
    az storage container create --name unprocessed-pdf --connection-string UseDevelopmentStorage=true
    ```

   **Using Azure Storage Explorer:**
   + Install [Azure Storage Explorer](https://azure.microsoft.com/en-us/products/storage/storage-explorer/#Download-4)
   + Open Azure Storage Explorer.
   + Connect to the local emulator by selecting `Attach to a local emulator.`
   + Navigate to the `Blob Containers` section.
   + Right-click and select `Create Blob Container.`
   + Name the containers `processed-pdf` and `unprocessed-pdf`.

   **Using VS Code Storage Extension:**
   + Install the VS Code [Azure Storage extension](https://marketplace.visualstudio.com/items?itemName=ms-azuretools.vscode-azurestorage)
   + Ensure Azurite is running
   + Click on the Azure extension icon in VS Code
   + Under `Workspace`, expand `Local Emulator`
   + Right click on `Blob Containers` and select `Create Blob Container`

3. Upload the PDF files from the `data` folder to the `unprocessed-pdf` container.

    **Using Azure CLI:**
    Run the following command to upload the files:

    ```bash
    az storage blob upload-batch --source ./data --destination unprocessed-pdf --connection-string UseDevelopmentStorage=true
    ```

    **Using Azure Storage Explorer:**
    + Open Azure Storage Explorer.
    + Navigate to the `unprocessed-pdf` container.
    + Click on "Upload" and select "Upload Folder" or "Upload Files."
    + Choose the `data` folder or the specific PDF files to upload.
    + Confirm the upload to the `unprocessed-pdf` container.

   **Using VS Code Storage Extension:**
   + Install the VS Code [Azure Storage extension](https://marketplace.visualstudio.com/items?itemName=ms-azuretools.vscode-azurestorage)
   + Ensure Azurite is running
   + Click on the Azure extension icon in VS Code
   + Under `Workspace`, expand `Local Emulator`, expand `Blob Containers`
   + Right click on `unprocessed-pdf` and select `Open in Explorer`
   + Copy and paste all the pdf files from the `data` folder to it

## Run your app

  **Using the terminal**

+ From the `http` folder, run this command to start the Functions host locally:

    ```bash
    func start
    ```

  **Using Visual Studio Code**

+ Open the `src` app folder in a new terminal.
+ Run the `code .` code command to open the project in Visual Studio Code.
+ In the command palette (F1), type `Azurite: Start`, which enables debugging without warnings.

  **Using Visual Studio**

1. Open the `src.csproj` project file in Visual Studio.
1. Press **Run/F5** to run in the debugger. Make a note of the `localhost` URL endpoints, including the port, which might not be `7071`.

## Trigger the function

Now that the storage emulator is running, has files on the `unprocessed-pdf` container, and our app is running, we can execute the `ProcessBlobUpload` function to simulate a new blob event.

+ If you are using VS Code or Visual Studio, you can also open the [`test.http`](./test.http) project file, update the port on the `localhost` URL (if needed), and then click on Send Request to call the locally running `ProcessBlobUpload` function.

+ Otherwise, use an HTTP client tool for making HTTP calls and send a **POST** to `http://localhost:7071/runtime/webhooks/blobs?functionName=Host.Functions.ProcessBlobUpload` (update the port if needed), headers `content-type: application/json` and `aeg-event-type: Notification`, with the following body:

    ```json
    {
      "source": "/subscriptions/{subscription-id}/resourceGroups/Storage/providers/Microsoft.Storage/storageAccounts/my-storage-account",
      "subject": "/blobServices/default/containers/unprocessed-pdf/blobs/Benefit_Options.pdf",
      "type": "Microsoft.Storage.BlobCreated",
      "time": "2021-08-16T02:51:26.4248221Z",
      "id": "beb21a5e-401e-002b-3749-928517060431",
      "data": {
        "api": "PutBlob",
        "clientRequestId": "89bc72c2-5dfe-4d9f-9706-43612c1bd01b",
        "requestId": "beb21a5e-401e-002b-3749-928517000000",
        "eTag": "0x8D96060BDA19D9D",
        "contentType": "application/pdf",
        "contentLength": 142977,
        "blobType": "BlockBlob",
        "accessTier": "Hot",
        "url": "https://mystorage.blob.core.windows.net/unprocessed-pdf/Benefit_Options.pdf",
        "sequencer": "0000000000000000000000000000B00D0000000005b058d3",
        "storageDiagnostics": {
          "batchId": "23f68872-a006-0065-0049-9240f2000000"
        }
      },
      "specversion": "1.0"
    }
    ```
+ The above will trigger the function to process the `Benefit_Options.pdf` file. You can update the file name in the JSON to process other PDF files.

## Source Code

The function code for the `ProcessBlobUpload` endpoint is defined in [`ProcessBlobUpload.cs`](./src/ProcessBlobUpload.cs). The `Function` attribute applied to the async `Run` method sets the name of the function endpoint.

    ```csharp
    [Function(nameof(ProcessBlobUpload))]
    public async Task Run([BlobTrigger("unprocessed-pdf/{name}", Source = BlobTriggerSource.EventGrid, Connection = "PDFProcessorSTORAGE")] Stream stream, string name)
    {
        using var blobStreamReader = new StreamReader(stream);
        var fileSize = stream.Length;
        _logger.LogInformation($"C# Blob Trigger (using Event Grid) processed blob\n Name: {name} \n Size: {fileSize} bytes");
    
        // Simple demonstration of an async operation - copy to a processed container
        await CopyToProcessedContainerAsync(stream, name);
        
        _logger.LogInformation($"PDF processing complete for {name}");
    }
    ```

The `CopyToProcessedContainerAsync` method that id calls uses the dependency injected `_copyContainerClient` blob client instance to upload the stream to the destination blob container.

## Deploy to Azure

If required you can opt-out of a VNet being used in the sample. To do so, use `azd env` to configure `SKIP_VNET` to `true` before running `azd up`:

```bash
azd env set SKIP_VNET true
azd up
```

Then run this command to provision the function app and other required Azure Azure resources, and deploy your code:

```bash
azd up
```

You're prompted to supply these required deployment parameters:

| Parameter | Description |
| ---- | ---- |
| _Environment name_ | An environment that's used to maintain a unique deployment context for your app. You won't be prompted if you created the local project using `azd init`.|
| _Azure subscription_ | Subscription in which your resources are created.|
| _Azure location_ | Azure region in which to create the resource group that contains the new Azure resources. Only regions that currently support the Flex Consumption plan are shown.|

After publish completes successfully, the new resource group will have a storage account and the `processed-pdf` and `unprocessed-pdf` containers. Upload PDF files from the data folder to the `unprocessed-pdf` folder and then check `processed-pdf` for the file.

## Redeploy your code

You can run the `azd up` command as many times as you need to both provision your Azure resources and deploy code updates to your function app.

>[!NOTE]
>Deployed code files are always overwritten by the latest deployment package.

## Clean up resources

When you're done working with your function app and related resources, you can use this command to delete the function app and its related resources from Azure and avoid incurring any further costs:

```bash
azd down
```
