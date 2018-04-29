using System;
using System.Diagnostics;
using System.Threading;
using System.Threading.Tasks;
using Microsoft.Azure.WebJobs;
using Microsoft.Azure.WebJobs.Host;
using Microsoft.WindowsAzure.Storage;
using Microsoft.WindowsAzure.Storage.Blob;
using Microsoft.WindowsAzure.Storage.DataMovement;

namespace TimelapseBlobCopyFunction
{
    public static class CopyBlobFunction2
    {
        private static readonly CloudStorageAccount _storageAccount = CloudStorageAccount.Parse("DefaultEndpointsProtocol=https;AccountName=blobteststaw95f8;AccountKey=FMXfZH6HheMsloaWHyEypL98R7RYFH8NKG/CYKlflxcBU/jIrO3EkOtOtRFLKBjMsWcso8azEcfSydhAlGTKzg==;EndpointSuffix=core.windows.net");
        private static readonly CloudStorageAccount _targetstorageAccount = CloudStorageAccount.Parse("DefaultEndpointsProtocol=https;AccountName=20532storagmc;AccountKey=gvPdMEhrWjHQ1nY/loOEH4znittI/1Hj7gxByIYT8qwApNCIBsrqTUqEebjhSdkrtRTbMf6f5B4rx758EdFhRQ==;EndpointSuffix=core.windows.net");

        [FunctionName("CopyBlobFunction2")]
        public static async Task Run([TimerTrigger("0 */5 * * * *")]TimerInfo myTimer, TraceWriter log)
        {
            try
            {
                await TransferAzureBlobToAzureBlob(_storageAccount, _targetstorageAccount, log);
            }
            catch (Exception e)
            {
                log.Error(e.ToString());
                throw;
            }
        }

        public static SingleTransferContext GetSingleTransferContext(TraceWriter log)
        {
            SingleTransferContext context = new SingleTransferContext();

            context.ProgressHandler = new Progress<TransferStatus>((progress) =>
            {
                log.Info($"\rBytes transferred: {progress.BytesTransferred}");
            });

            return context;
        }


        public static async Task TransferAzureBlobToAzureBlob(CloudStorageAccount sourceAccount, CloudStorageAccount targetAccount, TraceWriter log)
        {
            CloudBlockBlob sourceBlob = await GetBlob(sourceAccount, "filmy", "stawy_latest.mp4");
            CloudBlockBlob destinationBlob = await GetBlob(targetAccount, "filmycopied", "stawy_latest.mp4");
            SingleTransferContext context = GetSingleTransferContext(log);
            CancellationTokenSource cancellationSource = new CancellationTokenSource();
            log.Info("\nTransfer started...\nPress 'c' to temporarily cancel your transfer...\n");

            Stopwatch stopWatch = Stopwatch.StartNew();

            try
            {
                await TransferManager.CopyAsync(sourceBlob, destinationBlob, true, null, context, cancellationSource.Token);
            }
            catch (Exception e)
            {
                log.Error($"\nThe transfer is canceled: {e.Message}");
            }

            if (cancellationSource.IsCancellationRequested)
            {
                await TransferManager.CopyAsync(sourceBlob, destinationBlob, false, null, context, cancellationSource.Token);
            }

            stopWatch.Stop();
            log.Info($"\nTransfer operation completed in {stopWatch.Elapsed.TotalSeconds} seconds.");
        }

        public static async Task<CloudBlockBlob> GetBlob(CloudStorageAccount account, string containerName, string blobName)
        {
            CloudBlobClient blobClient = account.CreateCloudBlobClient();

            CloudBlobContainer container = blobClient.GetContainerReference(containerName);
            await container.CreateIfNotExistsAsync();

            CloudBlockBlob blob = container.GetBlockBlobReference(blobName);

            return blob;
        }
    }
}