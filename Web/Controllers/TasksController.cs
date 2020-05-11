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
        public IActionResult Add(int? TaskID)
        {
            Models.TMT_Tasks Model = null;
            if (TaskID.HasValue)
            {
                Model = DB.TMT_Tasks.Find(TaskID);
                if (Model.Status != Models.DBEnums.TasksStatus.进行 && Model.CreateUserID != G.User.UserID)
                {
                    return NoPermission();
                }
            }
            ViewBag.Requirements = DB.TMT_Requirements.Where(c => c.ProjectID == G.NowProject.ProjectID && c.Status == Models.DBEnums.RequirementStatus.通过);
            ViewBag.IsDraftEdit = Test.Teaks.IsDraftEdit(Model);
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
                if (!Model.TaskID.HasValue)
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
                        RequirementID = Model.RequirementID,
                        Title = Model.Title,
                        EmergencyLevel = Model.EmergencyLevel,
                        MType = Model.MType.Value,
                        Status = Models.DBEnums.TasksStatus.稿件,
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
                    DB.TMT_Tasks.Add(Re);
                }
                else
                {
                    Re = DB.TMT_Tasks.Find(Model.TaskID);
                    if (!Test.Teaks.AllowEdit(Re, G.User.UserID))
                    {
                        return Json("当前文档状态不可编辑或没有编辑权限！");
                    }
                    if (Re.Status == Models.DBEnums.TasksStatus.稿件)
                    {
                        var Detaile = Re.Detailes.FirstOrDefault(c => c.TagID == Re.RequirementID && c.Version == Re.NowVersion);
                        if (Detaile != null)
                        {
                            Detaile.Content = string.IsNullOrWhiteSpace(Model.Content) ? "" : Model.Content;
                            Detaile.CreateDate = DateTime.Now;
                        }
                        Re.RequirementID = Model.RequirementID;
                        Re.Title = Model.Title;
                        Re.EmergencyLevel = Model.EmergencyLevel;
                        Re.MType = Model.MType.Value;
                        Re.ModuleID = Model.ModuleID;
                        Re.ExecutorUserID = Model.ExecutorUserID;
                    }
                    else if (Re.Status == Models.DBEnums.TasksStatus.进行)
                    {
                        Re.NowVersion += 1;
                        Re.Detailes.Add(new Models.TMT_Detaile
                        {
                            Content = string.IsNullOrWhiteSpace(Model.Content) ? "" : Model.Content,
                            Version = Re.NowVersion,
                            CreateDate = DateTime.Now
                        });
                        Re.Logs.Add(new Models.TMT_Logs
                        {
                            LogType = Models.DBEnums.LogType.任务,
                            TagID = Re.TaskID,
                            UserID = G.User.UserID,
                            Content = "[任务#" + Model.TaskID + "]更新至第<div class=\"ui label horizontal mini\">" + Re.NowVersion + "</div>版本！"
                        });
                    }
                    Re.LastUPDate = DateTime.Now;
                }
                DB.SaveChanges();
                return Success(Re.TaskID.ToString());
            }
            catch (Exception Ex)
            {
                return Json(Ex.Message);
            }
        }
        [HttpGet]
        public IActionResult View(int TaskID, int? Version)
        {
            var Model = DB.TMT_Tasks.Find(TaskID);
            if (!Test.Teaks.AllowView(Model, G.User.UserID))
            {
                return NoPermission();
            }
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
        public IActionResult Submit(int TaskID)
        {
            var Model = DB.TMT_Tasks.Find(TaskID);
            if (!Test.Teaks.AllowSubmitAndDelete(Model, G.User.UserID))
            {
                return Json("当前文档状态不可发布或没有发布权限！");
            }
            Model.Logs.Add(new Models.TMT_Logs
            {
                LogType = Models.DBEnums.LogType.任务,
                TagID = Model.TaskID,
                UserID = G.User.UserID,
                Content = "[任务#" + Model.TaskID + "]发布第<div class=\"ui label horizontal mini\">" + Model.NowVersion + "</div>版本！",
            });
            Model.Status = Models.DBEnums.TasksStatus.进行;
            DB.SaveChanges();
            return Json();
        }

        [HttpGet]
        public IActionResult HandOver(int TaskID)
        {
            ViewBag.TaskID = TaskID;
            return View();
        }
        [HttpPost]
        public IActionResult HandOver(int TaskID, int UserID, string Remark)
        {
            var Model = DB.TMT_Tasks.Find(TaskID);
            if (!Test.Teaks.AllowHandOverAndDone(Model, G.User.UserID))
            {
                return Json("当前文档状态不可转派或没有转派权限！");
            }
            if (UserID < 1)
            {
                return Json("转派负责人必须选择！");
            }
            if (Model.ExecutorUserID == UserID)
            {
                return Json("原负责人不能与转派负责人相同！");
            }
            var _User = DB.TMT_Users.Find(UserID);
            var Content = "[任务#" + Model.TaskID + "]转派<div class=\"ui label horizontal mini\">" + _User.UserName + "</div>负责！";
            if (!string.IsNullOrWhiteSpace(Remark))
            {
                Content += "<br />" + Remark;
            }
            Model.Logs.Add(new Models.TMT_Logs
            {
                LogType = Models.DBEnums.LogType.任务,
                TagID = Model.TaskID,
                UserID = G.User.UserID,
                Content = Content
            });
            Model.ExecutorUserID = UserID;
            DB.SaveChanges();
            return Json();
        }
        [HttpGet]
        public IActionResult Done(int TaskID)
        {
            ViewBag.TaskID = TaskID;
            return View();
        }
        [HttpPost]
        public IActionResult Done(int TaskID, bool Agree, string Remark)
        {
            var Model = DB.TMT_Tasks.Find(TaskID);
            if (!Test.Teaks.AllowHandOverAndDone(Model, G.User.UserID))
            {
                return Json("当前文档状态不可完成或没有完成权限！");
            }
            Model.Status = Agree ? Models.DBEnums.TasksStatus.完成 : Models.DBEnums.TasksStatus.终止;
            var Content = "[任务#" + Model.TaskID + "]完成，结果为：<div class=\"ui label horizontal mini\">" + (Agree ? "正常完成" : "异常终止") + "</div>！";
            if (!Agree && string.IsNullOrWhiteSpace(Remark))
            {
                return Json("任务异常终止时必须写明终止原因！");
            }
            if (!string.IsNullOrWhiteSpace(Remark))
            {
                Content += "<br />" + Remark;
            }
            Model.Logs.Add(new Models.TMT_Logs
            {
                LogType = Models.DBEnums.LogType.任务,
                TagID = Model.TaskID,
                UserID = G.User.UserID,
                Content = Content
            });
            DB.SaveChanges();
            return Json();
        }

        [HttpGet]
        public IActionResult AddLog(int TaskID)
        {
            ViewBag.TaskID = TaskID;
            return View();
        }
        [HttpPost]
        public IActionResult AddLog(int TaskID, string Remark)
        {
            var Model = DB.TMT_Tasks.Find(TaskID);
            if (!Test.Teaks.AllowAddLog(Model, G.User.UserID))
            {
                return Json("当前文档状态不可新增备注或没有新增备注权限！");
            }
            if (string.IsNullOrWhiteSpace(Remark))
            {
                return Json("[备注]必须填写！");
            }
            Remark = "[任务#" + Model.TaskID + "]备注：" + Remark;
            Model.Logs.Add(new Models.TMT_Logs
            {
                LogType = Models.DBEnums.LogType.任务,
                TagID = Model.TaskID,
                UserID = G.User.UserID,
                Content = Remark
            });
            DB.SaveChanges();
            return Json();
        }
        [HttpPost]
        public IActionResult Delete(int TaskID)
        {
            var Model = DB.TMT_Tasks.Find(TaskID);
            if (!Test.Teaks.AllowSubmitAndDelete(Model, G.User.UserID))
            {
                return Json("当前文档状态不可删除或没有删除权限！");
            }
            Model.Logs.Add(new Models.TMT_Logs
            {
                LogType = Models.DBEnums.LogType.任务,
                TagID = Model.TaskID,
                UserID = G.User.UserID,
                Content = "[任务#" + Model.TaskID + "]删除！",
            });
            Model.IsDelete = true;
            DB.SaveChanges();
            return Json();
        }


        [HttpGet]
        public IActionResult ViewRequirement(int RequirementID)
        {
            var Model = DB.TMT_Requirements.Find(RequirementID);
            return View(Model);
        }
        [HttpPost]
        public IActionResult LoadRequirement(int RequirementID, int Version)
        {
            var Model = DB.TMT_Requirements.Find(RequirementID);
            return Success(Model.Detailes.FirstOrDefault(c => c.Version == Version).Content);
        }
    }
}
namespace Web.Test
{
    public class Teaks
    {
        public static bool IsDraftEdit(Models.TMT_Tasks Model)
        {
            return Model == null || Model.Status == Models.DBEnums.TasksStatus.稿件;
        }
        public static bool AllowEdit(Models.TMT_Tasks Model, int UserID)
        {
            if (Model.CreateUserID != UserID)
            {
                return false;
            }
            var AllowStatus = new Models.DBEnums.TasksStatus[] {
            Models.DBEnums.TasksStatus.稿件,
            Models.DBEnums.TasksStatus.进行};
            if (!AllowStatus.Contains(Model.Status))
            {
                return false;
            }
            return true;
        }
        public static bool AllowView(Models.TMT_Tasks Model, int UserID)
        {
            if (Model.Status == Models.DBEnums.TasksStatus.稿件 && Model.CreateUserID == UserID)
            {
                return true;
            }
            var AllowStatus = new Models.DBEnums.TasksStatus[] {
            Models.DBEnums.TasksStatus.进行,
            Models.DBEnums.TasksStatus.终止,
            Models.DBEnums.TasksStatus.完成};
            if (AllowStatus.Contains(Model.Status))
            {
                return true;
            }
            return false;
        }
        public static bool AllowSubmitAndDelete(Models.TMT_Tasks Model, int UserID)
        {
            return Model.Status == Models.DBEnums.TasksStatus.稿件 && Model.CreateUserID == UserID;
        }
        public static bool AllowHandOverAndDone(Models.TMT_Tasks Model, int UserID)
        {
            return Model.Status == Models.DBEnums.TasksStatus.进行 && Model.ExecutorUserID == UserID;
        }
        public static bool AllowAddLog(Models.TMT_Tasks Model, int UserID)
        {
            return Model.Status == Models.DBEnums.TasksStatus.进行 && (Model.CreateUserID == UserID || Model.ExecutorUserID == UserID);
        }
    }
}