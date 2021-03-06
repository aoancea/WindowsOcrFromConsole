﻿using System.Collections.Generic;
using System.IO;
using System.Net;
using System.Net.Http;
using System.Threading.Tasks;
using System.Web.Http;
using WindowsOcrWrapper;

namespace WinOcrFromConsoleUsingDllInvoke
{
    //[Authorize]
    public class ValuesController : ApiController
    {
        // GET api/values
        public IEnumerable<string> Get()
        {
            return new string[] { "value1", "value2" };
        }

        // GET api/values/5
        public string Get(int id)
        {
            return "value";
        }

        public async Task<IHttpActionResult> Post()
        {
            if (!Request.Content.IsMimeMultipartContent())
                throw new HttpResponseException(HttpStatusCode.UnsupportedMediaType);

            var provider = new MultipartMemoryStreamProvider();//For now I read the file in memory, but that is not needed, it could be saved directly to disk.
            await Request.Content.ReadAsMultipartAsync(provider);
            foreach (var file in provider.Contents)
            {                
                var buffer = await file.ReadAsByteArrayAsync();
                string tempFileName = Path.GetTempFileName();
                File.WriteAllBytes(tempFileName, buffer);
                OcrExecutor ocrExecutor = new OcrExecutor();//TODO: Should be SINGLETON, created using DI Container
                var ocrResult = await ocrExecutor.GetOcrResultAsync(tempFileName);
                File.Delete(tempFileName);
                return Ok(ocrResult);
            }

            return Ok();
        }
    }
}
