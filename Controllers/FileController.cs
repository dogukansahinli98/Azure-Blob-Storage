using AzureBlobStorageApiTest.Interface;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;

namespace AzureBlobStorageApiTest.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class FileController : ControllerBase
    {
        readonly IBlobStorage _blobStorage;
        public FileController(IBlobStorage blobStorage)
        {
            _blobStorage = blobStorage;
        }
        [HttpGet("[action]")]
        public async Task<IActionResult> UploadAsync(string fileName)
        {
            try
            {
                FileStream fileStream = new FileStream(fileName, FileMode.OpenOrCreate);
                await _blobStorage.UploadAsnyc(fileStream, Path.GetFileName(fileName), "your-containerName");
                return Ok(true);
            }
            catch
            {
                return Ok("An unexpected error was encountered.");
            }
        }
        [HttpGet("[action]")]
        public async Task<IActionResult> DownloadAsync(string fileName, string containerName)
        {
            try
            {
                Stream stream = await _blobStorage.DownloadAsync(fileName, containerName);
                return File(stream, "application/octet-stream", fileName);
            }
            catch
            {
                return Ok("An unexpected error was encountered.");
            }
        }
        [HttpGet("[action]")]
        public async Task<IActionResult> DeleteAsync(string fileName, string containerName)
        {
            try
            {
                await _blobStorage.DeleteAsync(fileName, containerName);
                return Ok(true);
            }
            catch
            {
                return Ok(false);
            }
        }
       
    }
}
