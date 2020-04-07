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
            Model.List = DB.TMT_Requirements.Where(c => c.ProjectID == G.NowProject.ProjectID && c.Status == Models.DBEnums.RequirementStatus.通过 || c.Status == Models.DBEnums.RequirementStatus.归档 || c.CreateUserID == G.User.UserID || (c.Status == Models.DBEnums.RequirementStatus.待审 && c.AuditorUserID == G.User.UserID));
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
                if (Model.KeyWord.IndexOf("#") > -1 && int.TryParse(Model.KeyWord.Replace("#", ""), out int TID))
                {
                    Model.List = Model.List.Where(c => c.RequirementID == TID);
                }
                else
                {
                    Model.List = Model.List.Where(c => c.Title.Contains(Model.KeyWord));
                }
            }
            if (Model.ModuleID.HasValue)
            {
                Model.List = Model.List.Where(c => c.ModuleID == Model.ModuleID);
            }
            Model.List = Model.List.OrderByDescending(m => m.ProjectID);
            Model.Create();
            return View(Model);
        }

        public IActionResult Add(int? RequirementID)
        {
            Models.TMT_Requirements Model = null;
            if (RequirementID.HasValue)
            {
                Model = DB.TMT_Requirements.Find(RequirementID.Value);
                if (Model.Status != Models.DBEnums.RequirementStatus.草稿 && Model.Status != Models.DBEnums.RequirementStatus.拒绝 && Model.Status != Models.DBEnums.RequirementStatus.通过)
                {
                    throw new Exception("当前状态不能进行编辑操作!");
                }
                if (Model.CreateUserID != G.User.UserID)
                {
                    throw new Exception("无权");
                }
            }
            return View(Model);
        }
        public IActionResult View(int RequirementID)
        {
            var Model = DB.TMT_Requirements.Find(RequirementID);
            if ((Model.Status == Models.DBEnums.RequirementStatus.草稿 || Model.Status == Models.DBEnums.RequirementStatus.拒绝) && Model.CreateUserID != G.User.UserID)
            {
                throw new Exception("无权");
            }
            if (Model.Status == Models.DBEnums.RequirementStatus.待审 && (Model.CreateUserID != G.User.UserID && Model.AuditorUserID != G.User.UserID))
            {
                throw new Exception("无权");
            }
            return View(Model);
        }
        public Models.TMT_Requirements SaveModel(ParamModels.Requirement.Requirement_Add_Model Model)
        {
            if (string.IsNullOrWhiteSpace(Model.Title))
            {
                throw new Exception("[需求标题]不可为空！");
            }
            if (Model.ModuleID < 1)
            {
                throw new Exception("[模块]必须选择！");
            }
            if (Model.AuditorUserID < 1)
            {
                throw new Exception("[评审人]必须选择！");
            }
            if (!Model.RequirementID.HasValue)
            {
                Models.TMT_Requirements Re = new Models.TMT_Requirements
                {
                    Title = Model.Title,
                    EmergencyLevel = Model.EmergencyLevel,
                    Status = Models.DBEnums.RequirementStatus.草稿,
                    ProjectID = G.NowProject.ProjectID,
                    ModuleID = Model.ModuleID,
                    AuditorUserID = Model.AuditorUserID,
                    CreateUserID = G.User.UserID,
                    LastUPDate = DateTime.Now,
                    NowVersion = 1,
                    Detailes = new List<Models.TMT_Requirements_Detaile>()
                };
                Re.Detailes.Add(new Models.TMT_Requirements_Detaile()
                {
                    Content = string.IsNullOrWhiteSpace(Model.Content) ? "" : Model.Content,
                    Version = Re.NowVersion,
                    CreateDate = DateTime.Now
                });
                return Re;
            }
            else
            {
                var Re = DB.TMT_Requirements.Find(Model.RequirementID.Value);
                if (Re.CreateUserID != G.User.UserID)
                {
                    throw new Exception("无权对此数据进行编辑！");
                }
                if (Re.Status == Models.DBEnums.RequirementStatus.归档 || Re.Status == Models.DBEnums.RequirementStatus.待审)
                {
                    throw new Exception("此需求已归档或正在评审中，无法进行编辑！");
                }
                if (Re.Status == Models.DBEnums.RequirementStatus.草稿 || Re.Status == Models.DBEnums.RequirementStatus.拒绝)
                {
                    var Detaile = Re.Detailes.FirstOrDefault(c => c.Version == Re.NowVersion);
                    if (Detaile != null)
                    {
                        Detaile.Content = string.IsNullOrWhiteSpace(Model.Content) ? "" : Model.Content;
                        Detaile.CreateDate = DateTime.Now;
                    }
                }
                else if (Re.Status == Models.DBEnums.RequirementStatus.通过)
                {
                    Re.NowVersion += 1;
                    Re.Detailes.Add(new Models.TMT_Requirements_Detaile
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
                        Content = "将需求更新至第<div class=\"ui label horizontal mini\">" + Re.NowVersion + "</div>版本！",
                    });
                }
                Re.Title = Model.Title;
                Re.ModuleID = Model.ModuleID;
                Re.AuditorUserID = Model.AuditorUserID;
                Re.EmergencyLevel = Model.EmergencyLevel;
                Re.LastUPDate = DateTime.Now;
                return Re;
            }
        }

        [HttpPost]
        public IActionResult Save(ParamModels.Requirement.Requirement_Add_Model Model)
        {
            try
            {
                var Re = SaveModel(Model);
                if (!Model.RequirementID.HasValue)
                {
                    DB.TMT_Requirements.Add(Re);
                }
                DB.SaveChanges();
                return Json();
            }
            catch (Exception Ex)
            {
                return Json(Ex.Message);
            }
        }
        [HttpPost]
        public IActionResult Publish(ParamModels.Requirement.Requirement_Add_Model Model)
        {
            try
            {
                var Re = SaveModel(Model);
                if (!Model.RequirementID.HasValue)
                {
                    Re.Status = Models.DBEnums.RequirementStatus.待审;
                    Re.Logs.Add(new Models.TMT_Logs
                    {
                        TagID = Re.RequirementID,
                        LogType = Models.DBEnums.LogType.需求,
                        TriggerUserID = G.User.UserID,
                        TargetUserID = null,
                        Content = "发布第<div class=\"ui label horizontal mini\">" + Re.NowVersion + "</div>版本需求！",
                    });
                    DB.TMT_Requirements.Add(Re);
                }
                if (Re.Status == Models.DBEnums.RequirementStatus.草稿 || Re.Status == Models.DBEnums.RequirementStatus.拒绝)
                {
                    Re.Status = Models.DBEnums.RequirementStatus.待审;
                }
                DB.SaveChanges();
                return Json();
            }
            catch (Exception Ex)
            {
                return Json(Ex.Message);
            }
        }

        [HttpGet]
        public IActionResult Review(int RequirementID)
        {
            ViewBag.RequirementID = RequirementID;
            return View();
        }
        [HttpPost]
        public IActionResult Review(int RequirementID, bool Agree, string Remark)
        {
            var Model = DB.TMT_Requirements.Find(RequirementID);
            if (Model.Status != Models.DBEnums.RequirementStatus.待审)
            {
                return Json("当前状态不能进行评审!");
            }
            if (Model.AuditorUserID != G.User.UserID)
            {
                return Json("无权评审!");
            }
            Model.Status = Agree ? Models.DBEnums.RequirementStatus.通过 : Models.DBEnums.RequirementStatus.拒绝;
            var Content = "评审需求，评审结果为：<div class=\"ui label horizontal mini\">" + Model.Status + "</div>";
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
        public IActionResult SealUP(int RequirementID)
        {
            var Model = DB.TMT_Requirements.Find(RequirementID);
            if (Model.Status != Models.DBEnums.RequirementStatus.通过)
            {
                return Json("当前状态不能进行归档操作!");
            }
            if (Model.CreateUserID != G.User.UserID)
            {
                return Json("无权归档!");
            }
            Model.Status = Models.DBEnums.RequirementStatus.归档;
            DB.SaveChanges();
            return Json();
        }

        [HttpPost]
        public IActionResult Delete(int RequirementID)
        {
            var Model = DB.TMT_Requirements.Find(RequirementID);
            if (Model.Status != Models.DBEnums.RequirementStatus.草稿 && Model.Status != Models.DBEnums.RequirementStatus.拒绝)
            {
                return Json("当前状态不可删除!");
            }
            if (Model.CreateUserID != G.User.UserID)
            {
                return Json("无权删除!");
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
    }
}