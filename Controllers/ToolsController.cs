using FileManager.DTO;
using FileManager.Helpers;
using FileManager.Services.Tools;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace FileManager.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class ToolsController : ControllerBase
    {
        private readonly IToolService _service;

        public ToolsController(IToolService service) => _service = service;

        /// <summary>
        /// Consulta logo da aplicação.
        /// </summary>
        /// <returns>Arquivo de Dados</returns>
        [HttpGet("Logo")]
        [AllowAnonymous]
        public ActionResult<FileManagerDto> ReadLogo()
        {
            (var byteArray, string contentType) = _service.ReadLogoFile();

            Response.Headers.Add("Content-Disposition", FileManagerHelper.GetContentDisposition(ToolService.LogoFileName, "inline"));
            return File(byteArray, contentType, ToolService.LogoFileName, true);
        }

        /// <summary>
        /// Carrega logo da aplicação.
        /// </summary>
        /// <param name="file">Arquivo de Dados</param>
        /// <param name="cancellationToken">Token de cancelamento da requisição</param>
        /// <returns></returns>
        [HttpPost]
        [Produces("application/json")]
        public async Task<ActionResult<FileManagerResponseDto>> UploadMedia(IFormFile file, CancellationToken cancellationToken = default)
        {
            var result = await _service.UploadLogoFile(file, cancellationToken);
            return CreatedAtAction(nameof(ReadLogo), new { fileName = result.FileName }, result);
        }
    }
}
