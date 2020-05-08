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
            Model.List = Model.List.Where(c => c.Status == Models.DBEnums.RequirementStatus.通过 || c.Status == Models.DBEnums.RequirementStatus.归档 || c.Status == Models.DBEnums.RequirementStatus.取消 || c.CreateUserID == G.User.UserID || c.AuditorUserID == G.User.UserID);
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
                if (Model.CreateUserID != G.User.UserID)
                {
                    return View("_NoRight");
                }
            }
            return View(Model);
        }
        public IActionResult View(string RequirementID, int? Version)
        {
            var Model = DB.TMT_Requirements.Find(RequirementID);
            if ((Model.Status == Models.DBEnums.RequirementStatus.待审 || Model.Status == Models.DBEnums.RequirementStatus.拒绝) && Model.CreateUserID != G.User.UserID && Model.AuditorUserID != G.User.UserID)
            {
                return View("_NoRight");
            }
            if (Version.HasValue)
            {
                Model.NowVersion = Version.Value;
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
                        Status = Models.DBEnums.RequirementStatus.待审,
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
                    Re.Logs.Add(new Models.TMT_Logs
                    {
                        TagID = Re.RequirementID,
                        LogType = Models.DBEnums.LogType.需求,
                        TriggerUserID = G.User.UserID,
                        TargetUserID = null,
                        Content = "发布第<div class=\"ui label horizontal mini basic\">" + Re.NowVersion + "</div>版本需求，等待审核！",
                    });
                    DB.TMT_Requirements.Add(Re);
                }
                else
                {
                    Re = DB.TMT_Requirements.Find(Model.RequirementID);
                    if (Re.CreateUserID != G.User.UserID)
                    {
                        return Json("无权对此需求进行编辑操作！");
                    }
                    if (Re.Status == Models.DBEnums.RequirementStatus.拒绝)
                    {
                        var Detaile = Re.Detailes.FirstOrDefault(c => c.Version == Re.NowVersion);
                        if (Detaile != null)
                        {
                            Detaile.Content = string.IsNullOrWhiteSpace(Model.Content) ? "" : Model.Content;
                            Detaile.CreateDate = DateTime.Now;
                        }
                        Re.Status = Models.DBEnums.RequirementStatus.待审;
                        Re.Title = Model.Title;
                        Re.EmergencyLevel = Model.EmergencyLevel;
                        Re.AuditorUserID = Model.AuditorUserID;
                        Re.ModuleID = Model.ModuleID;
                        Re.Logs.Add(new Models.TMT_Logs
                        {
                            TagID = Re.RequirementID,
                            LogType = Models.DBEnums.LogType.需求,
                            TriggerUserID = G.User.UserID,
                            TargetUserID = null,
                            Content = "编辑后重新提交需求，等待审核！",
                        });
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
                            LogType = Models.DBEnums.LogType.需求,
                            TriggerUserID = G.User.UserID,
                            TargetUserID = null,
                            Content = "将需求更新至第<div class=\"ui label horizontal mini basic\">" + Re.NowVersion + "</div>版本！",
                        });
                    }
                    else
                    {
                        return Json("当前需求状态无法进行更新发布！");
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
            if (Model.Status != Models.DBEnums.RequirementStatus.待审)
            {
                return Json("当前需求状态不能进行评审操作！");
            }
            if (Model.AuditorUserID != G.User.UserID)
            {
                return Json("无权对此需求进行评审操作！");
            }
            Model.Status = Agree ? Models.DBEnums.RequirementStatus.通过 : Models.DBEnums.RequirementStatus.拒绝;
            var Content = "评审需求，评审结果为：<div class=\"ui label horizontal mini basic\">" + Model.Status + "</div>";
            if (!string.IsNullOrWhiteSpace(Remark))
            {
                Content += "<br />" + Remark;
            }
            Model.Logs.Add(new Models.TMT_Logs
            {
                TagID = Model.RequirementID,
                LogType = Models.DBEnums.LogType.需求,
                TriggerUserID = G.User.UserID,
                TargetUserID = null,
                Content = Content
            });
            DB.SaveChanges();
            return Json();
        }

        [HttpPost]
        public IActionResult SealUP(string RequirementID)
        {
            var Model = DB.TMT_Requirements.Find(RequirementID);
            if (Model.Status != Models.DBEnums.RequirementStatus.通过 && Model.Status != Models.DBEnums.RequirementStatus.取消)
            {
                return Json("当前需求状态不能进行归档操作！");
            }
            if (Model.AuditorUserID != G.User.UserID)
            {
                return Json("无权对此需求进行归档操作！");
            }
            Model.Logs.Add(new Models.TMT_Logs
            {
                TagID = Model.RequirementID,
                LogType = Models.DBEnums.LogType.需求,
                TriggerUserID = G.User.UserID,
                TargetUserID = null,
                Content = "归档需求，锁定并存档！",
            });
            Model.Status = Models.DBEnums.RequirementStatus.归档;
            DB.SaveChanges();
            return Json();
        }

        [HttpPost]
        public IActionResult Delete(string RequirementID)
        {
            var Model = DB.TMT_Requirements.Find(RequirementID);
            if (Model.Status != Models.DBEnums.RequirementStatus.拒绝)
            {
                return Json("当前需求状态不能进行删除操作！");
            }
            if (Model.CreateUserID != G.User.UserID)
            {
                return Json("无权对此需求进行删除操作！");
            }
            Model.Logs.Add(new Models.TMT_Logs
            {
                TagID = Model.RequirementID,
                LogType = Models.DBEnums.LogType.需求,
                TriggerUserID = G.User.UserID,
                TargetUserID = null,
                Content = "删除需求！",
            });
            Model.IsDelete = true;
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
            if (Model.Status != Models.DBEnums.RequirementStatus.待审)
            {
                return Json("当前需求状态不能进行转交操作！");
            }
            if (Model.AuditorUserID != G.User.UserID)
            {
                return Json("无权对此需求进行转交操作！");
            }
            if (Model.AuditorUserID == UserID)
            {
                return Json("原评审人不能与转交审核人相同！");
            }
            var _User = DB.TMT_Users.Find(UserID);
            var Content = "需求转交给：<div class=\"ui label horizontal mini basic\">" + _User.UserName + "</div>进行评审！";
            if (!string.IsNullOrWhiteSpace(Remark))
            {
                Content += "<br />" + Remark;
            }
            Model.Logs.Add(new Models.TMT_Logs
            {
                TagID = Model.RequirementID,
                LogType = Models.DBEnums.LogType.需求,
                TriggerUserID = G.User.UserID,
                TargetUserID = null,
                Content = Content
            });
            Model.AuditorUserID = UserID;
            DB.SaveChanges();
            return Json();
        }

        [HttpGet]
        public IActionResult Cancel(string RequirementID)
        {
            ViewBag.RequirementID = RequirementID;
            return View();
        }
        [HttpPost]
        public IActionResult Cancel(string RequirementID, string Remark)
        {
            var Model = DB.TMT_Requirements.Find(RequirementID);
            if (Model.Status != Models.DBEnums.RequirementStatus.通过)
            {
                return Json("当前需求状态不能进行取消操作！");
            }
            if (Model.CreateUserID != G.User.UserID)
            {
                return Json("无权对此需求进行取消操作！");
            }
            var Content = "取消需求，不再继续执行！";
            if (!string.IsNullOrWhiteSpace(Remark))
            {
                Content += "<br />" + Remark;
            }
            Model.Logs.Add(new Models.TMT_Logs
            {
                TagID = Model.RequirementID,
                LogType = Models.DBEnums.LogType.需求,
                TriggerUserID = G.User.UserID,
                TargetUserID = null,
                Content = Content
            });
            Model.Status = Models.DBEnums.RequirementStatus.取消;
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
            Model.Logs.Add(new Models.TMT_Logs
            {
                TagID = Model.RequirementID,
                LogType = Models.DBEnums.LogType.需求,
                TriggerUserID = G.User.UserID,
                TargetUserID = null,
                Content = Remark
            });
            DB.SaveChanges();
            return Json();
        }
    }
}