using System;
using System.Collections.Generic;
using System.Text;
using System.Threading.Tasks;
using Acr.UserDialogs;
using CostManagerForms.Core.Extensions;
using MvvmCross.ViewModels;
using Plugin.Connectivity;
using Plugin.Connectivity.Abstractions;

namespace CostManagerForms.Core.ViewModels
{
    public abstract class BaseViewModel : MvxViewModel
    {
        public bool IsConnected => CrossConnectivity.IsSupported
                                  && _connectivity.IsConnected;

        private readonly IConnectivity _connectivity;
        private readonly IUserDialogs _dialogs;

        bool _canNavigatePage = true;

        protected BaseViewModel()
        {
            _connectivity = CrossConnectivity.Current;
            _dialogs = UserDialogs.Instance;
        }

        protected BaseViewModel(IUserDialogs dialogs)
        {
            _connectivity = CrossConnectivity.Current;
            _dialogs = dialogs;
        }

        protected BaseViewModel(IConnectivity connectivity, IUserDialogs dialogs)
        {
            _connectivity = connectivity;
            _dialogs = dialogs;
        }

        public override void ViewAppeared()
        {
            base.ViewAppeared();
            _canNavigatePage = true;
        }

        public async Task RequestCommand(Func<Task> action, string loadingTitle = null)
        {
            if (!IsConnected)
            {
                await _dialogs.ShowConnectionError();
                return;
            }
            await LoadingCommand(action, loadingTitle);
        }

        public async Task RequestCommand<TParam>(Func<TParam, Task> action, TParam obj, string loadingTitle = null)
        {
            if (!IsConnected)
            {
                await _dialogs.ShowConnectionError();
                return;
            }
            await LoadingCommand(action, obj, loadingTitle);
        }

        public async Task LoadingCommand(Func<Task> action, string loadingTitle = null)
        {
            _dialogs.ShowLoading(loadingTitle);

            await action();

            _dialogs.HideLoading();
        }

        public async Task LoadingCommand<TParam>(Func< TParam, Task> action, TParam obj, string loadingTitle = null)
        {
            _dialogs.ShowLoading(loadingTitle);

            await action(obj);

            _dialogs.HideLoading();
        }

        public async Task NavigateCommand<TParam>(Func<TParam, Task> action, TParam obj)
        {
            if (_canNavigatePage)
            {
                _canNavigatePage = false;
                await action(obj);
                _canNavigatePage = true;
            }
        }

        public async Task NavigateCommand(Func<Task> action)
        {
            if (_canNavigatePage)
            {
                _canNavigatePage = false;
                await action();
            }
        }

        public bool ValidateCollection<TItem>(List<TItem> collection)
        {
            if (collection != null && collection.Count != 0)
                return true;
            return false;
        }

        public bool Validate(string item)
        {
            if (item == string.Empty)
                return false;
            return true;
        }

        public bool Validate(int item)
        {
            if (item == default(int))
                return false;
            return true;
        }

        public bool Validate(decimal item)
        {
            if (item == default(decimal))
                return false;
            return true;
        }
    }
}
