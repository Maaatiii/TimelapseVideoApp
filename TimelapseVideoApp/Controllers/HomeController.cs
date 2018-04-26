using System;
using System.Collections.Generic;
using System.Configuration;
using System.IO;
using System.Linq;
using System.Net;
using System.Net.Http;
using System.Web;
using System.Web.Mvc;
using Microsoft.WindowsAzure.Storage;
using Microsoft.WindowsAzure.Storage.Blob;

namespace TimelapseVideoApp.Controllers
{
    public class HomeController : Controller
    {
        //private readonly CloudStorageAccount _storageAccount = CloudStorageAccount.Parse(ConfigurationManager.AppSettings["Microsoft.WindowsAzure.Storage.ConnectionString"]);

        public ActionResult Index()
        {
            //CloudBlobClient blobClient = _storageAccount.CreateCloudBlobClient();
            //CloudBlobContainer container = blobClient.GetContainerReference("filmy");
            //ICloudBlob blob = container.get(_blobId);
            //Stream blobStream = await blob.OpenReadAsync();
            //DownloadPayload payload = new DownloadPayload();
            //payload.Stream = blobStream;
            //payload.ContentType = blob.Properties.ContentType;
            //return payload;


            //MemoryStream stream = new MemoryStream(new AzureMainStorage("avideo").GetFile(filename));

            //HttpResponseMessage partialResponse = Request.CreateResponse(HttpStatusCode.PartialContent);
            //partialResponse.Content = new ByteRangeStreamContent(stream, Request.Headers.Range, "video/mp4");
            //return partialResponse;

            //ViewBag.Url = "https://youtu.be/8wLCmDtCDAM";

            return View();
        }

        public ActionResult About()
        {
            ViewBag.Message = "Your application description page.";

            return View();
        }

        public ActionResult Contact()
        {
            ViewBag.Message = "Your contact page.";

            return View();
        }

    }
}