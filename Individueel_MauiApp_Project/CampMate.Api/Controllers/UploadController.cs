using Microsoft.AspNetCore.Mvc;

namespace CampMate.Api.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class UploadController : ControllerBase
    {
        private readonly IWebHostEnvironment _environment;

        public UploadController(IWebHostEnvironment environment)
        {
            _environment = environment;
        }

        [HttpPost]
        public async Task<IActionResult> UploadImage(IFormFile file)
        {
            if (file == null || file.Length == 0)
                return BadRequest("Geen bestand geüpload.");

            try
            {
                // 1. Unieke bestandsnaam genereren (zodat bestanden elkaar niet overschrijven)
               
                string uniqueFileName = Guid.NewGuid().ToString() + Path.GetExtension(file.FileName);

                // 2. Het pad bepalen waar het bestand wordt opgeslagen (wwwroot/images)
                string uploadsFolder = Path.Combine(_environment.WebRootPath, "images");

               
                if (!Directory.Exists(uploadsFolder))
                    Directory.CreateDirectory(uploadsFolder);

                string filePath = Path.Combine(uploadsFolder, uniqueFileName);

                // 3. Bestand opslaan op de server
                using (var fileStream = new FileStream(filePath, FileMode.Create))
                {
                    await file.CopyToAsync(fileStream);
                }

                // 4. Stuur de nieuwe bestandsnaam terug naar de app
                return Ok(new { imageName = uniqueFileName });
            }
            catch (Exception ex)
            {
                return StatusCode(500, $"Interne serverfout: {ex.Message}");
            }
        }
    }
}