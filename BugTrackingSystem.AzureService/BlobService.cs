using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using Microsoft.Azure;
using Microsoft.WindowsAzure.Storage;
using Microsoft.WindowsAzure.Storage.Blob;

namespace BugTrackingSystem.AzureService
{
    public class BlobService
    {
        private const string ConnectionString = "StorageConnectionString";
        private readonly CloudStorageAccount _storageAccount;
        private readonly CloudBlobClient _blobClient;
        private readonly CloudBlobContainer _container;

        public BlobService(string containerName)
        {
            var appSetting = CloudConfigurationManager.GetSetting(ConnectionString);
            _storageAccount = CloudStorageAccount.Parse(appSetting);
            _blobClient = _storageAccount.CreateCloudBlobClient();
            _container = _blobClient.GetContainerReference(containerName);
            _container.CreateIfNotExists();
        }

        public void UploadBlobIntoContainer(string pathToFile)
        {
            if (File.Exists(pathToFile)) 
                return;

            var fileName = string.Empty;

            using (var fileStream = File.OpenRead(pathToFile))
            {
                fileName = Path.GetFileName(pathToFile);
                var blockBlob = _container.GetBlockBlobReference(fileName);
                blockBlob.UploadFromStream(fileStream);
            }
        }

        public List<string> GetBlockBlobList()
        {
            foreach (var item in _container.ListBlobs())
            {
                if (item.GetType() == typeof(CloudBlockBlob))
                {
                    var blob = (CloudBlockBlob)item;
                    Console.WriteLine("Block blob with name {0} of length {1}: {2}", blob.Name, blob.Properties.Length, blob.Uri);
                }
                else if (item.GetType() == typeof(CloudPageBlob))
                {
                    var pageBlob = (CloudPageBlob)item;
                    Console.WriteLine("Page blob with name {0} of length {1}: {2}", pageBlob.Name, pageBlob.Properties.Length, pageBlob.Uri);
                }
                else if (item.GetType() == typeof(CloudBlobDirectory))
                {
                    var directory = (CloudBlobDirectory)item;
                    Console.WriteLine("Directory: {0}", directory.Uri);
                }
            }

            return new List<string>();
        }

        public void DownloadBlobFromContainer(string blobName, string pathToFile)
        {
            var listBlockBlobs = _container.ListBlobs().Where(b => b.GetType() == typeof(CloudBlockBlob));
            var isBlobExists = listBlockBlobs.Cast<CloudBlockBlob>().Any(b => b.Name == blobName);

            if (!isBlobExists)
                return;

            using (var fileStream = File.OpenWrite(pathToFile))
            {
                var blockBlob = _container.GetBlockBlobReference(blobName);
                blockBlob.DownloadToStream(fileStream);
            }
        }

        public void DeleteBlobFromContainer(string blobName)
        {
            var blockBlob = _container.GetBlockBlobReference(blobName);
            blockBlob.Delete();
        }
    }
}
