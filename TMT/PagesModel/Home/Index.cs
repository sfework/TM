using System;
using System.Collections.Generic;

namespace TMT.PagesModel.Home
{
    [Command.Auth]
    public class Index : Command.PageComponentBase
    {
        protected override void InitComplete()
        {
            Title = "首页";
        }
    }
}