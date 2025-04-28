using Microsoft.AspNetCore.Mvc;
using System.IO;

namespace WebApplication3.Controllers
{
    public class MLController : Controller
    {
        [HttpGet]
        public IActionResult PreprocessDataView()
        {
            return View();
        }

        [HttpPost]
        public IActionResult PreprocessData()
        {
            string inputFile = Path.Combine(Directory.GetCurrentDirectory(), "model11.txt");
            string outputFile = Path.Combine(Directory.GetCurrentDirectory(), "model12_encoded.txt");

            WebApplication3.Utilities.DataPreprocessor.EncodeData(inputFile, outputFile);

            return Ok("Data preprocessing complete. Encoded file saved.");
        }
    }
}
