using Microsoft.AspNetCore.Components;
using Microsoft.AspNetCore.Components.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.JSInterop;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace TMT.Command
{
    public class PageComponentBase : LayoutComponentBase
    {
        [Inject]
        public IJSRuntime JSRuntime { get; set; }
        [Inject]
        public IHttpContextAccessor Context { get; set; }
        [Inject]
        public NavigationManager Navigation { get; set; }
        [Inject]
        public Helper.CacheHelper Cache { get; set; }

        public Helper.CookieHelper Cookie { get; set; }

        public UserModel User { get; set; }

        public string Title { get; set; }

        protected override async Task OnInitializedAsync()
        {
            Cookie = new Helper.CookieHelper(JSRuntime, Context.HttpContext);
            if (!await Authentication())
            {
                if (!Context.HttpContext.WebSockets.IsWebSocketRequest)
                {
                    Context.HttpContext.Response.Redirect("/Login");
                }
                else
                {
                    Navigation.NavigateTo("/Login");
                }
            }
            if (Context.HttpContext.WebSockets.IsWebSocketRequest)
            {
                InitComplete();
            }
        }
        protected override async Task OnAfterRenderAsync(bool FirstRender)
        {
            if (FirstRender)
            {
                await JSRuntime.InvokeVoidAsync("app.init", Title);
            }
            RenderComplete();
        }

        private async Task<bool> Authentication()
        {
            string Token = await Cookie.Get("Web-Auth");
            if (!string.IsNullOrWhiteSpace(Token))
            {
                User = Cache.Get<UserModel>(Token);
            }
            (bool AllowAnonymous, int[] Roles) AuthData = (false, new int[] { });
            var TP = GetType().BaseType == typeof(PageComponentBase) ? GetType() : GetType().BaseType;
            if (Cache.ContainKey(TP.FullName))
            {
                AuthData = Cache.Get<(bool, int[])>(TP.FullName);
            }
            else
            {
                var IsDefinedAuth = TP.IsDefined(typeof(AuthAttribute), true);
                if (IsDefinedAuth)
                {
                    var AuthSign = (AuthAttribute)TP.GetCustomAttributes(true).FirstOrDefault(c => c.GetType() == typeof(AuthAttribute));
                    AuthData.Roles = AuthSign.Roles;
                }
                else
                {
                    AuthData.AllowAnonymous = true;
                }
                Cache.Set(TP.FullName, AuthData);
            }
            if (!AuthData.AllowAnonymous && (User == null || (AuthData.Roles.Length > 0 && !AuthData.Roles.Contains(User.Role))))
            {
                return false;
            }
            return true;
        }

        protected virtual void InitComplete()
        {

        }
        protected virtual void RenderComplete()
        {

        }
    }
}