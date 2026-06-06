using Microsoft.AspNetCore.Mvc;
using SixLabors.ImageSharp;
using SixLabors.ImageSharp.Formats.Webp;
using SixLabors.ImageSharp.Processing;

namespace abcsxl.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    public class UploadController : ControllerBase
    {
        private readonly IWebHostEnvironment _environment;

        public UploadController(IWebHostEnvironment environment)
        {
            _environment = environment;
        }

        [HttpPost("image")]
        public async Task<IActionResult> UploadImage(IFormFile file)
        {
            if (file == null || file.Length == 0)
            {
                return BadRequest("No file uploaded.");
            }

            // Check file type
            var allowedExtensions = new[] { ".jpg", ".jpeg", ".png", ".gif", ".bmp" };
            var extension = Path.GetExtension(file.FileName).ToLower();
            if (!allowedExtensions.Contains(extension))
            {
                return BadRequest("Invalid file type. Only image files are allowed.");
            }

            // Create uploads directory if not exists
            var uploadsFolder = Path.Combine(_environment.WebRootPath, "uploads");
            if (!Directory.Exists(uploadsFolder))
            {
                Directory.CreateDirectory(uploadsFolder);
            }

            // Generate unique filename with .webp extension
            var fileName = Guid.NewGuid().ToString() + ".webp";
            var filePath = Path.Combine(uploadsFolder, fileName);

            // Process image with ImageSharp
            using (var image = await Image.LoadAsync(file.OpenReadStream()))
            {
                // Resize if too large (max 1920px width, maintain aspect ratio)
                if (image.Width > 1920)
                {
                    image.Mutate(x => x.Resize(1920, 0));
                }

                // Compress and save as WebP
                await image.SaveAsWebpAsync(filePath, new WebpEncoder
                {
                    Quality = 80 // Good quality compression
                });
            }

            // Return the URL
            var url = $"/uploads/{fileName}";
            return Ok(new { data = new { succ = true, data = url } });
        }
    }
}
