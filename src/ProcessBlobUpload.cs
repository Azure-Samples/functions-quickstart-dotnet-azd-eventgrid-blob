using Microsoft.Azure.Functions.Worker;
using Microsoft.Extensions.Logging;
using Azure.Storage.Blobs;

namespace Company.Function
{
    public class ProcessBlobUpload
    {
        [Function(nameof(ProcessBlobUpload))]
        public async Task Run([BlobTrigger("unprocessed-pdf/{name}", Source = BlobTriggerSource.EventGrid, Connection = "PDFProcessorSTORAGE")] BlobClient sourceBlobClient,
        [BlobInput("processed-pdf", Connection = "PDFProcessorSTORAGE")] BlobContainerClient blobContainerClient,
         string name, FunctionContext context, CancellationToken cancellationToken)
        {
            var inputBlobProperties = await sourceBlobClient.GetPropertiesAsync(cancellationToken: cancellationToken);
            var fileSize = inputBlobProperties.Value.ContentLength;

            var logger = context.GetLogger(nameof(ProcessBlobUpload));
            logger.LogInformation($"C# Blob Trigger (using Event Grid) processed blob\n Name: {name} \n Size: {fileSize} bytes");

            // Copy the blob to the processed container with a new name
            string newBlobName = $"processed-{name}";
            if (await blobContainerClient.GetBlobClient(newBlobName).ExistsAsync(cancellationToken: cancellationToken))
            {
                logger.LogInformation($"Blob {newBlobName} already exists in the processed container. Skipping upload.");
                return;
            }

            // Here you can add any processing logic for the input blob before uploading it to the processed container.

            //Uploading the blob to the processed container using streams. You could add processing of the input stream logic here if needed.
            await blobContainerClient.UploadBlobAsync(newBlobName, await sourceBlobClient.OpenReadAsync(cancellationToken: cancellationToken), cancellationToken);
            logger.LogInformation($"PDF processing complete for {name}. Blob copied to processed container with new name {newBlobName}.");
        }
    }
}
