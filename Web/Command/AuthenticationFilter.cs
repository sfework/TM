using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Controllers;
using Microsoft.AspNetCore.Mvc.Filters;
using Microsoft.Extensions.Caching.Memory;
using System;

namespace Web
{
    public class AuthenticationFilter : IAuthorizationFilter
    {
        private readonly AuthenticationAction Actions = new AuthenticationAction();
        private readonly Command.SessionHelper Session;
        public AuthenticationFilter(Command.SessionHelper _Session)
        {
            Session = _Session;
        }
        public void OnAuthorization(AuthorizationFilterContext Context)
        {
            var ReturnAction = Actions.Get(Context);
            var User = Session.Get<Command.UserModel>("Web_Auth");
            if (!ReturnAction.IsAllowAnonymous)
            {
                if (User == null)
                {
                    Context.Result = new RedirectResult("/");
                    return;
                }
            }
            Context.HttpContext.Items.Add("Web_User", User);
        }
    }
    public class AuthenticationAction
    {
        private static readonly MemoryCache MCache = new MemoryCache(new MemoryCacheOptions());
        public RequestActionModel Get(AuthorizationFilterContext Context)
        {
            var ReturnAction = MCache.Get<RequestActionModel>("RequestActionSign:" + Context.ActionDescriptor.Id);
            if (ReturnAction == null)
            {
                ControllerActionDescriptor Controller = (ControllerActionDescriptor)Context.ActionDescriptor;
                bool IsAllowAnonymous = Controller.ControllerTypeInfo.IsDefined(typeof(IAllowAnonymous), true);
                if (Controller.MethodInfo.IsDefined(typeof(IAllowAnonymous), true)) { IsAllowAnonymous = true; }
                ReturnAction = new RequestActionModel
                {
                    Sign = Controller.Id,
                    ControllerName = Controller.ControllerName,
                    ActionName = Controller.ActionName,
                    IsAllowAnonymous = IsAllowAnonymous
                };
                MCache.Set("RequestActionSign:" + Controller.Id, ReturnAction);
            }
            return ReturnAction;
        }
    }
    public class RequestActionModel
    {
        public string Sign { get; set; }
        public string ControllerName { get; set; }
        public string ActionName { get; set; }
        public bool IsAllowAnonymous { get; set; }
    }

    public class AuthAttribute : Attribute
    {
        public Command.Powers[] Powers { get; set; }
        public AuthAttribute(params Command.Powers[] _Powers)
        {
            Powers = _Powers;
        }
    }
}
