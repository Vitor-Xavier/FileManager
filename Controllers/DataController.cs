using FileManager.DTO;
using FileManager.Helpers;
using FileManager.Services.Data;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.WebUtilities;
using Microsoft.Net.Http.Headers;
using FileAccess = FileManager.Models.FileAccess;

namespace FileManager.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class DataController : ControllerBase
    {
        private readonly IDataService _service;

        public DataController(IDataService service) => _service = service;

        /// <summary>
        /// Consulta arquivo de dados pelo nome.
        /// </summary>
        /// <param name="fileName">Nome do arquivo de dados</param>
        /// <param name="downloadName">Nome sugerido para download</param>
        /// <param name="disposition">attachment ou inline</param>
        /// <returns>Arquivo de Dados</returns>
        [HttpGet("{fileName}")]
        public async Task<ActionResult<FileManagerDto>> ReadFile(string fileName, string downloadName, Dispositions disposition)
        {
            var result = await _service.ReadDataFile(fileName);

            if (disposition == Dispositions.Inline)
            {
                Response.Headers.Add("Content-Disposition", FileManagerHelper.GetContentDisposition(downloadName ?? result.FileName, "inline"));
                return PhysicalFile(result.FilePath, result.ContentType, true);
            }
            return PhysicalFile(result.FilePath, result.ContentType, disposition == Dispositions.Attachment ? downloadName ?? result.FileName : downloadName, true);
        }

        /// <summary>
        /// Consulta arquivo de dados temporário pelo nome.
        /// </summary>
        /// <param name="fileName">Nome do arquivo de dados temporário</param>
        /// <param name="downloadName">Nome sugerido para download</param>
        /// <param name="disposition">attachment ou inline</param>
        /// <returns>Arquivo de Dados</returns>
        [HttpGet("Temp/{fileName}")]
        public ActionResult<FileManagerDto> ReadTempFile(string fileName, string downloadName, Dispositions disposition)
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
        /// <param name="access">Acesso ao arquivo</param>
        /// <param name="cancellationToken">Token de cancelamento da requisição</param>
        /// <returns></returns>
        [HttpPost]
        [Produces("application/json")]
        public async Task<ActionResult<FileManagerResponseDto>> UploadData(IFormFile file, FileAccess access = FileAccess.Private, CancellationToken cancellationToken = default)
        {
            var result = await _service.UploadDataFile(file, access, cancellationToken);
            return CreatedAtAction(nameof(ReadFile), new { fileName = result.FileName }, result);
        }

        /// <summary>
        /// Carrega arquivo de dados temporário.
        /// </summary>
        /// <param name="file">Arquivo de Dados Temporario</param>
        /// <param name="cancellationToken">Token de cancelamento da requisição</param>
        /// <returns></returns>
        [HttpPost("Temp")]
        [Produces("application/json")]
        public async Task<ActionResult<FileManagerResponseDto>> UploadTempData(IFormFile file, CancellationToken cancellationToken)
        {
            var result = await _service.UploadTempFile(file, cancellationToken);
            return CreatedAtAction(nameof(ReadTempFile), new { fileName = result.FileName }, result);
        }

        /// <summary>
        /// Carrega grande arquivo de dados.
        /// </summary>
        /// <returns></returns>
        [HttpPost("Large")]
        [Produces("application/json")]
        public async Task<ActionResult<FileManagerResponseDto>> UploadLargeData()
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

            var result = await _service.UploadLargeDataFile(reader, section);
            return CreatedAtAction(nameof(ReadFile), new { fileName = result.FileName }, result);
        }

        /// <summary>
        /// Carrega grande arquivo de dados temporário.
        /// </summary>
        /// <returns></returns>
        [HttpPost("Temp/Large")]
        [Produces("application/json")]
        public async Task<ActionResult<FileManagerResponseDto>> UploadLargeTempData()
        {
            var request = HttpContext.Request;

            if (!request.HasFormContentType ||
                !MediaTypeHeaderValue.TryParse(request.ContentType, out var mediaTypeHeader) ||
                string.IsNullOrEmpty(mediaTypeHeader.Boundary.Value))
            {
                return new UnsupportedMediaTypeResult();
            }

            var reader = new MultipartReader(mediaTypeHeader.Boundary.Value.Trim('"'), Request.Body);
            var section = await reader.ReadNextSectionAsync();

            var result = await _service.UploadLargeTempFile(reader, section);
            return CreatedAtAction(nameof(ReadTempFile), new { fileName = result.FileName }, result);
        }

        /// <summary>
        /// Remove arquivo de dados.
        /// </summary>
        /// <param name="fileName">Nome do Arquivo de Dados</param>
        /// <param name="cancellationToken">Token de cancelamento da requisição</param>
        [HttpDelete("{fileName}")]
        [Produces("application/json")]
        public async Task<IActionResult> DeleteData(string fileName, CancellationToken cancellationToken)
        {
            await _service.DeleteDataFile(fileName, cancellationToken);
            return NoContent();
        }
    }
}
