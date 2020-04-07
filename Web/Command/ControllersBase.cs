using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Controllers;
using Microsoft.AspNetCore.Mvc.Filters;
using Microsoft.AspNetCore.Mvc.ModelBinding;
using Microsoft.AspNetCore.Mvc.Razor.Internal;
using Microsoft.EntityFrameworkCore;
using Newtonsoft.Json.Serialization;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.Threading.Tasks;

namespace Web
{
    public abstract class CustomRazorPage<TModel> : Microsoft.AspNetCore.Mvc.Razor.RazorPage<TModel>
    {
        public SystemModel.Model G
        {
            get
            {
                return (SystemModel.Model)Context.Items["Web_System"];
            }
        }
    }
    public class ControllersBase : Controller
    {
        [ControllerInject]
        public Command.SessionHelper Session { get; set; }
        [ControllerInject]
        public Models.DBContext DB { get; set; }

        public SystemModel.Model G { get; set; }

        public override void OnActionExecuting(ActionExecutingContext Context)
        {
            G = (SystemModel.Model)HttpContext.Items["Web_System"];
            if (G?.User != null)
            {
                //查询可用项目
                var Projects = DB.TMT_Projects.AsNoTracking().Where(c => ("," + c.Users + ",").IndexOf("," + G.User.UserID.ToString() + ",") > -1 && c.State == 0);
                G.UsableProject = new List<SystemModel.UsableProjects>();
                foreach (var item in Projects)
                {
                    G.UsableProject.Add(new SystemModel.UsableProjects { ProjectID = item.ProjectID, ProjectName = item.ProjectName });
                }
                //读取当前默认项目
                var NowProject = DB.TMT_Projects.Find(G.User.DefaultProjectID);
                if (NowProject != null)
                {
                    G.NowProject = new SystemModel.NowProjectModel
                    {
                        ProjectID = NowProject.ProjectID,
                        ProjectName = NowProject.ProjectName
                    };
                    G.NowProject.ProjectUser = DB.TMT_Users.FromSqlRaw(string.Format("select * from TMT_Users where UserID in ({0}) and IsDelete=0", NowProject.Users)).Select(c => new SystemModel.ProjectUsers { UserID = c.UserID, UserName = c.UserName }).ToList();
                    G.NowProject.Modules = DB.TMT_Modules.Where(c => c.ProjectID == NowProject.ProjectID).Select(c => new SystemModel.Modules { ModuleID = c.ModuleID, ModuleName = c.ModuleName }).ToList();
                }
                Context.HttpContext.Items["Web_System"] = G;
            }
            if (G?.NowProject == null && Context.Controller.GetType().IsDefined(typeof(NeedSetProject)))
            {
                Context.Result = new ViewResult
                {
                    ViewName = "_NoSetProject"
                };
            }
        }
        public JsonResult Json()
        {
            return new JsonResult(new ResultModel(), JsonHelp.Option);
        }
        public override JsonResult Json(object Data)
        {
            return new JsonResult(new ResultModel() { Result = Data }, JsonHelp.Option);
        }
        public JsonResult Json(string ErrorMessage)
        {
            return new JsonResult(new ResultModel() { ErrorMessage = ErrorMessage }, JsonHelp.Option);
        }
    }
    public class ResultModel
    {
        public bool Success => string.IsNullOrWhiteSpace(ErrorMessage);
        public string ErrorMessage { get; set; }
        public object Result { get; set; }
    }
    public class JsonHelp
    {
        private class LimitPropsContractResolver : Newtonsoft.Json.Serialization.DefaultContractResolver
        {
            protected override IList<JsonProperty> CreateProperties(Type type, Newtonsoft.Json.MemberSerialization memberSerialization)
            {
                IList<JsonProperty> Properties = base.CreateProperties(type, memberSerialization);
                foreach (var item in Properties)
                {
                    if (item.DeclaringType.GetProperty(item.PropertyName).GetMethod.Attributes.HasFlag(MethodAttributes.Virtual))
                    {
                        item.Ignored = true;
                    }
                }
                return Properties;
            }
        }

        public static System.Text.Json.JsonSerializerOptions Option
        {
            get
            {
                return new System.Text.Json.JsonSerializerOptions { };
            }
        }
    }


    [AttributeUsage(AttributeTargets.Property, AllowMultiple = false, Inherited = true)]
    public class ControllerInjectAttribute : Attribute, IBindingSourceMetadata
    {
        public BindingSource BindingSource => BindingSource.Services;
    }
}