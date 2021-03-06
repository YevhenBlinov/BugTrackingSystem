﻿using System;
using System.Collections.Generic;
using System.IO;
using System.Web;
using System.Web.Mvc;
using BugTrackingSystem.Service.Models.FormModels;
using BugTrackingSystem.Service.Services;
using BugTrackingSystem.Web.Filters;

namespace BugTrackingSystem.Web.Controllers
{
    [CustomAuthenticate]
    public class TaskController : Controller
    {
        private readonly IBugService _bugService;
        private readonly IProjectService _projectService;
        private readonly IUserService _userService;

        public TaskController(IBugService bugService, IUserService userService, IProjectService projectService)
        {
            _bugService = bugService;
            _projectService = projectService;
            _userService = userService;
        }

        //
        // GET: /Task/
        [Route("Task/{bugId?}")]
        public ActionResult Task(int bugId = 1)
        {
            var userId = (int) Session["UserId"];
            if (User.IsInRole("User"))
            {
                //_userService.GetUsersBugs()
            }

            var allBugsCount = 0;
            _userService.GetUsersBugs(userId, out allBugsCount);
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
            int bugId = _bugService.AddNewBug(bug);
            return RedirectToActionPermanent("Task", "Task", new { bugId = bugId });
        }

        public ActionResult EditTask(BugEditFormViewModel bug, HttpPostedFileBase[] attachmentsModal)
        {
            if (attachmentsModal[0] != null)
            {
                bug.Attachments = new Dictionary<string, byte[]>();

                foreach (HttpPostedFileBase item in attachmentsModal)
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
            _bugService.EditBug(bug);
            return RedirectToActionPermanent("Task", "Task", new { bugId = bug.BugId });
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

        public void AddComment(int bugId, string comment, string userName)
        {
            userName = Session["FirstName"] + " " + Session["LastName"];
            _bugService.AddCommentToBug(bugId, userName, comment);
        }

        public ActionResult EditTaskUsers(int projectId)
        {
            var users = _projectService.GetAllProjectUsers(projectId);
            return PartialView(users);
        }

        public ActionResult DeleteAttachment(int bugId, string name)
        {
            _bugService.DeleteBugAttachment(bugId, name);
            return RedirectToActionPermanent("BugAttachments", "Task", new { bugId = bugId });
        }

        public ActionResult BugAttachments(int bugId)
        {
            var attachments = _bugService.GetBugAttachmentsByBugId(bugId);
            return PartialView(attachments);
        }

        public ActionResult AddAttachment(HttpPostedFileBase[] attachments, int bugId)
        {
            if (attachments[0] != null)
            {
                Dictionary<string, byte[]> attachmentsArray = new Dictionary<string, byte[]>();

                foreach (HttpPostedFileBase item in attachments)
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
                        attachmentsArray.Add(item.FileName, data);
                    }
                }
                _bugService.AddBugAttachmentsByBugId(bugId, attachmentsArray);
            }
            return RedirectToActionPermanent("Task", "Task", new { bugId = bugId });
        }
        public ActionResult TaskAssignedUser(int userId)
        {
            var user = _userService.GetUserById(userId);
            return PartialView(user);
        }

        public ActionResult AssignTaskToMe(int bugId)
        {
            var userId = Convert.ToInt32(Session["UserId"]);
            _bugService.AssigneeUserToBug(bugId, userId);
            return RedirectToAction("TaskAssignedUser", "Task", new { userId = userId });
        }
    }
}