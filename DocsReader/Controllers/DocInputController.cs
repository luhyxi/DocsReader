using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.StaticFiles;

// For more information on enabling Web API for empty projects, visit https://go.microsoft.com/fwlink/?LinkID=397860

namespace DocsReader.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class DocInputController : ControllerBase
    {
        IContentTypeProvider _contentTypeProvider;
        public DocInputController(IContentTypeProvider contentTypeProvider)
        {
            _contentTypeProvider = contentTypeProvider;
        }

        // GET api/<DocInputController>/5
        [HttpGet]
        public IActionResult Get(string filename)
        {
            string path = $"wwwroot/{filename}";

            if (!System.IO.File.Exists(path)) return BadRequest();

            var readFile = System.IO.File.ReadAllBytes(path);
            _contentTypeProvider.TryGetContentType(path, out string contentType);
            return File(readFile, contentType);
        }

        // POST api/<DocInputController>
        [HttpPost]
        public async Task<IActionResult> Post(IFormFile file)
        {
            string path = $"wwwroot/{file.FileName}";

            if (ValidateFile(file) == false) return BadRequest();

            using (var FileStream = System.IO.File.Create(path)) { await file.CopyToAsync(FileStream); }
            return Ok(file.FileName);
        }

        [ApiExplorerSettings(IgnoreApi = true)]

        public bool ValidateFile(IFormFile file)
        {
            List<string> acceptedExtentions = new List<string>() { ".pdf", ".docx", ".doc" };

            if (file == null) { return false; }
            if (file.Length > 5 * 1024 * 1024) { return false; }
            if (!acceptedExtentions.Contains(Path.GetExtension(file.FileName))) { return false; }

            return true;
        }
    }
}
