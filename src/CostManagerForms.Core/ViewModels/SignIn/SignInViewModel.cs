using System;
using System.Collections.Generic;
using System.Net;
using System.Text;
using System.Threading.Tasks;
using Acr.UserDialogs;
using CostManagerForms.Core.Localization;
using DAL.Services.CostManager;
using MvvmCross.Commands;
using Xamarin.Forms;

namespace CostManagerForms.Core.ViewModels.SignIn
{
    public class SignInViewModel : BaseViewModel
    {
        private string _login;
        public string Login
        {
            get => _login;
            set => SetProperty(ref _login, value);
        }

        private string _password;
        public string Password
        {
            get => _password;
            set => SetProperty(ref _password, value);
        }

        private bool _save = true;
        public bool Save
        {
            get => _save;
            set => SetProperty(ref _save, value);
        }

        public IMvxCommand SignInCommand { get; }
        public IMvxCommand RegistrateCommand { get; }

        private readonly IUserDialogs _dialogs;
        private readonly ICostManagerService _costManagerService;

        public SignInViewModel(ICostManagerService costManagerService,
                               IUserDialogs dialogs)
        {
            _costManagerService = costManagerService;
            _dialogs = dialogs;

            SignInCommand = new MvxAsyncCommand(() => RequestCommand(SignInAsync));
            RegistrateCommand = new MvxAsyncCommand(() => RequestCommand(RegistrateAsync));
        }

        private async Task RegistrateAsync()
        {
            if (!ValidateLoginData())
                return;

            var message = await _costManagerService.RegisterAsync(_login, _password);

            if (message.StatusCode == 409)
            {
                _dialogs.Alert(AppResources.ExistsUsernameMessage, AppResources.ExistsUsernameTitle);
                return;
            }

            await SignInAsync();
        }
        
        private async Task SignInAsync()
        {
            if (!ValidateLoginData())
                return;

            var login = await _costManagerService.SignInAsync(_login, _password);

            if (login.Token == null)
            {
                _dialogs.Alert(AppResources.IncorrectDataMessage, AppResources.EmptyFieldsTitle);
                return;
            }

            Application.Current.Properties["token"] = login.Token;            

            _dialogs.Alert(login.Token);
        }

        private bool ValidateLoginData()
        {
            if (string.IsNullOrEmpty(_login)
                && string.IsNullOrEmpty(_password))
            {
                _dialogs.Alert(AppResources.EmptyFieldsMessage, AppResources.EmptyFieldsTitle);
                return false;
            }
            return true;
        }
    }
}
