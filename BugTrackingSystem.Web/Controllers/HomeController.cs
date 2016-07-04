using System.Collections.Generic;
using System.Linq;
using System.Web.Mvc;
using AutoMapper;
using BugTrackingSystem.Data.Model;
using BugTrackingSystem.Service.Services;
using BugTrackingSystem.Web.Models;

namespace BugTrackingSystem.Web.Controllers
{
    public class HomeController : Controller
    {
        private readonly IUserService _userService;

        public HomeController(IUserService userService)
        {
            _userService = userService;
        }

        public ActionResult Index()
        {
            return View();
        }

        public ActionResult Dashboard()
        {
            //var config = new MapperConfiguration(cfg =>
            //{
            //    cfg.CreateMap<User, UserViewModel>()
            //        .ForMember(uvm => uvm.Projects, opt => opt.Ignore())
            //        .ForMember(uvm => uvm.Bugs, opt => opt.Ignore());
            //    cfg.CreateMap<Project, ProjectViewModel>()
            //        .ForMember(pvm => pvm.Bugs, opt => opt.Ignore())
            //        .ForMember(pvm => pvm.Users, opt => opt.Ignore());
            //    cfg.CreateMap<Bug, BugViewModel>();
            //});

            //var user = _userService.GetUserById(1);
            //var mapper = config.CreateMapper();
            //var userModel = mapper.Map<User,UserViewModel>(user);
            //userModel.Projects = user.Projects.Select(project => mapper.Map<Project, ProjectViewModel>(project)).ToList();
            //userModel.Bugs = user.Bugs.Select(project => mapper.Map<Bug, BugViewModel>(project)).ToList();

            var commentService = new CommentService();
            commentService.AddComment(1,"Admin","first comment");
            commentService.AddComment(1, "Admin", "second comment");
            var comment = commentService.GetCommentsForBug(1);

            return View();
        }
        public ActionResult MyProjects()
        {

            return PartialView();
        }

        //public ActionResult Login()
        //{

        //    return View();
        //}
        //public ActionResult ForgotPassword()
        //{

        //    return View();
        //}

        //public ActionResult ResetPassword()
        //{

        //    return View();
        //}

    }
}