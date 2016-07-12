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

        public string UploadBlobIntoContainerFromFile(string pathToFile)
        {
            if (!File.Exists(pathToFile)) 
                return String.Empty;

            var fileName = Path.GetFileName(pathToFile);
            var blockBlob = _container.GetBlockBlobReference(fileName);
            blockBlob.UploadFromFile(pathToFile);
            return fileName;
        }

        public void UploadBlobIntoContainerFromByteArray(string name, byte[] byteArray)
        {
            var blockBlob = _container.GetBlockBlobReference(name);

            using (var stream = new MemoryStream(byteArray, false))
            {
                blockBlob.UploadFromStream(stream);
            }
        }

        public Dictionary<string, string> GetBlockBlobDictionary()
        {
            var listBlockBlobs = _container.ListBlobs().Where(b => b.GetType() == typeof(CloudBlockBlob));
            var blobDicrionary = listBlockBlobs.Cast<CloudBlockBlob>().ToDictionary(listBlockBlob => listBlockBlob.Name, listBlockBlob => listBlockBlob.Uri.AbsoluteUri);
            return blobDicrionary;
        }

        public void DownloadBlobFromContainer(string blobName, string pathToFile)
        {
            var listBlockBlobs = _container.ListBlobs().Where(b => b.GetType() == typeof(CloudBlockBlob));
            var isBlobExists = listBlockBlobs.Cast<CloudBlockBlob>().Any(b => b.Name == blobName);

            if (!isBlobExists)
                return;

            var blockBlob = _container.GetBlockBlobReference(blobName);
            blockBlob.DownloadToFile(pathToFile, FileMode.OpenOrCreate);
        }

        public string GetBlobSasUri(string blobName)
        {
            var blob = _container.GetBlockBlobReference(blobName);
            var sasConstraints = new SharedAccessBlobPolicy
            {
                SharedAccessExpiryTime = DateTime.UtcNow + TimeSpan.FromMinutes(10),
                Permissions = SharedAccessBlobPermissions.Read
            };

            var sasBlobToken = blob.GetSharedAccessSignature(sasConstraints);
            var uri = new Uri(blob.Uri.AbsoluteUri + sasBlobToken);
            return uri.ToString();
        }

        public void DeleteBlobFromContainer(string blobName)
        {
            var blockBlob = _container.GetBlockBlobReference(blobName);
            blockBlob.Delete();
        }
    }
}
