using FileManager.DTO;
using FileManager.Services.Data;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.WebUtilities;
using Microsoft.Net.Http.Headers;

namespace FileManager.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class DataController : ControllerBase
    {
        private readonly IDataService _service;

        public DataController(IDataService service) => _service = service;

        [HttpGet("{fileName}")]
        public ActionResult<FileManagetDto> ReadFile(string fileName)
        {
            var result = _service.ReadFile(fileName);
            return PhysicalFile(result.FilePath, result.ContentType);
        }

        [HttpGet("Temp/{fileName}")]
        public ActionResult<FileManagetDto> ReadTempFile(string fileName)
        {
            var result = _service.ReadTempFile(fileName);
            return PhysicalFile(result.FilePath, result.ContentType);
        }

        [HttpPost]
        public async Task<IActionResult> UploadData(IFormFile file, CancellationToken cancellationToken)
        {
            var result = await _service.UploadFile(file, cancellationToken);
            return CreatedAtAction(nameof(ReadFile), new { fileName = result.FileName }, result);
        }

        [HttpPost("Temp")]
        public async Task<IActionResult> UploadTempData(IFormFile file, CancellationToken cancellationToken)
        {
            var result = await _service.UploadTempFile(file, cancellationToken);
            return CreatedAtAction(nameof(ReadTempFile), new { fileName = result.FileName }, result);
        }

        [HttpPost("Large")]
        public async Task<IActionResult> UploadLargeData()
        {
            var request = HttpContext.Request;

            if (!request.HasFormContentType ||
                !MediaTypeHeaderValue.TryParse(request.ContentType, out var mediaTypeHeader) ||
                string.IsNullOrEmpty(mediaTypeHeader.Boundary.Value))
            {
                return new UnsupportedMediaTypeResult();
            }

            var reader = new MultipartReader(mediaTypeHeader.Boundary.Value, Request.Body);
            var section = await reader.ReadNextSectionAsync();

            var result = await _service.UploadLargeFile(reader, section);
            return CreatedAtAction(nameof(ReadFile), new { fileName = result.FileName }, result);
        }


        [HttpPost("Temp/Large")]
        public async Task<IActionResult> UploadLargeTempData()
        {
            var request = HttpContext.Request;

            if (!request.HasFormContentType ||
                !MediaTypeHeaderValue.TryParse(request.ContentType, out var mediaTypeHeader) ||
                string.IsNullOrEmpty(mediaTypeHeader.Boundary.Value))
            {
                return new UnsupportedMediaTypeResult();
            }

            var reader = new MultipartReader(mediaTypeHeader.Boundary.Value, Request.Body);
            var section = await reader.ReadNextSectionAsync();

            var result = await _service.UploadLargeTempFile(reader, section);
            return CreatedAtAction(nameof(ReadTempFile), new { fileName = result.FileName }, result);
        }

        [HttpDelete("{fileName}")]
        public IActionResult DeleteData(string fileName)
        {
            _service.DeleteFile(fileName);
            return NoContent();
        }
    }
}
