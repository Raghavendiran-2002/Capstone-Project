
using System;
using System.IO;
using System.Net.Http;
using System.Text;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Azure.WebJobs;
using Microsoft.Azure.WebJobs.Extensions.Http;
using Microsoft.AspNetCore.Http;
using Microsoft.Extensions.Logging;
using Newtonsoft.Json;
using PdfSharp.Drawing;
using PdfSharp.Pdf;
using Azure.Storage.Blobs;

namespace CertificateGenerate
{
    public static class GenerateCertificate
    {
        private static readonly string storageConnectionString = Environment.GetEnvironmentVariable("AzureWebJobsStorage");
        private static readonly string containerName = "certificates";
        private static readonly HttpClient httpClient = new HttpClient();

        [FunctionName("GenerateCertificate")]
        public static async Task<IActionResult> Run(
            [HttpTrigger(AuthorizationLevel.Anonymous, "post", Route = null)] HttpRequest req,
            ILogger log)
        {
            log.LogInformation("C# HTTP trigger function processed a request.");

            // Read and parse the request body
            string requestBody = await new StreamReader(req.Body).ReadToEndAsync();
            dynamic data = JsonConvert.DeserializeObject(requestBody);

            // Extract parameters from the request body
            string name = data?.name;
            string date = DateTime.Now.ToShortDateString();
            string quizTopic = data?.quizTopic;
            string certType = data?.certType;

            if (string.IsNullOrEmpty(name) ||  string.IsNullOrEmpty(quizTopic) || string.IsNullOrEmpty(certType))
            {
                return new BadRequestObjectResult("Please provide name, quizTopic, and certType.");
            }

            // Generate PDF
           byte[] pdfBytes;
            using (MemoryStream stream = new MemoryStream())
            {
                PdfDocument document = new PdfDocument();
                PdfPage page = document.AddPage();
                XGraphics gfx = XGraphics.FromPdfPage(page);

                // Define fonts
                XFont titleFont = new XFont("Arial", 24, XFontStyleEx.Bold);
                XFont subtitleFont = new XFont("Arial", 18, XFontStyleEx.Bold);
                XFont normalFont = new XFont("Arial", 16);
                XFont italicFont = new XFont("Arial", 16, XFontStyleEx.Italic);

                // Define brushes
                XBrush blueBrush = new XSolidBrush(XColor.FromArgb(0, 51, 102));
                XBrush blackBrush = XBrushes.Black;
                
                // Draw border
                gfx.DrawRectangle(blueBrush, new XRect(20, 20, page.Width - 40, page.Height - 40));
                gfx.DrawRectangle(XBrushes.White, new XRect(25, 25, page.Width - 50, page.Height - 50));

                // Draw certificate content
                gfx.DrawString("CERTIFICATE OF COMPLETION", titleFont, blueBrush, new XRect(0, 60, page.Width, 40), XStringFormats.TopCenter);
                gfx.DrawString("THIS IS AWARDED TO", normalFont, blackBrush, new XRect(0, 120, page.Width, 40), XStringFormats.TopCenter);
                gfx.DrawString(name.ToUpper(), subtitleFont, blackBrush, new XRect(0, 160, page.Width, 40), XStringFormats.TopCenter);
                
                if (certType.ToLower() == "special")
                {
                    gfx.DrawString($"for completing the {quizTopic} quiz in 50% of the time", normalFont, blackBrush, new XRect(0, 200, page.Width, 40), XStringFormats.TopCenter);
                }
                else
                {
                    gfx.DrawString($"for successfully completing the {quizTopic} quiz", normalFont, blackBrush, new XRect(0, 200, page.Width, 40), XStringFormats.TopCenter);
                }

                gfx.DrawString($"Date: {date}", normalFont, blackBrush, new XRect(0, 240, page.Width, 40), XStringFormats.TopCenter);
                gfx.DrawString("Well done", italicFont, blackBrush, new XRect(0, 280, page.Width, 40), XStringFormats.TopCenter);
                gfx.DrawString("QUIZZ ACADEMY", normalFont, blackBrush, new XRect(0, 320, page.Width, 40), XStringFormats.TopCenter);

                // Optionally, add a seal image
                //gfx.DrawImage(XImage.FromFile("download.png"), page.Width - 150, page.Height - 150, 100, 100);

                document.Save(stream, false);
                pdfBytes = stream.ToArray();
            }
            // Upload PDF to Azure Blob Storage
            BlobServiceClient blobServiceClient = new BlobServiceClient(storageConnectionString);
            BlobContainerClient containerClient = blobServiceClient.GetBlobContainerClient(containerName);
            await containerClient.CreateIfNotExistsAsync();

            string blobName = $"{name}_{Guid.NewGuid()}.pdf";
            BlobClient blobClient = containerClient.GetBlobClient(blobName);
            using (MemoryStream ms = new MemoryStream(pdfBytes))
            {
                await blobClient.UploadAsync(ms, true);
            }

            // Get the PDF URL
            string pdfUrl = blobClient.Uri.ToString();

            // Prepare the request body for the POST method
            var postBody = new
            {
                name = name,
                date= date,
                certType = certType,
                pdfUrl = pdfUrl
            };
            return new OkObjectResult(postBody);           
        }
    }
}
