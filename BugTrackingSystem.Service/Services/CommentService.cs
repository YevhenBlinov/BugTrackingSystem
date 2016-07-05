using System.Collections.Generic;
using BugTrackingSystem.AzureService;
using BugTrackingSystem.Service.Models;

namespace BugTrackingSystem.Service.Services
{
    public class CommentService
    {
        private readonly TableService _tableService;

        public CommentService()
        {
            _tableService = new TableService();
        }

        public void AddComment(int bugId, string userName, string comment)
        {
            _tableService.InsertComment(bugId.ToString(), userName, comment);
        }

        public List<CommentViewModel> GetCommentsForBug(int bugId)
        {
            var azureComments = _tableService.RetrieveAllCommentsForBug(bugId.ToString());
            var comments = new List<CommentViewModel>();

            foreach (var commentModel in azureComments)
            {
                comments.Add(new CommentViewModel { UserName = commentModel.UserName, Comment = commentModel.Comment });
            }

            return comments;
        }
    }
}
