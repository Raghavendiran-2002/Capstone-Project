

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
            string issueDate = data?.issueDate;
            string expDate = data?.expDate;
            string certType = data?.certType;

            if (string.IsNullOrEmpty(name) || string.IsNullOrEmpty(issueDate) || string.IsNullOrEmpty(expDate) || string.IsNullOrEmpty(certType))
            {
                return new BadRequestObjectResult("Please provide name, issueDate, expDate, and certType.");
            }

            // Generate PDF
            byte[] pdfBytes;
            using (MemoryStream stream = new MemoryStream())
            {
                PdfDocument document = new PdfDocument();
                PdfPage page = document.AddPage();
                XGraphics gfx = XGraphics.FromPdfPage(page);
                
                // Define fonts
                XFont titleFont = new XFont("Verdana", 24, XFontStyleEx.Bold);
                XFont subtitleFont = new XFont("Verdana", 20, XFontStyleEx.Bold);
                XFont normalFont = new XFont("Verdana", 16);
                XFont italicFont = new XFont("Verdana", 16, XFontStyleEx.Bold);

                // Draw certificate content
                gfx.DrawString("CERTIFICATE", titleFont, XBrushes.Black, new XRect(0, 50, page.Width, page.Height), XStringFormats.TopCenter);
                gfx.DrawString($"OF {certType.ToUpper()}", subtitleFont, XBrushes.Black, new XRect(0, 90, page.Width, page.Height), XStringFormats.TopCenter);
                gfx.DrawString("PROUDLY PRESENTED TO", normalFont, XBrushes.Black, new XRect(0, 150, page.Width, page.Height), XStringFormats.TopCenter);
                gfx.DrawString(name, subtitleFont, XBrushes.Black, new XRect(0, 180, page.Width, page.Height), XStringFormats.TopCenter);
                gfx.DrawString("For successfully completing Inter-Institute Quiz", normalFont, XBrushes.Black, new XRect(0, 240, page.Width, page.Height), XStringFormats.TopCenter);
                gfx.DrawString($"Competition ({issueDate})", normalFont, XBrushes.Black, new XRect(0, 270, page.Width, page.Height), XStringFormats.TopCenter);
                gfx.DrawString("We acknowledge your efforts, keep participating.", italicFont, XBrushes.Black, new XRect(0, 310, page.Width, page.Height), XStringFormats.TopCenter);
                
                gfx.DrawString("Convener,", normalFont, XBrushes.Black, new XRect(100, 400, page.Width, page.Height), XStringFormats.TopLeft);
                gfx.DrawString("GIET", normalFont, XBrushes.Black, new XRect(100, 430, page.Width, page.Height), XStringFormats.TopLeft);
                
                gfx.DrawString("Principal,", normalFont, XBrushes.Black, new XRect(page.Width - 200, 400, page.Width, page.Height), XStringFormats.TopLeft);
                gfx.DrawString("GIET", normalFont, XBrushes.Black, new XRect(page.Width - 200, 430, page.Width, page.Height), XStringFormats.TopLeft);

                document.Save(stream, false);
                pdfBytes = stream.ToArray();
            }

            // Upload PDF to Azure Blob Storage
            BlobServiceClient blobServiceClient = new BlobServiceClient(storageConnectionString);
            BlobContainerClient containerClient = blobServiceClient.GetBlobContainerClient(containerName);
            await containerClient.CreateIfNotExistsAsync();

            string blobName = $"{Guid.NewGuid()}.pdf";
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
                issueDate = issueDate,
                expDate = expDate,
                certType = certType,
                pdfUrl = pdfUrl
            };
            return new OkObjectResult(postBody);

            // Update to Database

    
            // string postBodyJson = JsonConvert.SerializeObject(postBody);
            // StringContent content = new StringContent(postBodyJson, Encoding.UTF8, "application/json");

            // // Call the POST method
            // HttpResponseMessage response = await httpClient.PostAsync("http://update.db.com/api/certs", content);

            // if (response.IsSuccessStatusCode)
            // {
            //     string responseContent = await response.Content.ReadAsStringAsync();
            //     return new OkObjectResult(new { pdfUrl, responseContent });
            // }
            // else
            // {
            //     string errorContent = await response.Content.ReadAsStringAsync();
            //     return new ObjectResult(errorContent) { StatusCode = (int)response.StatusCode };
            // }
        }
    }
}
