using Microsoft.AspNetCore.Components;
using Microsoft.JSInterop;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Collections.Specialized;
using System.Linq;
using System.Threading.Tasks;

namespace Tools.PagesModel
{
    public class Home : LayoutComponent
    {
        protected override void Initialization()
        {
            Em em1 = new Em
            {
                Name = "ABC"
            };



            var em2 = em1.Clone();
            em2.Name = "DEF";

            Console.WriteLine(em1.Name);
            Url.NavigateTo("/RedisManage");
        }

        public void ShowMessage()
        {
            ModalService.ConfirmBox("title", "mess", null, null, null);
        }
    }

    public class Em
    {
        public string Name { get; set; }
    }
}