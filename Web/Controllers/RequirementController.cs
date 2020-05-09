using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

namespace Web.Controllers
{
    [NeedSetProject]
    public class RequirementController : ControllersBase
    {
        public IActionResult Index(ParamModels.Requirement.Requirement_Index Model)
        {
            Model.List = DB.TMT_Requirements.Where(c => c.ProjectID == G.NowProject.ProjectID);
            Model.List = Model.List.Where(c => c.Status == Models.DBEnums.RequirementStatus.通过 || c.Status == Models.DBEnums.RequirementStatus.归档 || c.CreateUserID == G.User.UserID || c.AuditorUserID == G.User.UserID);
            if (Model.Tag == 1)
            {
                Model.List = Model.List.Where(c => c.CreateUserID == G.User.UserID);
            }
            if (Model.Tag == 2)
            {
                Model.List = Model.List.Where(c => c.AuditorUserID == G.User.UserID);
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
            Model.List = Model.List.OrderByDescending(m => m.RequirementID);
            Model.Create();
            return View(Model);
        }

        public IActionResult Add(string RequirementID)
        {
            Models.TMT_Requirements Model = null;
            if (!string.IsNullOrWhiteSpace(RequirementID))
            {
                Model = DB.TMT_Requirements.Find(RequirementID);
                if (!Test.Requirement.AllowEdit(Model, G.User.UserID))
                {
                    return View("_NoRight");
                }
            }
            return View(Model);
        }
        [HttpPost]
        public IActionResult Save(ParamModels.Requirement.Requirement_Add_Model Model)
        {
            try
            {
                Models.TMT_Requirements Re = null;
                if (string.IsNullOrWhiteSpace(Model.Title))
                {
                    return Json("[需求标题]不可为空！");
                }
                if (string.IsNullOrWhiteSpace(Model.RequirementID))
                {
                    if (!Model.MType.HasValue)
                    {
                        return Json("[类型]必须选择！");
                    }
                    if (Model.ModuleID < 1)
                    {
                        return Json("[模块]必须选择！");
                    }
                    if (Model.AuditorUserID < 1)
                    {
                        return Json("[评审人]必须选择！");
                    }
                    Re = new Models.TMT_Requirements
                    {
                        RequirementID = Guid.NewGuid().ToString("N"),
                        Title = Model.Title,
                        EmergencyLevel = Model.EmergencyLevel,
                        MType = Model.MType.Value,
                        Status = Models.DBEnums.RequirementStatus.稿件,
                        ProjectID = G.NowProject.ProjectID,
                        ModuleID = Model.ModuleID,
                        AuditorUserID = Model.AuditorUserID,
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
                    DB.TMT_Requirements.Add(Re);
                }
                else
                {
                    Re = DB.TMT_Requirements.Find(Model.RequirementID);
                    if (!Test.Requirement.AllowEdit(Re, G.User.UserID))
                    {
                        return Json("当前文档状态不可编辑或没有编辑权限！");
                    }
                    if (Re.Status == Models.DBEnums.RequirementStatus.驳回)
                    {
                        var Detaile = Re.Detailes.FirstOrDefault(c => c.Version == Re.NowVersion);
                        if (Detaile != null)
                        {
                            Detaile.Content = string.IsNullOrWhiteSpace(Model.Content) ? "" : Model.Content;
                            Detaile.CreateDate = DateTime.Now;
                        }
                        Re.Title = Model.Title;
                        Re.EmergencyLevel = Model.EmergencyLevel;
                        Re.AuditorUserID = Model.AuditorUserID;
                        Re.MType = Model.MType.Value;
                        Re.ModuleID = Model.ModuleID;
                    }
                    else if (Re.Status == Models.DBEnums.RequirementStatus.通过)
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
                            TagID = Re.RequirementID,
                            UserID = G.User.UserID,
                            Content = "需求更新至第<div class=\"ui label horizontal mini\">" + Re.NowVersion + "</div>版本！",
                        });
                    }
                    Re.LastUPDate = DateTime.Now;
                }
                DB.SaveChanges();
                return Success(Re.RequirementID);
            }
            catch (Exception Ex)
            {
                return Json(Ex.Message);
            }
        }

        public IActionResult View(string RequirementID, int? Version)
        {
            var Model = DB.TMT_Requirements.Find(RequirementID);
            if (!Test.Requirement.AllowView(Model, G.User.UserID))
            {
                return View("_NoRight");
            }
            foreach (var item in Model.Tasks)
            {
                Model.Logs = Model.Logs.Union(item.Logs).ToList();
            }
            if (Version.HasValue)
            {
                Model.NowVersion = Version.Value;
            }
            return View(Model);
        }

        [HttpPost]
        public IActionResult Submit(string RequirementID)
        {
            var Model = DB.TMT_Requirements.Find(RequirementID);
            if (!Test.Requirement.AllowSubmit(Model, G.User.UserID))
            {
                return Json("当前文档状态不可发布或没有发布权限！");
            }
            Model.Logs.Add(new Models.TMT_Logs
            {
                TagID = Model.RequirementID,
                UserID = G.User.UserID,
                Content = "提交审核，当前文档版本<div class=\"ui label horizontal mini\">" + Model.NowVersion + "</div>！",
            });
            Model.Status = Models.DBEnums.RequirementStatus.待审;
            DB.SaveChanges();
            return Json();
        }

        [HttpGet]
        public IActionResult Review(string RequirementID)
        {
            ViewBag.RequirementID = RequirementID;
            return View();
        }
        [HttpPost]
        public IActionResult Review(string RequirementID, bool Agree, string Remark)
        {
            var Model = DB.TMT_Requirements.Find(RequirementID);
            if (!Test.Requirement.AllowReview(Model, G.User.UserID))
            {
                return Json("当前文档状态不可审核或没有审核权限！");
            }
            Model.Status = Agree ? Models.DBEnums.RequirementStatus.通过 : Models.DBEnums.RequirementStatus.驳回;
            var Content = "评审需求，评审结果为：<div class=\"ui label horizontal mini\">" + Model.Status + "</div>";
            if (!Agree && string.IsNullOrWhiteSpace(Remark))
            {
                return Json("需求被驳回时必须写明驳回原因（评审备注）！");
            }
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
            DB.SaveChanges();
            return Json();
        }

        [HttpPost]
        public IActionResult SealUP(string RequirementID)
        {
            var Model = DB.TMT_Requirements.Find(RequirementID);
            if (!Test.Requirement.AllowSealUP(Model, G.User.UserID))
            {
                return Json("当前文档状态不可归档或没有归档权限！");
            }
            Model.Logs.Add(new Models.TMT_Logs
            {
                TagID = Model.RequirementID,
                UserID = G.User.UserID,
                Content = "归档需求！",
            });
            Model.Status = Models.DBEnums.RequirementStatus.归档;
            DB.SaveChanges();
            return Json();
        }

        [HttpGet]
        public IActionResult HandOver(string RequirementID)
        {
            ViewBag.RequirementID = RequirementID;
            return View();
        }
        [HttpPost]
        public IActionResult HandOver(string RequirementID, int UserID, string Remark)
        {
            var Model = DB.TMT_Requirements.Find(RequirementID);
            if (!Test.Requirement.AllowReview(Model, G.User.UserID))
            {
                return Json("当前文档状态不可转交审核或没有转交审核权限！");
            }
            if (Model.AuditorUserID == UserID)
            {
                return Json("原评审人不能与转交审核人相同！");
            }
            var _User = DB.TMT_Users.Find(UserID);
            var Content = "需求转交给<div class=\"ui label horizontal mini\">" + _User.UserName + "</div>进行评审！";
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
            Model.AuditorUserID = UserID;
            DB.SaveChanges();
            return Json();
        }

        [HttpGet]
        public IActionResult AddLog(string RequirementID)
        {
            ViewBag.RequirementID = RequirementID;
            return View();
        }
        [HttpPost]
        public IActionResult AddLog(string RequirementID, string Remark)
        {
            var Model = DB.TMT_Requirements.Find(RequirementID);
            if (!Test.Requirement.AllowAddLog(Model, G.User.UserID))
            {
                return Json("当前文档状态不可新增备注或没有新增备注权限！");
            }
            if (string.IsNullOrWhiteSpace(Remark))
            {
                return Json("必须填写[备注]！");
            }
            Model.Logs.Add(new Models.TMT_Logs
            {
                TagID = Model.RequirementID,
                UserID = G.User.UserID,
                Content = Remark
            });
            DB.SaveChanges();
            return Json();
        }

        [HttpPost]
        public IActionResult Delete(string RequirementID)
        {
            var Model = DB.TMT_Requirements.Find(RequirementID);
            if (!Test.Requirement.AllowSubmit(Model, G.User.UserID))
            {
                return Json("当前文档状态不可删除或没有删除权限！");
            }
            Model.Logs.Add(new Models.TMT_Logs
            {
                TagID = Model.RequirementID,
                UserID = G.User.UserID,
                Content = "删除需求！",
            });
            Model.IsDelete = true;
            DB.SaveChanges();
            return Json();
        }
    }
}
namespace Web.Test
{
    public class Requirement
    {
        public static bool AllowEdit(Models.TMT_Requirements Model, int UserID)
        {
            if (Model.CreateUserID != UserID)
            {
                return false;
            }
            var AllowStatus = new Models.DBEnums.RequirementStatus[] {
            Models.DBEnums.RequirementStatus.稿件,
            Models.DBEnums.RequirementStatus.驳回,
            Models.DBEnums.RequirementStatus.通过};
            if (!AllowStatus.Contains(Model.Status))
            {
                return false;
            }
            return true;
        }
        public static bool AllowView(Models.TMT_Requirements Model, int UserID)
        {
            if (Model.Status == Models.DBEnums.RequirementStatus.稿件 && Model.CreateUserID == UserID)
            {
                return true;
            }
            var AllowStatus = new Models.DBEnums.RequirementStatus[] {
            Models.DBEnums.RequirementStatus.待审,
            Models.DBEnums.RequirementStatus.驳回,
            Models.DBEnums.RequirementStatus.通过,
            Models.DBEnums.RequirementStatus.归档};
            if (AllowStatus.Contains(Model.Status))
            {
                return true;
            }
            return false;
        }
        public static bool AllowSubmit(Models.TMT_Requirements Model, int UserID)
        {
            if ((Model.Status == Models.DBEnums.RequirementStatus.驳回 || Model.Status == Models.DBEnums.RequirementStatus.稿件) && Model.CreateUserID == UserID)
            {
                return true;
            }
            return false;
        }
        public static bool AllowReview(Models.TMT_Requirements Model, int UserID)
        {
            if (Model.Status == Models.DBEnums.RequirementStatus.待审 && Model.AuditorUserID == UserID)
            {
                return true;
            }
            return false;
        }
        public static bool AllowSealUP(Models.TMT_Requirements Model, int UserID)
        {
            if (Model.Status == Models.DBEnums.RequirementStatus.通过 && Model.AuditorUserID == UserID)
            {
                return true;
            }
            return false;
        }
        public static bool AllowAddLog(Models.TMT_Requirements Model, int UserID)
        {
            if (Model.Status == Models.DBEnums.RequirementStatus.通过 && (Model.CreateUserID == UserID || Model.AuditorUserID == UserID))
            {
                return true;
            }
            return false;
        }
    }
}