using System;
using System.Collections.Generic;
using System.IO;
using System.Web;
using System.Web.Mvc;
using BugTrackingSystem.Service.Models.FormModels;
using BugTrackingSystem.Service.Services;

namespace BugTrackingSystem.Web.Controllers
{
    public class TaskController : Controller
    {
        private readonly IBugService _bugService;

        public TaskController(IBugService bugService)
        {
            _bugService = bugService;
        }

        //
        // GET: /Task/
        public ActionResult Task(int bugId = 1)
        {
            var info = _bugService.GetFullBugById(bugId);
            return View(info);
        }

        [AcceptVerbs(HttpVerbs.Post)]
        public ActionResult CreateTask(BugFormViewModel bug, HttpPostedFileBase[] image)
        {
            if (image[0] != null)
            {
                bug.Attachments = new Dictionary<string, byte[]>();

                foreach (HttpPostedFileBase item in image)
                {
                    using (Stream inputStream = item.InputStream)
                    {
                        MemoryStream memoryStream = inputStream as MemoryStream;

                        if (memoryStream == null)
                        {
                            memoryStream = new MemoryStream();
                            inputStream.CopyTo(memoryStream);
                        }

                        byte[] data = memoryStream.ToArray();
                        bug.Attachments.Add(item.FileName, data);
                    }
                }
            }
            _bugService.AddNewBug(bug);
            return RedirectToActionPermanent("Index", "Project");
        }

        public void ChangeStatus(int bugId, string status)
        {
            _bugService.UpdateBugStatus(bugId, status);
        }

        public ActionResult TaskComments(int bugId)
        {
            var comments = _bugService.GetBugCommentsByBugId(bugId);
            ViewBag.BugId = bugId;
            return PartialView(comments);
        }

        public void AddComment(int bugId, string comment, string userId = "George Orwell")
        {
            _bugService.AddCommentToBug(bugId, userId, comment);
        }
    }
}