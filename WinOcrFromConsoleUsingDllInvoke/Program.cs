﻿using Microsoft.Owin.Hosting;
using System;
using System.IO;
using System.Linq;
using System.Net.Http;
using System.Net.Http.Headers;
using System.Threading.Tasks;
using WindowsOcrWrapper;

namespace WinOcrFromConsoleUsingDllInvoke
{
    /// <summary>
    /// Add dependency injection, remove test client ...
    /// index.html is just a test html page to check if upload is working
    /// </summary>
    class Program
    {
        static async Task Main(string[] args)
        {
            //var png = ConfigurationManager.AppSettings["ClocksFolder"] + @"\AAA_BXSP001_060.mxf_clock.png";
            string png = @"E:\projects\WindowsOcrFromConsole\data\1341000-0_UCMR_6000011a1_1562654575151.jpg";
            //RunOcrForAllClocksInParallel();

            await UploadImageInApi();

            string baseAddress = "http://localhost:9000/";

            // Start OWIN host 
            using (WebApp.Start<Startup>(url: baseAddress))
            {
                // Create HttpCient and make a request to api/values 
                HttpClient client = new HttpClient();
                Console.WriteLine("Starting invoke...");
                var response = UploadImage(baseAddress + "api/values", File.ReadAllBytes(png)).Result;

                Console.WriteLine(response);
                Console.WriteLine(response.Content.ReadAsStringAsync().Result);
                Console.ReadLine();
            }
        }

        private static void RunOcrForAllClocksInParallel()
        {
            var dir = @"C:\Users\mihai.petrutiu\Downloads\clocks\clocks\";
            string[] files = Directory.GetFiles(dir).Where(f => f.EndsWith("png")).ToArray();
            OcrExecutor ocrExecutor = new OcrExecutor();

            Parallel.For(0, files.Length, async (i) =>
            {
                var result = await ocrExecutor.GetOcrResultAsync(files[i]);
                Console.WriteLine(result.Text);
                Console.WriteLine(i);
            });
            Console.ReadKey();
        }

        static async public Task<HttpResponseMessage> UploadImage(string url, byte[] ImageData)
        {
            HttpClient client = new HttpClient();
            var requestContent = new MultipartFormDataContent();
            //    here you can specify boundary if you need---^
            var imageContent = new ByteArrayContent(ImageData);
            imageContent.Headers.ContentType =
                MediaTypeHeaderValue.Parse("image/jpeg");

            requestContent.Add(imageContent, "image", "image.jpg");

            return await client.PostAsync(url, requestContent);
        }

        private static async Task UploadImageInApi()
        {
            string imagePath = @"E:\projects\vdb\data\1341000-0_UCMR_6000011a1_1562654575151.jpg";

            byte[] image = File.ReadAllBytes(imagePath);

            ByteArrayContent imageContent = new ByteArrayContent(image);
            imageContent.Headers.ContentType = MediaTypeHeaderValue.Parse("image/jpeg");

            MultipartFormDataContent requestContent = new MultipartFormDataContent();
            requestContent.Add(imageContent, "image", "image.jpg");

            HttpClient client = new HttpClient();
            HttpResponseMessage ocrResponse = await client.PostAsync("https://localhost:5001/ocr", requestContent);

            Console.WriteLine(ocrResponse);
            Console.WriteLine(ocrResponse.Content.ReadAsStringAsync().Result);
            Console.ReadLine();
        }
    }
}
