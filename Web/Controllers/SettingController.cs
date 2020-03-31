using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

namespace Web.Controllers
{
    public class SettingController : ControllersBase
    {
        #region 用户管理
        public IActionResult UsersManagement(ParamModels.Setting.UsersManagementList Model)
        {
            if (!string.IsNullOrWhiteSpace(Model.KeyWord))
            {
                Model.List = DB.TMT_Users.Where(c => c.UserName.Contains(Model.KeyWord));
            }
            else
            {
                Model.List = DB.TMT_Users;
            }
            if (Model.RoleID.HasValue)
            {
                Model.List = Model.List.Where(c => c.RoleID == Model.RoleID.Value);
            }
            if (Model.Enable.HasValue)
            {
                Model.List = Model.List.Where(c => c.Enable == Model.Enable);
            }
            Model.List = Model.List.OrderByDescending(m => m.UserID);
            Model.Create();
            return View(Model);
        }
        public IActionResult Users_Add(int? UserID)
        {
            Models.TMT_Users Model = null;
            if (UserID.HasValue)
            {
                Model = DB.TMT_Users.Find(UserID.Value);
            }
            return View(Model);
        }
        [HttpPost]
        public IActionResult Users_Add_Save(Models.TMT_Users Model)
        {
            if (!Check.IsStr(Model.Account))
            {
                return Json("[Account]Format error!");
            }
            if (!Check.IsStr(Model.UserName))
            {
                return Json("[UserName]Format error!");
            }
            if (string.IsNullOrEmpty(Model.Avatar))
            {
                return Json("[Avatar]Format error!");
            }
            if (Model.RoleID < 1)
            {
                return Json("[Role]Setting error!");
            }
            if (Model.UserID > 0)
            {
                if (!string.IsNullOrEmpty(Model.PassWord) && !Check.IsPassWord(Model.PassWord))
                {
                    return Json("[PassWord]Format error!");
                }
                if (DB.TMT_Users.Any(c => c.UserID != Model.UserID && c.UserName == Model.UserName))
                {
                    return Json("UserName已存在！");
                }
                if (DB.TMT_Users.Any(c => c.UserID != Model.UserID && c.Account == Model.Account))
                {
                    return Json("Account已存在！");
                }
                var TUser = DB.TMT_Users.Find(Model.UserID);
                TUser.UserName = Model.UserName;
                TUser.Account = Model.Account;
                TUser.Avatar = Model.Avatar;
                TUser.RoleID = Model.RoleID;
                TUser.Enable = Model.Enable;
                if (!string.IsNullOrEmpty(Model.PassWord))
                {
                    TUser.PassWord = Command.Helper.GenerateMD5(TUser.PassWord);
                }
            }
            else
            {
                if (!Check.IsPassWord(Model.PassWord))
                {
                    return Json("[PassWord]Format error!");
                }
                if (DB.TMT_Users.Any(c => c.UserName == Model.UserName))
                {
                    return Json("UserName已存在！");
                }
                if (DB.TMT_Users.Any(c => c.Account == Model.Account))
                {
                    return Json("Account已存在！");
                }
                Model.PassWord = Command.Helper.GenerateMD5(Model.PassWord);
                DB.TMT_Users.Add(Model);
            }
            DB.SaveChanges();
            return Json();
        }
        #endregion

        #region 角色管理
        public IActionResult RolesManagement(ParamModels.Setting.RolesManagementList Model)
        {
            if (!string.IsNullOrWhiteSpace(Model.KeyWord))
            {
                Model.List = DB.TMT_Roles.Where(c => c.RoleName.Contains(Model.KeyWord));
            }
            else
            {
                Model.List = DB.TMT_Roles;
            }
            Model.List = Model.List.OrderByDescending(m => m.RoleID);
            return View(Model);
        }
        public IActionResult Roles_Add(int? RoleID)
        {
            Models.TMT_Roles Model = null;
            if (RoleID.HasValue)
            {
                Model = DB.TMT_Roles.Find(RoleID.Value);
            }
            return View(Model);
        }
        [HttpPost]
        public IActionResult Roles_Add_Save(Models.TMT_Roles Model)
        {
            if (Model.RoleID > 0)
            {
                var Role = DB.TMT_Roles.Find(Model.RoleID);
                if (!Role.IsAllowDelete)
                {
                    return Json("此记录不可编辑！");
                }
                if (DB.TMT_Roles.Any(c => c.RoleName == Model.RoleName && c.RoleID != Role.RoleID))
                {
                    return Json("角色名已存在！");
                }
                Role.RoleName = Model.RoleName;
                Role.Auths = Model.Auths;
            }
            else
            {
                if (DB.TMT_Roles.Any(c => c.RoleName == Model.RoleName))
                {
                    return Json("角色名已存在！");
                }
                DB.TMT_Roles.Add(Model);
            }
            DB.SaveChanges();
            return Json();
        }
        [HttpPost]
        public IActionResult Roles_Add_Del(int RoleID)
        {
            var Role = DB.TMT_Roles.Find(RoleID);
            if (!Role.IsAllowDelete)
            {
                return Json("此记录不可编辑！");
            }
            if (DB.TMT_Users.Any(c => c.RoleID == RoleID))
            {
                return Json("存在使用此角色的用户，无法删除！");
            }
            Role.IsDelete = true;
            DB.SaveChanges();
            return Json();
        }
        #endregion
        #region 项目管理
        public IActionResult ProjectsManagement(ParamModels.Setting.ProjectsManagementList Model)
        {
            if (!string.IsNullOrWhiteSpace(Model.KeyWord))
            {
                Model.List = DB.TMT_Projects.Where(c => c.ProjectName.Contains(Model.KeyWord));
            }
            else
            {
                Model.List = DB.TMT_Projects;
            }
            Model.List = Model.List.OrderByDescending(m => m.ProjectID);
            foreach (var item in Model.List)
            {
                item.D_Users = DB.TMT_Users.FromSqlRaw(string.Format("select * from TMT_Users where UserID in ({0})", item.Users));
            }
            return View(Model);
        }
        public IActionResult Projects_Add(int? ProjectID)
        {
            ViewBag.User = DB.TMT_Users.Where(c => c.Enable);
            Models.TMT_Projects Model = null;
            if (ProjectID.HasValue)
            {
                Model = DB.TMT_Projects.Find(ProjectID.Value);
            }
            return View(Model);
        }
        [HttpPost]
        public IActionResult Projects_Add_Save(Models.TMT_Projects Model)
        {
            if (!Check.IsStr(Model.ProjectName, 4, 50))
            {
                return Json("[ProjectName] Format Error!");
            }
            if (string.IsNullOrEmpty(Model.Description))
            {
                return Json("[Description] Format Error!");
            }
            if (string.IsNullOrEmpty(Model.Users))
            {
                return Json("[Users] Must Select!");
            }
            if (Model.ProjectID > 0)
            {
                var Project = DB.TMT_Projects.Find(Model.ProjectID);
                if (DB.TMT_Projects.Any(c => c.ProjectName == Model.ProjectName && c.ProjectID != Project.ProjectID))
                {
                    return Json("项目名已存在！");
                }
                Project.ProjectName = Model.ProjectName;
                Project.Users = Model.Users;
                Project.Description = Model.Description;
                Project.State = Model.State;
            }
            else
            {
                if (DB.TMT_Projects.Any(c => c.ProjectName == Model.ProjectName))
                {
                    return Json("项目名已存在！");
                }
                Model.Path = "Path";
                DB.TMT_Projects.Add(Model);
            }
            DB.SaveChanges();
            return Json();
        }
        [HttpGet]
        public IActionResult Projects_Module_Set(int ProjectID)
        {
            ViewBag.ProjectID = ProjectID;
            var Model = DB.TMT_Modules.Where(c => c.ProjectID == ProjectID);
            return View(Model);
        }
        [HttpPost]
        public IActionResult Projects_Module_Set(int ProjectID, List<Models.TMT_Modules> Modules)
        {
            var IDs = string.Join(",", Modules.Where(c => c.ModuleID > 0).Select(c => c.ModuleID).ToArray());
            DB.Database.ExecuteSqlRaw(string.Format("update TMT_Modules set IsDelete=1 where ProjectID={0} and ModuleID not in({1})", ProjectID, IDs));
            foreach (var item in Modules)
            {
                if (item.ModuleID > 0)
                {
                    if (DB.TMT_Modules.Any(c => c.ProjectID == ProjectID && c.ModuleID != item.ModuleID && c.ModuleName == item.ModuleName))
                    {
                        return Json($"[{item.ModuleName}]已经存在！");
                    }
                    var Temp = DB.TMT_Modules.Find(item.ModuleID);
                    Temp.ModuleName = item.ModuleName;
                }
                else
                {
                    if (DB.TMT_Modules.Any(c => c.ProjectID == ProjectID && c.ModuleName == item.ModuleName))
                    {
                        return Json($"[{item.ModuleName}]已经存在！");
                    }
                    DB.TMT_Modules.Add(item);
                }
            }
            DB.SaveChanges();
            return Json();
        }
        #endregion
    }
}