using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using System;
using System.Net;
using System.Web;
using Azure.Messaging.ServiceBus;
using Azure.Storage.Blobs;
using IRFestival.Api.Common;
using IRFestival.Api.Domain;
using IRFestival.Api.Options;
using Newtonsoft.Json;

namespace IRFestival.Api.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class PicturesController : ControllerBase
    {
        private BlobUtility BlobUtility { get; }
        private readonly IConfiguration Configuration;

        public PicturesController(BlobUtility blobUtility, IConfiguration configuration)
        {
            BlobUtility = blobUtility;
            Configuration = configuration;  
        }
        [HttpGet]
        [ProducesResponseType((int)HttpStatusCode.OK, Type = typeof(string[]))]
        public async Task<ActionResult> GetAllPictureUrls()
        {
            var container = BlobUtility.GetThumbsContainer();
            //var result = container.GetBlobs().Select(blob => BlobUtility.GetSasUri(container, blob.Name)).ToArray();
            //return Ok(result);
            //return Array.Empty<string>();
            return Ok(container.GetBlobs().Select(blob => BlobUtility.GetSasUri(container, blob.Name)).ToArray());
        }

        [HttpPost]
        [ProducesResponseType((int)HttpStatusCode.OK, Type = typeof(AppSettingsOptions))]
        public async Task<ActionResult> PostPicture(IFormFile file)
        {
            BlobContainerClient container = BlobUtility.GetPicturesContainer();
            var filename = $"{DateTimeOffset.UtcNow.ToUnixTimeSeconds()}{HttpUtility.UrlPathEncode(file.FileName.Replace(" ",""))}";
            await container.UploadBlobAsync(filename, file.OpenReadStream());
            await using (var client =
                         new ServiceBusClient(Configuration.GetConnectionString("ServiceBusSenderConnection")))
            {
                ServiceBusSender sender = client.CreateSender(Configuration.GetValue<string>("QueueNameMails"));
               
                Mail mail = new()
                {
                    EmailAddress = "me@you.us",
                    
                };
                mail.Message = $"The picture {filename}was uploaded ! Send a fictional mail to {mail.EmailAddress}";
                ServiceBusMessage message = new ServiceBusMessage(JsonConvert.SerializeObject(mail));
                await sender.SendMessageAsync(message);
            }
            
            return Ok();
        }
    }
}
