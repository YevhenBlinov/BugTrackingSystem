using System;
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
        public void CreateTask(BugFormViewModel bug, HttpPostedFileBase[] image)
        {
            byte[][] data = new byte[image.Length][];
            for (int i = 0; i < image.Length; i++)
            {
                using (Stream inputStream = image[i].InputStream)
                {
                    MemoryStream memoryStream = inputStream as MemoryStream;
                    if (memoryStream == null)
                    {
                        memoryStream = new MemoryStream();
                        inputStream.CopyTo(memoryStream);
                    }
                    data[i] = memoryStream.ToArray();
                }
            }
            
            bug.Attachments = data;
            throw new NotImplementedException();
        }

    }

    public class MyClass
    {
        public string Title { get; set; }

        public string Project { get; set; }

        public string Assignee { get; set; }

        public string Priority { get; set; }

        public string Status { get; set; }

        public string Description { get; set; }
    }
}