using System.Collections.Generic;
using System.Linq;
using Microsoft.Azure;
using Microsoft.WindowsAzure.Storage;
using Microsoft.WindowsAzure.Storage.Table;

namespace BugTrackingSystem.AzureService
{
    public class TableService
    {
        private const string ConnectionString = "StorageConnectionString";
        private const string TableName = "Comments";
        private CloudStorageAccount _storageAccount;
        private CloudTableClient _tableClient;
        private CloudTable _table;

        public TableService()
        {
            var appSetting = CloudConfigurationManager.GetSetting(ConnectionString);
            _storageAccount = CloudStorageAccount.Parse(appSetting);
            _tableClient = _storageAccount.CreateCloudTableClient();
            _table = _tableClient.GetTableReference(TableName);
            _table.CreateIfNotExists();
        }

        public void InsertComment(string bugId, string userId, string comment)
        {
            var commentToInsert = new CommentModel(bugId, userId, comment);
            var insertOperation = TableOperation.Insert(commentToInsert);
            _table.Execute(insertOperation);
        }

        public List<CommentModel> RetrieveAllCommentsForBug(string bugId)
        {
            var commentsQuery =
                new TableQuery<CommentModel>().Where(TableQuery.GenerateFilterCondition("PartitionKey",
                    QueryComparisons.Equal, bugId));
            var comments = _table.ExecuteQuery(commentsQuery).ToList();
            return comments;
        }

        public void DeleteTable()
        {
            _table.DeleteIfExists();
        }
    }
}
