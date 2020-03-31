using Microsoft.AspNetCore.Components;
using Microsoft.JSInterop;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Threading.Tasks;

namespace Tools.Shared.Components
{
    public class Modal_Service
    {
        public ObservableCollection<ModalModel> Modals = new ObservableCollection<ModalModel>();
        public void MessageBox(string Title, string Message, string ICON, Func<ModelResult, object, bool> CallBack)
        {
            var _Model = new ModalModel
            {
                Title = Title,
                Message = Message,
                ModalType = ModalType.信息,
                ICON = ICON,
                CallBack = CallBack
            };
            Modals.Add(_Model);
        }
        public void ConfirmBox(string Title, string Message, string ICON, object Model, Func<ModelResult, object, bool> CallBack)
        {
            var _Model = new ModalModel
            {
                Title = Title,
                Message = Message,
                ModalType = ModalType.询问,
                Model = Model,
                ICON = ICON,
                CallBack = CallBack
            };
            Modals.Add(_Model);
        }
        public void CustomBox(string Title, string Message, string ICON, object Model, Type Component, Func<ModelResult, object, bool> CallBack)
        {
            var _Model = new ModalModel
            {
                Title = Title,
                Message = Message,
                ModalType = ModalType.自定,
                Model = Model,
                ICON = ICON,
                Component = Component,
                CallBack = CallBack
            };
            Modals.Add(_Model);
        }
    }
}
