using System;
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
        private const string pathToFileWithBlobsNames = "BlobsNames.txt";
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

        public void UploadBlobIntoContainer()
        {
            var isFileFounded = false;
            var pathToFile = string.Empty;
           
            while (!isFileFounded)
            {
                Console.WriteLine("Please, enter the pass to the file, which you want to upload.");
                pathToFile = Console.ReadLine();

                if (!File.Exists(pathToFile))
                {
                    Console.WriteLine("The path is invalid, please, try again.");
                }
                else
                {
                    isFileFounded = true;
                }
            }

            var fileName = string.Empty;

            using (var fileStream = File.OpenRead(pathToFile))
            {
                fileName = Path.GetFileName(pathToFile);
                var blockBlob = _container.GetBlockBlobReference(fileName);
                blockBlob.UploadFromStream(fileStream);
            }

            File.WriteAllText(pathToFileWithBlobsNames, fileName);            
        }

        public void DisplayBlobList()
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
        }

        public void DownloadBlobFromContainer()
        {
            var isDirecrotyFounded = false;
            var pathToDirectory = string.Empty;

            while (!isDirecrotyFounded)
            {
                Console.WriteLine("Please, enter the pass to the directory, in which you want to save the file.");
                pathToDirectory = Console.ReadLine();

                if (!Directory.Exists(pathToDirectory))
                {
                    Console.WriteLine("The path is invalid, please, try again.");
                }
                else
                {
                    isDirecrotyFounded = true;
                }
            }

            var fileName = string.Empty;
            var isBlobWithTheNameExists = false;

            while (!isBlobWithTheNameExists)
            {
                Console.WriteLine("Please, write a name for the file with an extension, in which you want to save the blob information.");
                fileName = Console.ReadLine();
                var blobsNames = File.ReadAllLines(pathToFileWithBlobsNames);
                isBlobWithTheNameExists = blobsNames.Any(blobName => blobName == fileName);   
            }
            
            using (var fileStream = File.OpenWrite(pathToDirectory + "\\" + fileName))
            {
                var blockBlob = _container.GetBlockBlobReference(fileName);
                blockBlob.DownloadToStream(fileStream);
            }
        }

        public void DeleteBlobFromContainer()
        {
            var fileName = string.Empty;
            var isBlobWithTheNameExists = false;

            while (!isBlobWithTheNameExists)
            {
                Console.WriteLine("Please, write a name for the file with an extension, which you want to delete.");
                fileName = Console.ReadLine();
                var blobsNames = File.ReadAllLines(pathToFileWithBlobsNames);
                isBlobWithTheNameExists = blobsNames.Any(blobName => blobName == fileName);
            }

            var blockBlob = _container.GetBlockBlobReference(fileName);
            blockBlob.Delete();

            var tempFile = Path.GetTempFileName();
            var linesToKeep = File.ReadLines(pathToFileWithBlobsNames).Where(l => l != fileName);
            File.WriteAllLines(tempFile, linesToKeep);
            File.Delete(pathToFileWithBlobsNames);
            File.Move(tempFile, pathToFileWithBlobsNames);
        }
    }
}
