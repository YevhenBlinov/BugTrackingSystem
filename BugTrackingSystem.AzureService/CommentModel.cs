using System;
using Microsoft.WindowsAzure.Storage.Table;

namespace BugTrackingSystem.AzureService
{
    public class CommentModel : TableEntity
    {
        public string Comment { get; set; }

        public string UserName { get; set; }

        public CommentModel()
        {
        }

        public CommentModel(string bugId, string userName, string comment)
        {
            PartitionKey = bugId;
            RowKey = Guid.NewGuid().ToString();
            Comment = comment;
            UserName = userName;
        }
    }
}
