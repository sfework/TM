using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;

namespace Web.Controllers
{
    [NeedSetProject]
    public class TasksController : ControllersBase
    {
        public IActionResult Index(ParamModels.Tasks.Tasks_Index Model)
        {
            Model.List = DB.TMT_Tasks.Where(c => c.ProjectID == G.NowProject.ProjectID);
            if (Model.Tag == 1)
            {
                Model.List = Model.List.Where(c => c.CreateUserID == G.User.UserID);
            }
            if (Model.Tag == 2)
            {
                Model.List = Model.List.Where(c => c.ExecutorUserID == G.User.UserID);
            }
            if (Model.Status.HasValue)
            {
                Model.List = Model.List.Where(c => c.Status == Model.Status.Value);
            }
            if (Model.MType.HasValue)
            {
                Model.List = Model.List.Where(c => c.MType == Model.MType.Value);
            }
            if (!string.IsNullOrWhiteSpace(Model.KeyWord))
            {
                Model.List = Model.List.Where(c => c.Title.Contains(Model.KeyWord));
            }
            if (Model.ModuleID.HasValue)
            {
                Model.List = Model.List.Where(c => c.ModuleID == Model.ModuleID);
            }
            Model.List = Model.List.OrderByDescending(m => m.TaskID);
            Model.Create();
            return View(Model);
        }
        [HttpGet]
        public IActionResult Add(string TaskID)
        {
            Models.TMT_Tasks Model = null;
            if (!string.IsNullOrWhiteSpace(TaskID))
            {
                Model = DB.TMT_Tasks.Find(TaskID);
                if (Model.Status != Models.DBEnums.TasksStatus.进行 && Model.CreateUserID != G.User.UserID)
                {
                    return View("_NoRight");
                }
            }
            ViewBag.Requirements = DB.TMT_Requirements.Where(c => c.ProjectID == G.NowProject.ProjectID && c.Status == Models.DBEnums.RequirementStatus.通过);
            return View(Model);
        }

        [HttpGet]
        public IActionResult View(string TaskID, int? Version)
        {
            var Model = DB.TMT_Tasks.Find(TaskID);
            if (Version.HasValue)
            {
                Model.NowVersion = Version.Value;
            }
            if (Model.Requirement != null)
            {
                Model.Logs = Model.Logs.Union(Model.Requirement.Logs).ToList();
            }
            return View(Model);
        }
        [HttpPost]
        public IActionResult Save(ParamModels.Tasks.Tasks_Add Model)
        {
            try
            {
                Models.TMT_Tasks Re = null;
                if (string.IsNullOrWhiteSpace(Model.Title))
                {
                    return Json("[任务标题]不可为空！");
                }
                if (string.IsNullOrWhiteSpace(Model.TaskID))
                {
                    if (!Model.MType.HasValue)
                    {
                        return Json("[类型]必须选择！");
                    }
                    if (Model.ModuleID < 1)
                    {
                        return Json("[模块]必须选择！");
                    }
                    if (Model.ExecutorUserID < 1)
                    {
                        return Json("[负责人]必须选择！");
                    }
                    Re = new Models.TMT_Tasks
                    {
                        TaskID = Guid.NewGuid().ToString("N"),
                        RequirementID = Model.RequirementID,
                        Title = Model.Title,
                        EmergencyLevel = Model.EmergencyLevel,
                        MType=Model.MType.Value,
                        Status = Models.DBEnums.TasksStatus.进行,
                        ProjectID = G.NowProject.ProjectID,
                        ModuleID = Model.ModuleID,
                        ExecutorUserID = Model.ExecutorUserID,
                        CreateUserID = G.User.UserID,
                        LastUPDate = DateTime.Now,
                        NowVersion = 1,
                        Detailes = new List<Models.TMT_Detaile>(),
                        Logs = new List<Models.TMT_Logs>()
                    };
                    Re.Detailes.Add(new Models.TMT_Detaile()
                    {
                        Content = string.IsNullOrWhiteSpace(Model.Content) ? "" : Model.Content,
                        Version = Re.NowVersion,
                        CreateDate = DateTime.Now
                    });
                    Re.Logs.Add(new Models.TMT_Logs
                    {
                        TagID = Re.RequirementID,
                        UserID = G.User.UserID,
                        Content = "创建第<div class=\"ui label horizontal mini\">" + Re.NowVersion + "</div>版本任务！",
                    });
                    DB.TMT_Tasks.Add(Re);
                }
                else
                {
                    Re = DB.TMT_Tasks.Find(Model.TaskID);
                    if (Re.CreateUserID != G.User.UserID)
                    {
                        return Json("无权对此任务进行编辑操作！");
                    }
                    if (Re.Status == Models.DBEnums.TasksStatus.完成 || Re.Status == Models.DBEnums.TasksStatus.取消)
                    {
                        return Json("任务已完成或已取消，无法对此任务进行编辑操作！");
                    }
                    Re.NowVersion += 1;
                    Re.Detailes.Add(new Models.TMT_Detaile
                    {
                        Content = string.IsNullOrWhiteSpace(Model.Content) ? "" : Model.Content,
                        Version = Re.NowVersion,
                        CreateDate = DateTime.Now
                    });
                    Re.Logs.Add(new Models.TMT_Logs
                    {
                        TagID = Re.RequirementID,
                        UserID = G.User.UserID,
                        Content = "任务更新至第<div class=\"ui label horizontal mini\">" + Re.NowVersion + "</div>版本！",
                    });
                    Re.LastUPDate = DateTime.Now;
                }
                DB.SaveChanges();
                return Success(Re.TaskID);
            }
            catch (Exception Ex)
            {
                return Json(Ex.Message);
            }
        }

        [HttpPost]
        public IActionResult GetRequirement(string RequirementID)
        {
            var Model = DB.TMT_Requirements.Find(RequirementID);
            return Json(new { Model.Title, Model.EmergencyLevel, Model.ModuleID });
        }


        [HttpGet]
        public IActionResult HandOver(string TaskID)
        {
            ViewBag.TaskID = TaskID;
            return View();
        }
        [HttpPost]
        public IActionResult HandOver(string TaskID, int UserID, string Remark)
        {
            var Model = DB.TMT_Tasks.Find(TaskID);
            if (Model.Status != Models.DBEnums.TasksStatus.进行)
            {
                return Json("当前任务已完成，不能进行任务转交!");
            }
            if (Model.ExecutorUserID != G.User.UserID)
            {
                return Json("无权转交!");
            }
            var _User = DB.TMT_Users.Find(UserID);
            var Content = "将任务转交由：<div class=\"ui label horizontal mini\">" + _User.UserName + "</div>负责。";
            if (!string.IsNullOrWhiteSpace(Remark))
            {
                Content += "<br />" + Remark;
            }
            Model.Logs.Add(new Models.TMT_Logs
            {
                TagID = Model.RequirementID,
                UserID = G.User.UserID,
                Content = Content
            });
            Model.ExecutorUserID = UserID;
            DB.SaveChanges();
            return Json();
        }
        [HttpGet]
        public IActionResult Done(string TaskID)
        {
            ViewBag.TaskID = TaskID;
            return View();
        }
        [HttpPost]
        public IActionResult Done(string TaskID, string Remark)
        {
            var Model = DB.TMT_Tasks.Find(TaskID);
            if (Model.Status != Models.DBEnums.TasksStatus.进行)
            {
                return Json("当前任务已完成，不能再次完成!");
            }
            if (Model.ExecutorUserID != G.User.UserID)
            {
                return Json("无权完整任务!");
            }
            var Content = "完成任务！";
            if (!string.IsNullOrWhiteSpace(Remark))
            {
                Content += "<br />" + Remark;
            }
            Model.Logs.Add(new Models.TMT_Logs
            {
                TagID = Model.RequirementID,
                UserID = G.User.UserID,
                Content = Content
            });
            Model.Status = Models.DBEnums.TasksStatus.完成;
            DB.SaveChanges();
            return Json();
        }

        [HttpGet]
        public IActionResult Cancel(string TaskID)
        {
            ViewBag.TaskID = TaskID;
            return View();
        }
        [HttpPost]
        public IActionResult Cancel(string TaskID, string Remark)
        {
            var Model = DB.TMT_Tasks.Find(TaskID);
            if (Model.Status != Models.DBEnums.TasksStatus.进行)
            {
                return Json("当前状态不能取消任务!");
            }
            if (Model.ExecutorUserID != G.User.UserID)
            {
                return Json("无权取消任务!");
            }
            var Content = "取消任务！";
            if (!string.IsNullOrWhiteSpace(Remark))
            {
                Content += "<br />" + Remark;
            }
            Model.Logs.Add(new Models.TMT_Logs
            {
                TagID = Model.RequirementID,
                UserID = G.User.UserID,
                Content = Content
            });
            Model.Status = Models.DBEnums.TasksStatus.取消;
            DB.SaveChanges();
            return Json();
        }
    }
}