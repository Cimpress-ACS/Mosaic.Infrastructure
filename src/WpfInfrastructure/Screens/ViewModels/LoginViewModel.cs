/* Copyright 2017 Cimpress

Licensed under the Apache License, Version 2.0 (the "License");
you may not use this file except in compliance with the License.
You may obtain a copy of the License at

http://www.apache.org/licenses/LICENSE-2.0

Unless required by applicable law or agreed to in writing, software
distributed under the License is distributed on an "AS IS" BASIS,
WITHOUT WARRANTIES OR CONDITIONS OF ANY KIND, either express or implied.
See the License for the specific language governing permissions and
limitations under the License. */


using Centigrade.Kit.StateMachine;
using System.Collections.ObjectModel;
using System.Windows.Input;
using VP.FF.PT.Common.WpfInfrastructure.Screens.Model;

namespace VP.FF.PT.Common.WpfInfrastructure.Screens.ViewModels
{
    public class LoginViewModel : LoginBaseViewModel
    {
        public LoginViewModel(StateMachine stateMachine, ICommand loginCommand)
            : base(stateMachine, loginCommand)
        {
            // TODO: Remove test data
            if (DesignTimeHelper.IsInDesignModeStatic)
            {
                UseTestData();
            }
            UserNames = new ObservableCollection<string>();
        }

        public override ObservableCollection<string> UserNames
        {
            get { return _userNames; }
            set
            {
                _userNames = value;
                NotifyOfPropertyChange(() => UserNames);
            }
        }

        private string _selectedUserName;

        public string SelectedUserName
        {
            get { return _selectedUserName; }
            set
            {
                if (_selectedUserName != value)
                {
                    _selectedUserName = value;
                    UserName = _selectedUserName;
                    NotifyOfPropertyChange(() => SelectedUserName);
                }
            }
        }

        private void UseTestData()
        {
            UserName = "Jörg Niesenhaus";
            RecentUsers = new ObservableCollection<User>
            {
                new User {Name = "Peter Pan"},
                new User {Name = "Max Mustermann"}
            };
            Password = "12345";
        }

        public override ObservableCollection<User> RecentUsers { get; set; }

        private string _userName;

        public override string UserName
        {
            get
            {
                return _userName;
            }

            set
            {
                if (_userName != value)
                {
                    _userName = value;
                    NotifyOfPropertyChange(() => UserName);
                    var resetCommand = ResetCommand as IStateTransitionCommand;
                    var loginCommand = LoginCommand as IStateTransitionCommand;

                    if (resetCommand != null)
                    {
                        resetCommand.RaiseCanExecuteChanged();
                    }

                    if (loginCommand != null)
                    {
                        loginCommand.RaiseCanExecuteChanged();
                    }
                }
            }
        }

        private string _password;
        private ObservableCollection<string> _userNames;

        public override string Password
        {
            get { return _password; }

            set
            {
                if (_password != value)
                {
                    _password = value;
                    NotifyOfPropertyChange(() => Password);
                    var resetCommand = ResetCommand as IStateTransitionCommand;
                    var loginCommand = LoginCommand as IStateTransitionCommand;

                    if (resetCommand != null)
                    {
                        resetCommand.RaiseCanExecuteChanged();
                    }

                    if (loginCommand != null)
                    {
                        loginCommand.RaiseCanExecuteChanged();
                    }
                }
            }
        }

        protected override bool OnCanExecute()
        {
            return !(string.IsNullOrEmpty(UserName) && string.IsNullOrEmpty(Password));
        }

        protected override void OnReset()
        {
            base.OnReset();
            UserName = string.Empty;
            Password = string.Empty;
        }

    }
}
