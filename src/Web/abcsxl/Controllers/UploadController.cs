using Microsoft.AspNetCore.Mvc;
using SkiaSharp;

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

            // Process image with SkiaSharp
            using var input = file.OpenReadStream();
            using var original = SKBitmap.Decode(input);
            if (original == null)
            {
                return BadRequest("Invalid image file.");
            }

            int width = original.Width;
            int height = original.Height;
            if (width > 1920)
            {
                float ratio = 1920f / width;
                width = 1920;
                height = (int)(height * ratio);
            }

            SKBitmap? resized = null;
            if (width != original.Width || height != original.Height)
            {
                var info = new SKImageInfo(width, height);
                resized = original.Resize(info, SKFilterQuality.High);
            }

            using (resized ?? original)
            using (var image = SKImage.FromBitmap(resized ?? original))
            using (var data = image.Encode(SKEncodedImageFormat.Webp, 80))
            using (var fs = new FileStream(filePath, FileMode.Create, FileAccess.Write))
            {
                data.SaveTo(fs);
            }

            // Return the URL
            var url = $"/uploads/{fileName}";
            return Ok(new { data = new { succ = true, data = url } });
        }
    }
}
