using System;
using System.Collections.Generic;
using System.Data;
using System.Diagnostics;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Data.SqlClient;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging;

namespace Web.Controllers
{
    public class HomeController : ControllersBase
    {
        [AllowAnonymous]
        public IActionResult Index()
        {
            if (G?.User == null)
            {
                return View("Login");
            }
            return View();
        }
        [HttpPost, AllowAnonymous]
        public IActionResult Login(string Account, string PassWord)
        {
            if (!Check.IsStr(Account))
            {
                return Json("[Account] Input Error!");
            }
            if (!Check.IsPassWord(PassWord))
            {
                return Json("[PassWord] Input Error!");
            }
            var Temp = DB.TMT_Users.FirstOrDefault(c => c.Account == Account);
            if (Temp == null)
            {
                return Json("输入的账号不存在！");
            }
            if (!Temp.Enable)
            {
                return Json("输入的账号已被禁止使用！");
            }
            if (!Temp.PassWord.Equals(PassWord))
            {
                return Json("输入的密码不正确！");
            }
            Temp.LastLoginDate = DateTime.Now;
            DB.SaveChanges();
            Session.Add("Web_System", new SystemModel.Model
            {
                User = new SystemModel.UserModel
                {
                    UserID = Temp.UserID,
                    UserName = Temp.UserName,
                    Avatar = Temp.Avatar,
                    Auths = Array.ConvertAll(Temp.Role.Auths.Split(new char[] { ',' }, StringSplitOptions.RemoveEmptyEntries), c => int.Parse(c)),
                    RoleName = Temp.Role.RoleName,
                    DefaultProjectID = Temp.DefaultProjectID
                }
            });
            return Json();
        }
        public IActionResult Logout()
        {
            Session.Remove("Web_System");
            return Redirect("/");
        }


        [HttpPost]
        public IActionResult ProjectSet(int ProjectID)
        {
            var Project = G.UsableProject.FirstOrDefault(c => c.ProjectID == ProjectID);
            if (Project == null)
            {
                return Json("无权访问此项目或此项目不存在！");
            }
            var Model = DB.TMT_Users.Find(G.User.UserID);
            Model.DefaultProjectID = Project.ProjectID;
            G.User.DefaultProjectID = Project.ProjectID;
            DB.SaveChanges();
            Session.Add("Web_System", G);
            return Json();
        }

        [AllowAnonymous]
        public IActionResult UPTest()
        {
            return View();
        }

        [HttpPost, AllowAnonymous]
        public async Task<IActionResult> UPLoad()
        {
            return Json(await Command.Helper.SaveAsync(HttpContext.Request.Form.Files));
        }
        [HttpPost]
        public async Task<IActionResult> Edit_UPload()
        {
            var Re = await Command.Helper.SaveAsync(HttpContext.Request.Form.Files);

            var ImagesType = new string[] { ".gif", ".jpg", ".jpeg", ".bmp", ".png", ".ico" };

            List<string> _Files = new List<string>();
            List<bool> _IsImage = new List<bool>();
            foreach (var item in Re)
            {
                _Files.Add(item);
                _IsImage.Add(Array.IndexOf(ImagesType, System.IO.Path.GetExtension(item).ToLower()) > -1);
            }
            var Data = new
            {
                success = true,
                time = DateTime.Now,
                data = new
                {
                    baseurl = "",
                    messages = new string[] { },
                    files = _Files.ToArray(),
                    isImages = _IsImage.ToArray(),
                    code = 220
                }
            };
            return new JsonResult(Data);
        }
    }
}