using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

namespace Web.Controllers
{
    public class RequirementController : ControllersBase
    {
        public IActionResult Index(ParamModels.Requirement.Requirement_Index Model)
        {
            Model.Modules = DB.TMT_Modules.Where(c => c.ProjectID == User.DefaultProjectID);
            Model.List = DB.TMT_Requirements.Where(c => c.ProjectID == User.DefaultProjectID);
            if (Model.Tag == 1)
            {
                Model.List = Model.List.Where(c => c.CreateUserID == User.UserID);
            }
            if (Model.Tag == 2)
            {
                Model.List = Model.List.Where(c => c.AuditorUserID == User.UserID);
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
            Model.List = Model.List.OrderByDescending(m => m.ProjectID);
            Model.Create();
            return View(Model);
        }
        /// <summary>
        /// 
        /// </summary>
        /// <param name="RequirementID"></param>
        /// <param name="TP">0为新增，1为查看，2为编辑</param>
        /// <returns></returns>
        [HttpGet]
        public IActionResult View(int? RequirementID, int TP = 0)
        {
            var Project = DB.TMT_Projects.Find(User.DefaultProjectID);
            ParamModels.Requirement.Requirement_Model Model = new ParamModels.Requirement.Requirement_Model
            {
                TP = TP,
                Requirement = RequirementID.HasValue ? DB.TMT_Requirements.Find(RequirementID) : null,
                Users = DB.TMT_Users.FromSqlRaw(string.Format("select * from TMT_Users where UserID in ({0})", Project.Users)),
                Modules = DB.TMT_Modules.Where(c => c.ProjectID == Project.ProjectID)
            };
            return View(Model);
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="RequirementID"></param>
        /// <param name="Title"></param>
        /// <param name="ModuleID"></param>
        /// <param name="AuditorUserID"></param>
        /// <param name="EmergencyLevel"></param>
        /// <param name="Content"></param>
        /// <param name="SaveType">0为保存，1为发布，如果已发布此值无效</param>
        /// <returns></returns>
        [HttpPost]
        public IActionResult Add(ParamModels.Requirement.Requirement_Add_Model Model)
        {
            if (string.IsNullOrWhiteSpace(Model.Title))
            {
                return Json("[需求标题]不可为空！");
            }
            if (Model.ModuleID < 1)
            {
                return Json("[所属模块]必须选择！");
            }
            if (Model.AuditorUserID < 1)
            {
                return Json("[评审人]必须选择！");
            }
            if (!Model.RequirementID.HasValue)
            {
                Models.TMT_Requirements Req = new Models.TMT_Requirements
                {
                    Title = Model.Title,
                    EmergencyLevel = Model.EmergencyLevel,
                    Status = Model.SaveType == 0 ? Models.DBEnums.RequirementStatus.草稿 : Models.DBEnums.RequirementStatus.待审,
                    ProjectID = User.DefaultProjectID,
                    ModuleID = Model.ModuleID,
                    AuditorUserID = Model.AuditorUserID,
                    CreateDate = DateTime.Now,
                    CreateUserID = User.UserID,
                    NowVersion = 1,
                    Detailes = new List<Models.TMT_Requirements_Detaile>()
                };
                Req.Detailes.Add(new Models.TMT_Requirements_Detaile()
                {
                    Content = string.IsNullOrWhiteSpace(Model.Content) ? "" : Model.Content,
                    Version = Req.NowVersion
                });
                DB.TMT_Requirements.Add(Req);
                if (Req.Status == Models.DBEnums.RequirementStatus.待审)
                {
                    //日志，发布第一版本
                    DB.TMT_Logs.Add(new Models.TMT_Logs()
                    {
                        TagID = Req.RequirementID,
                        LogType = Models.DBEnums.LogType.需求,
                        TriggerUserID = User.UserID,
                        TargetUserID = null,
                        Content = "发布需求：版本",
                        CreateDate = DateTime.Now
                    });
                }
            }
            else
            {
                var Req = DB.TMT_Requirements.Find(Model.RequirementID.Value);
                if (Req.Status != Models.DBEnums.RequirementStatus.草稿)
                {
                    if (Req.Title.Equals(Model.Title))
                    {
                        //编辑了标题
                    }
                    if (Req.ModuleID.Equals(Model.ModuleID))
                    {
                        //变更了模块
                    }
                    if (Req.AuditorUserID.Equals(Model.AuditorUserID))
                    {
                        //变更了评审人
                    }
                    if (Req.EmergencyLevel.Equals(Model.EmergencyLevel))
                    {
                        //变更了优先级
                    }
                }
                if (Req.Status == Models.DBEnums.RequirementStatus.已审)
                {

                }

            }
            DB.SaveChanges();
            return Json();
        }
    }
}