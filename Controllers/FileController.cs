using AzureBlobStorageApiTest.Interface;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;

namespace AzureBlobStorageApiTest.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class FileController : ControllerBase
    {
        private readonly IBlobStorage _blobStorage;

        public FileController(IBlobStorage blobStorage)
        {
            _blobStorage = blobStorage;
        }

        [HttpPost("upload")]
        public async Task<IActionResult> UploadAsync([FromForm] IFormFile file)
        {
            if (file == null || file.Length == 0)
                return BadRequest("Invalid file.");

            try
            {
                using (var stream = file.OpenReadStream())
                {
                    await _blobStorage.UploadAsnyc(stream, file.FileName, "your-containerName");
                }
                return Ok(new { success = true, message = "File uploaded successfully." });
            }
            catch (Exception ex)
            {
                return StatusCode(500, new { success = false, message = $"An unexpected error was encountered: {ex.Message}" });
            }
        }

        [HttpGet("download")]
        public async Task<IActionResult> DownloadAsync(string fileName, string containerName)
        {
            if (string.IsNullOrWhiteSpace(fileName) || string.IsNullOrWhiteSpace(containerName))
                return BadRequest("Invalid file name or container name.");

            try
            {
                var stream = await _blobStorage.DownloadAsync(fileName, containerName);
                if (stream == null)
                    return NotFound("File not found.");

                return File(stream, "application/octet-stream", fileName);
            }
            catch (Exception ex)
            {
                return StatusCode(500, new { success = false, message = $"An unexpected error was encountered: {ex.Message}" });
            }
        }

        [HttpDelete("delete")]
        public async Task<IActionResult> DeleteAsync(string fileName, string containerName)
        {
            if (string.IsNullOrWhiteSpace(fileName) || string.IsNullOrWhiteSpace(containerName))
                return BadRequest("Invalid file name or container name.");

            try
            {
                await _blobStorage.DeleteAsync(fileName, containerName);
                return Ok(new { success = true, message = "File deleted successfully." });
            }
            catch (Exception ex)
            {
                return StatusCode(500, new { success = false, message = $"An unexpected error was encountered: {ex.Message}" });
            }
        }
    }
}
