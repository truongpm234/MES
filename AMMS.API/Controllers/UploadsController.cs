using AMMS.Application.Interfaces;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using System.Text.Json;

namespace AMMS.API.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class UploadsController : ControllerBase
    {
        private readonly IUploadFileService _uploadService;
        private readonly IRequestService _requestRepo;
        public UploadsController(IUploadFileService uploadService)
        {
            _uploadService = uploadService;
        }

        [HttpPost("upload")]
        public async Task<IActionResult> Upload(IFormFile file)
        {
            if (file == null || file.Length == 0)
                return BadRequest("File is empty");

            using var stream = file.OpenReadStream();

            var url = await _uploadService.UploadAsync(
                stream,
                file.FileName,
                file.ContentType,
                "uploads");

            return Ok(new { url });
        }
    }

}
