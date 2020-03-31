using Microsoft.JSInterop;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace TMT.PagesModel.Home
{
    public class Login : Command.PageComponentBase
    {
        public string Account { get; set; }
        public string PassWord { get; set; }
        protected override void InitComplete()
        {
            Title = "登录";
        }
        public async Task AccountLogin()
        {
            Command.UserModel Model = new Command.UserModel();
            string Token = Guid.NewGuid().ToString();
            await Cookie.Set("Web-Auth", Token);
            Cache.Set(Token, Model);
            await JSRuntime.InvokeVoidAsync("app.url.go", "/");
        }
        public void Sub()
        {
            Console.WriteLine(1);
        }
    }
}