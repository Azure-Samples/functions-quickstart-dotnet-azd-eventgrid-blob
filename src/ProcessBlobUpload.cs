using Microsoft.Azure.Functions.Worker;
using Microsoft.Extensions.Logging;
using Azure.Storage.Blobs;

namespace Company.Function
{
    public class ProcessBlobUpload
    {
        [Function(nameof(ProcessBlobUpload))]
        public async Task Run([BlobTrigger("unprocessed-pdf/{name}", Source = BlobTriggerSource.EventGrid, Connection = "PDFProcessorSTORAGE")] Stream stream,
        [BlobInput("processed-pdf", Connection = "PDFProcessorSTORAGE")] BlobContainerClient client,
         string name, FunctionContext context, CancellationToken cancellationToken)
        {
            //We are using a stream to show how to process the blob data as a stream. To to move a blob to another container you could also bind to BlobClient directly and copy the blob https://learn.microsoft.com/en-us/azure/storage/blobs/storage-blob-copy-async-dotnet
            var fileSize = stream.Length;
            var logger = context.GetLogger(nameof(ProcessBlobUpload));
            logger.LogInformation($"C# Blob Trigger (using Event Grid) processed blob\n Name: {name} \n Size: {fileSize} bytes");

            var newName = $"processed-{name}";

            stream.Position = 0; // Reset position after accessing Length property
            // Copy the blob to the processed container with a new name
            await client.UploadBlobAsync(newName, stream, cancellationToken);
            logger.LogInformation($"PDF processing complete for {name}. Blob copied to processed container with new name {newName}.");
        }
    }
}
