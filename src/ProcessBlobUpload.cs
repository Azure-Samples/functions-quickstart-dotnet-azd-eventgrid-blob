using System;
using System.IO;
using System.Threading.Tasks;
using Microsoft.Azure.Functions.Worker;
using Microsoft.Extensions.Logging;
using Azure.Storage.Blobs;
using Microsoft.Extensions.Azure;

namespace Company.Function
{
    public class ProcessBlobUpload
    {
        private readonly ILogger<ProcessBlobUpload> _logger;
        private readonly BlobContainerClient _copyContainerClient;
        public ProcessBlobUpload(ILogger<ProcessBlobUpload> logger, IAzureClientFactory<BlobServiceClient> blobClientFactory)
        {
            _logger = logger;
            // Create a BlobServiceClient using the factory and get the container client for processed PDFs
            _copyContainerClient = blobClientFactory.CreateClient("ProcessBlobUpload").GetBlobContainerClient("processed-pdf");
            _copyContainerClient.CreateIfNotExists();
        }

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

        // Simple async method to demonstrate uploading the processed PDF
        private async Task CopyToProcessedContainerAsync(Stream stream, string blobName)
        {
            _logger.LogInformation($"Starting async copy operation for {blobName}");
            
            // Get a reference to the blob
            var blobClient = _copyContainerClient.GetBlobClient(blobName);
            
            // Reset stream position to beginning before copying
            stream.Position = 0;
            
            // Upload the blob
            await blobClient.UploadAsync(stream, overwrite: true);
            
            _logger.LogInformation($"Successfully copied {blobName} to processed-pdf container");
        }
    }
}
