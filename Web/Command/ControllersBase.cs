using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Filters;
using Microsoft.AspNetCore.Mvc.ModelBinding;
using Microsoft.AspNetCore.Mvc.Razor.Internal;
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
        public new Command.UserModel User
        {
            get
            {
                return (Command.UserModel)Context.Items["Web_User"];
            }
        }
        public IQueryable<Models.TMT_Projects> Projects
        {
            get
            {
                return (IQueryable<Models.TMT_Projects>)Context.Items["Web_Usable_Projects"];
            }
        }
    }
    public class ControllersBase : Controller
    {
        [ControllerInject]
        public Command.SessionHelper Session { get; set; }
        [ControllerInject]
        public Models.DBContext DB { get; set; }

        public new Command.UserModel User { get; set; }

        public IQueryable<Models.TMT_Projects> Projects { get; set; }

        public override void OnActionExecuting(ActionExecutingContext Context)
        {
            User = (Command.UserModel)HttpContext.Items["Web_User"];
            if (User != null)
            {
                Projects = DB.TMT_Projects.Where(c => ("," + c.Users + ",").IndexOf("," + User.UserID.ToString() + ",") > -1 && c.State == 0);
                Context.HttpContext.Items.Add("Web_Usable_Projects", Projects);
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