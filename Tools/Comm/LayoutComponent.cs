using Microsoft.AspNetCore.Components;
using Microsoft.AspNetCore.Http;
using Microsoft.JSInterop;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace Tools
{
    public class LayoutComponent : LayoutComponentBase
    {
        [Inject]
        public IHttpContextAccessor Context { get; set; }
        [Inject]
        public Shared.Components.Modal_Service ModalService { get; set; }
        [Inject]
        public IJSRuntime JSRuntime { get; set; }
        [Inject]
        public NavigationManager Url { get; set; }

        protected override void OnInitialized()
        {
            if (Context.HttpContext.WebSockets.IsWebSocketRequest)
            {
                Initialization();
            }
        }

        protected virtual void Initialization()
        { 
        
        }

        protected override async Task OnAfterRenderAsync(bool FirstRender)
        {
            await JSRuntime.InvokeVoidAsync("web.init");
        }
    }
}