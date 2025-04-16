using System.Net;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using WebApplication3.Repository;
using WebApplication5.Repository;

namespace WebApplication5.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    public class ImagesController : ControllerBase
    {
        private readonly IImageRepository _imageRepository;

        public ImagesController(IImageRepository imageRepository)
        {
            _imageRepository = imageRepository;
        }

        [HttpPost]
        [Route("")]
        public async Task<IActionResult> UploadImage([FromForm] IFormFile file)
        {
            if (file == null || file.Length == 0)
            {
                return BadRequest(new { message = "The file field." });
            }

            var imageUrl = await _imageRepository.UploadAsync(file);
            if (imageUrl == null)
            {
                return StatusCode(500, "Image upload failed.");
            }

            return Ok(new { success = true, url = imageUrl });
        }
    }

}
