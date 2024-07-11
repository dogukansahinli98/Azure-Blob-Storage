using Azure.Storage.Blobs.Models;
using Azure.Storage.Blobs.Specialized;
using Azure.Storage.Blobs;
using Azure;
using AzureBlobStorageApiTest.Interface;
using System.Text;
using Microsoft.WindowsAzure.Storage.Blob;

namespace AzureBlobStorageApiTest.BlobStorage
{
    public class BlobStorage : IBlobStorage
    {
        private readonly BlobServiceClient _blobServiceClient;
        private BlobContainerClient _blobContainerClient;
        public BlobStorage()
        {
            string blobSasUrl = "your-url";
            string token = "your-token";


            // Creating a BlobClient object
            _blobServiceClient = new BlobServiceClient(new Uri(blobSasUrl + "?" + token));
        }
        public async Task DeleteAsync(string fileName, string containerName)
        {
            _blobContainerClient = _blobServiceClient.GetBlobContainerClient(containerName);
            BlobClient blobClient = _blobContainerClient.GetBlobClient(fileName);
            await blobClient.DeleteAsync();
        }
        public async Task<Stream> DownloadAsync(string fileName, string containerName)
        {
            _blobContainerClient = _blobServiceClient.GetBlobContainerClient(containerName);
            BlobClient blobClient = _blobContainerClient.GetBlobClient(fileName);
            Response<BlobDownloadInfo> response = await blobClient.DownloadAsync();
            return response.Value.Content;

        }
        public async Task UploadAsnyc(Stream fileStream, string name, string containerName)
        {


            try
            {
                //slow upload
                //_blobContainerClient = _blobServiceClient.GetBlobContainerClient(containerName);
                //BlobClient blobClient = _blobContainerClient.GetBlobClient(name);
                //await blobClient.UploadAsync(fileStream);


                BlobContainerClient containerClient = _blobServiceClient.GetBlobContainerClient(containerName);
                BlockBlobClient blockBlobClient = containerClient.GetBlockBlobClient(name);

                int chunkSize = 4 * 1024 * 1024; // 4 MB
                long fileSize = fileStream.Length;
                List<string> blockIds = new List<string>();
                long currentPosition = 0;

                while (currentPosition < fileSize)
                {
                    int bytesToRead = (int)Math.Min(chunkSize, fileSize - currentPosition);
                    byte[] buffer = new byte[bytesToRead];
                    await fileStream.ReadAsync(buffer, 0, bytesToRead);

                    string blockId = Convert.ToBase64String(Encoding.UTF8.GetBytes(Guid.NewGuid().ToString()));
                    blockIds.Add(blockId);

                    using (var stream = new MemoryStream(buffer, writable: false))
                    {
                        await blockBlobClient.StageBlockAsync(blockId, stream);
                    }

                    currentPosition += bytesToRead;
                }

                await blockBlobClient.CommitBlockListAsync(blockIds);


            }
            catch (Exception e)
            {

                Console.WriteLine(e.Message);
            }

        }
    }
}
