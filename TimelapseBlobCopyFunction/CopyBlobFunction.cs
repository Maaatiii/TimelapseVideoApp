using System;
using System.IO;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.Azure.WebJobs;
using Microsoft.Azure.WebJobs.Host;
using Microsoft.WindowsAzure.Storage;
using Microsoft.WindowsAzure.Storage.Blob;

namespace TimelapseBlobCopyFunction
{
    public static class CopyBlobFunction
    {
        private static readonly CloudStorageAccount _storageAccount = CloudStorageAccount.Parse("DefaultEndpointsProtocol=https;AccountName=blobteststaw95f8;AccountKey=FMXfZH6HheMsloaWHyEypL98R7RYFH8NKG/CYKlflxcBU/jIrO3EkOtOtRFLKBjMsWcso8azEcfSydhAlGTKzg==;EndpointSuffix=core.windows.net");
        private static readonly CloudStorageAccount _targetstorageAccount = CloudStorageAccount.Parse("DefaultEndpointsProtocol=https;AccountName=20532storagmc;AccountKey=gvPdMEhrWjHQ1nY/loOEH4znittI/1Hj7gxByIYT8qwApNCIBsrqTUqEebjhSdkrtRTbMf6f5B4rx758EdFhRQ==;EndpointSuffix=core.windows.net");

        [FunctionName("CopyBlobFunction")]
        public static async Task Run([TimerTrigger("0 */5 * * * *")]TimerInfo myTimer, TraceWriter log)
        {
            try
            {
                log.Info($"Run");

                CloudBlobClient blobClient = _storageAccount.CreateCloudBlobClient();

                log.Info($"Create source client");

                CloudBlobContainer container = blobClient.GetContainerReference("filmy");

                log.Info($"Get container");

                var sourceBlob = container.GetBlockBlobReference("stawy_latest.mp4");

                var policy = new SharedAccessBlobPolicy
                {
                    Permissions = SharedAccessBlobPermissions.Read,
                    SharedAccessStartTime = DateTime.UtcNow.AddMinutes(-15),
                    SharedAccessExpiryTime = DateTime.UtcNow.AddDays(7)
                };
                // Get SAS of that policy.
                var sourceBlobToken = sourceBlob.GetSharedAccessSignature(policy);
                // Make a full uri with the sas for the blob.
                var sourceBlobSAS = string.Format("{0}{1}", sourceBlob.Uri, sourceBlobToken);


                log.Info($"Source blob obtained: {sourceBlob.Uri}");

                CloudBlobClient targetBlobClient = _targetstorageAccount.CreateCloudBlobClient();

                log.Info($"Create target Cloud blob client");

                CloudBlobContainer targetContainer = targetBlobClient.GetContainerReference("filmycopied");

                log.Info($"get container ");

                await targetContainer.CreateIfNotExistsAsync();

                log.Info($"container created");

                //await targetContainer.CreateIfNotExistsAsync(BlobContainerPublicAccessType.Blob, 
                //    new BlobRequestOptions(), new OperationContext());                

                var targetBlob = targetContainer.GetBlockBlobReference("stawy_latest_copy.mp4");

                //await targetBlob.DeleteIfExistsAsync();

                log.Info($"created");

                log.Info($"begin copy");

                //string sasToken = container.GetSharedAccessSignature(new SharedAccessBlobPolicy(), "ReadBlobPolicy");

                var result = await targetBlob.StartCopyAsync(new Uri(sourceBlobSAS)); 
                    //AccessCondition.GenerateIfNotExistsCondition(), 
                    //AccessCondition.GenerateIfNotExistsCondition(), new BlobRequestOptions(),
                    //new OperationContext()
                     //);

                log.Info($"Blob copied addres: {result}");                

                log.Info($"C# Timer trigger function executed at: {DateTime.Now}");
            }
            catch (Exception e)
            {
                log.Error(e.ToString());
                throw;
            }
        }
    }
}
