using Microsoft.AspNetCore.Components;
using Microsoft.JSInterop;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Collections.Specialized;
using System.Linq;
using System.Linq.Expressions;
using System.Threading.Tasks;

namespace Tools.Shared.Components
{
    public enum ModalType
    {
        信息 = 1,
        询问 = 2,
        自定 = 3
    }
    public enum ModelResult
    {
        确认 = 1,
        取消 = 2,
        关闭 = 3
    }
    public class ModalModel
    {
        public string Title { get; set; }
        public string Message { get; set; }
        public ModalType ModalType { get; set; }
        public object Model { get; set; }
        public string ICON { get; set; }
        public bool Complete { get; set; }
        public ElementReference Element { get; set; }
        public Func<ModelResult, object, bool> CallBack { get; set; }
        public Type Component { get; set; }
        public RenderFragment ComponentRender { get; set; }
    }


    public class Modal_PageModel : LayoutComponent
    {
        public List<ModalModel> Modals = new List<ModalModel>();

        protected override void OnInitialized()
        {
            ModalService.Modals.CollectionChanged -= Models_CollectionChanged;
            ModalService.Modals = new ObservableCollection<ModalModel>();
            ModalService.Modals.CollectionChanged += Models_CollectionChanged;
        }
        private void Models_CollectionChanged(object sender, NotifyCollectionChangedEventArgs e)
        {
            ModalModel Temp = null;
            if (e.Action == NotifyCollectionChangedAction.Add)
            {
                Temp = e.NewItems.OfType<ModalModel>().FirstOrDefault();
                if (Temp.ModalType == ModalType.自定)
                {
                    Temp.ComponentRender = CreateButtonRenderer(Temp.Component, Temp.Model);
                }
                Modals.Add(Temp);
            }
            else if (e.Action == NotifyCollectionChangedAction.Remove)
            {
                foreach (var item in e.OldItems.OfType<ModalModel>())
                {
                    Modals.Remove(item);
                }
            }
            InvokeAsync(StateHasChanged);
        }
        private RenderFragment CreateButtonRenderer(Type _Type, object Model)
        {
            return Builder =>
            {
                Builder.OpenComponent(0, _Type);
                Builder.AddAttribute(1, "Model", Model);
                Builder.CloseComponent();
            };
        }
        public async Task CallBack(ModalModel _Modal, ModelResult Result)
        {
            bool IsComplete = false;
            if (_Modal.CallBack == null || Result == ModelResult.关闭)
            {
                IsComplete = true;
            }
            else if (_Modal.CallBack.Invoke(Result, _Modal.Model))
            {
                IsComplete = true;
            }
            if (IsComplete)
            {
                await JSRuntime.InvokeVoidAsync("web.modal.hide", _Modal.Element); ;
                ModalService.Modals.Remove(_Modal);
            }
        }
        protected override async Task OnAfterRenderAsync(bool FirstRender)
        {
            var _Modal = Modals.FirstOrDefault(c => !c.Complete);
            if (_Modal == null)
            {
                return;
            }
            _Modal.Complete = true;
            await JSRuntime.InvokeVoidAsync("web.modal.show", _Modal.Element);
        }
    }
}