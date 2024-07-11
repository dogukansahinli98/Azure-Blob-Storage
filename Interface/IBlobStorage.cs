namespace AzureBlobStorageApiTest.Interface
{
    public interface IBlobStorage
    {
        Task UploadAsnyc(Stream fileStream, string name, string containerName);
        Task<Stream> DownloadAsync(string fileName, string containerName);
        Task DeleteAsync(string fileName, string containerName);
    }
}
