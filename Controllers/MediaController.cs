using FileManager.DTO;
using FileManager.Helpers;
using FileManager.Services.Media;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.WebUtilities;
using Microsoft.Net.Http.Headers;

namespace FileManager.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class MediaController : ControllerBase
    {
        private readonly IMediaService _service;

        public MediaController(IMediaService service) => _service = service;

        /// <summary>
        /// Consulta arquivo de mídia pelo nome.
        /// </summary>
        /// <param name="fileName">Nome do arquivo de dados</param>
        /// <param name="downloadName">Nome sugerido para download</param>
        /// <param name="disposition">attachment ou inline</param>
        /// <returns>Arquivo de Dados</returns>
        [HttpGet("{fileName}")]
        public ActionResult<FileManagetDto> ReadFile(string fileName, string downloadName, Dispositions disposition)
        {
            var result = _service.ReadMediaFile(fileName);

            if (disposition == Dispositions.Inline)
            {
                Response.Headers.Add("Content-Disposition", FileManagerHelper.GetContentDisposition(downloadName ?? result.FileName, "inline"));
                return PhysicalFile(result.FilePath, result.ContentType, true);
            }
            return PhysicalFile(result.FilePath, result.ContentType, disposition == Dispositions.Attachment ? downloadName ?? result.FileName : downloadName, true);
        }

        /// <summary>
        /// Consulta arquivo de mídia temporário pelo nome.
        /// </summary>
        /// <param name="fileName">Nome do arquivo de dados temporário</param>
        /// <param name="downloadName">Nome sugerido para download</param>
        /// <param name="disposition">attachment ou inline</param>
        /// <returns>Arquivo de Dados</returns>
        [HttpGet("Temp/{fileName}")]
        public ActionResult<FileManagetDto> ReadTempFile(string fileName, string downloadName, Dispositions disposition)
        {
            var result = _service.ReadTempFile(fileName);

            if (disposition == Dispositions.Inline)
            {
                Response.Headers.Add("Content-Disposition", FileManagerHelper.GetContentDisposition(downloadName ?? result.FileName, "inline"));
                return PhysicalFile(result.FilePath, result.ContentType, true);
            }
            return PhysicalFile(result.FilePath, result.ContentType, disposition == Dispositions.Attachment ? downloadName ?? result.FileName : downloadName, true);
        }

        /// <summary>
        /// Carrega arquivo de dados.
        /// </summary>
        /// <param name="file">Arquivo de Dados</param>
        /// <param name="cancellationToken">Token de cancelamento da requisição</param>
        /// <returns></returns>
        [HttpPost]
        public async Task<ActionResult<FileManagerResponseDto>> UploadMedia(IFormFile file, CancellationToken cancellationToken)
        {
            var result = await _service.UploadMediaFile(file, cancellationToken);
            return CreatedAtAction(nameof(ReadFile), new { fileName = result.FileName }, result);
        }

        /// <summary>
        /// Carrega arquivo de mídia temporário.
        /// </summary>
        /// <param name="file">Arquivo de Dados Temporario</param>
        /// <param name="cancellationToken">Token de cancelamento da requisição</param>
        /// <returns></returns>
        [HttpPost("Temp")]
        public async Task<ActionResult<FileManagerResponseDto>> UploadTempMedia(IFormFile file, CancellationToken cancellationToken)
        {
            var result = await _service.UploadTempFile(file, cancellationToken);
            return CreatedAtAction(nameof(ReadTempFile), new { fileName = result.FileName }, result);
        }

        /// <summary>
        /// Carrega grande arquivo de mídia.
        /// </summary>
        /// <returns></returns>
        [HttpPost("Large")]
        public async Task<ActionResult<FileManagerResponseDto>> UploadLargeMedia()
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

            var result = await _service.UploadLargeMediaFile(reader, section);
            return CreatedAtAction(nameof(ReadFile), new { fileName = result.FileName }, result);
        }

        /// <summary>
        /// Carrega grande arquivo de mídia temporário.
        /// </summary>
        /// <returns></returns>
        [HttpPost("Temp/Large")]
        public async Task<ActionResult<FileManagerResponseDto>> UploadLargeTempMedia()
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

        /// <summary>
        /// Remove arquivo de mídia.
        /// </summary>
        /// <param name="fileName">Nome do Arquivo de Dados</param>
        /// <param name="cancellationToken">Token de cancelamento da requisição</param>
        [HttpDelete("{fileName}")]
        public async Task<IActionResult> DeleteMedia(string fileName, CancellationToken cancellationToken)
        {
            await _service.DeleteMediaFile(fileName, cancellationToken);
            return NoContent();
        }
    }
}
