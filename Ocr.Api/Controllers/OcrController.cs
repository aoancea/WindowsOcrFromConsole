using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Net;
using System.Threading.Tasks;
using WindowsOcrWrapper;
using WindowsOcrWrapper.WinOcrResults;

namespace Ocr.Api.Controllers
{
    [ApiController]
    [Route("[controller]")]
    public class OcrController : ControllerBase
    {
        private static readonly string[] Summaries = new[]
        {
            "Freezing", "Bracing", "Chilly", "Cool", "Mild", "Warm", "Balmy", "Hot", "Sweltering", "Scorching"
        };

        [HttpGet]
        public IEnumerable<WeatherForecast> Get()
        {
            var rng = new Random();
            return Enumerable.Range(1, 5).Select(index => new WeatherForecast
            {
                Date = DateTime.Now.AddDays(index),
                TemperatureC = rng.Next(-20, 55),
                Summary = Summaries[rng.Next(Summaries.Length)]
            })
            .ToArray();
        }

        [HttpPost]
        public async Task<IActionResult> Post()
        {
            IFormFile[] files = Request.Form.Files.ToArray();

            using MemoryStream memoryStream = new MemoryStream();

            await files[0].CopyToAsync(memoryStream);

            string tempFileName = Path.GetTempFileName();
            System.IO.File.WriteAllBytes(tempFileName, memoryStream.ToArray());

            OcrExecutor ocrExecutor = new OcrExecutor();
            OcrResult ocrResult = await ocrExecutor.GetOcrResultAsync(tempFileName);

            System.IO.File.Delete(tempFileName);

            return Ok(ocrResult);
        }
    }
}