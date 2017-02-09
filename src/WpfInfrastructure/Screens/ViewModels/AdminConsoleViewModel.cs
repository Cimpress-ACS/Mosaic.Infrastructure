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


using System;
using System.Reflection;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Input;
using Caliburn.Micro;
using Roslyn.Compilers;
using Roslyn.Scripting;
using Roslyn.Scripting.CSharp;
using VP.FF.PT.Common.WpfInfrastructure.AdminConsoleServiceReference;

namespace VP.FF.PT.Common.WpfInfrastructure.Screens.ViewModels
{
    public class AdminConsoleViewModel : Screen
    {
        private bool _isvisible;
        private string _consoleHistory;
        private string _command;
        private string _currentModule;
        private string _currentModuleKey;
        private object _context;
        private Session _session;
        private ScriptEngine _engine;
        private readonly AdminConsoleServiceClient _client;

        public AdminConsoleViewModel()
        {
            _client = new AdminConsoleServiceClient();
            _isvisible = false;
            _consoleHistory = String.Empty;
            _command = String.Empty;
            IsVisible = false;
            ReturnKeyPressed = new RelayCommand(EnterKeyPressed);
            ReturnKeyPressed.GestureKey = Key.Return;
        }

        /// <summary>
        /// Load all useful references
        /// </summary>
        public void ScriptEngineLoadAssemblies(ScriptEngine engine, string localAssembly)
        {
            //GAC Assemblies
            var systemCoreReference = MetadataReference.CreateAssemblyReference("System.Core");
            var systemReference = MetadataReference.CreateAssemblyReference("System");
            var systemComponentModel = MetadataReference.CreateAssemblyReference("System.ComponentModel");
            var systemComponentModelComposition =
                MetadataReference.CreateAssemblyReference("System.ComponentModel.Composition");
            var systemIO = MetadataReference.CreateAssemblyReference("System.IO");
            var systemLinq = MetadataReference.CreateAssemblyReference("System.Linq");
            var systemReflection = MetadataReference.CreateAssemblyReference("System.Runtime.Serialization");
            var systemServiceModel = MetadataReference.CreateAssemblyReference("System.ServiceModel");
            var systemWindows = MetadataReference.CreateAssemblyReference("System.Windows");

            //Custom Assemblies
            Assembly wpfAssembly = Assembly.GetAssembly(typeof(AdminConsoleViewModel));
            Assembly caliburnMicro = Assembly.GetAssembly(typeof(Conductor<IScreen>.Collection.OneActive));

            engine.AddReference(systemCoreReference);
            engine.AddReference(systemReference);
            engine.AddReference(systemComponentModel);
            engine.AddReference(systemComponentModelComposition);
            engine.AddReference(systemIO);
            engine.AddReference(systemLinq);
            engine.AddReference(systemReflection);
            engine.AddReference(systemServiceModel);
            engine.AddReference(systemWindows);
            engine.AddReference(wpfAssembly.Location);
            engine.AddReference(caliburnMicro.Location);
            engine.AddReference(localAssembly);

        }

        private void LoadAssemblies()
        {
            try
            {
                _session.Execute(
                    "using System;using System.ComponentModel;" +
                    "using System.ComponentModel.Composition;" +
                    "using System.IO;using System.Linq;" +
                    "using System.Runtime.Serialization;" +
                    "using System.ServiceModel;" +
                    "using System.Windows;" +
                    "using Caliburn.Micro;");
            }
            catch (Exception e)
            {
                ConsoleHistory += "An error occured!\n" + e.Message;
                return;
            }

            ConsoleHistory = String.Empty;
            ConsoleHistory += "All Assemblies loaded succesfully! Console is ready to run.\n";

        }

        public bool IsVisible
        {
            get { return _isvisible; }
            set
            {
                _isvisible = value;
                NotifyOfPropertyChange(() => IsVisible);
            }
        }

        public string Command
        {
            get { return _command; }
            set
            {
                _command = value;
                NotifyOfPropertyChange(() => Command);
            }
        }

        public string CurrentModuleKey
        {
            get { return _currentModuleKey; }
            set
            {
                _currentModuleKey = value;
                NotifyOfPropertyChange(() => CurrentModuleKey);
            }
        }

        public Session Session
        {
            get { return _session; }
            set
            {
                if (value != _session)
                {
                    _session = value;
                    LoadAssemblies();
                    NotifyOfPropertyChange(() => Session);
                }
            }
        }

        public void Animation(string role)
        {
            if (role != "Administrator")
            {
                IsVisible = false;
                return;
            }

            if (IsVisible)
            {
                IsVisible = false;
                return;
            }

            IsVisible = true;
        }

        public RelayCommand ReturnKeyPressed { get; set; }

        public String ConsoleHistory
        {
            get { return _consoleHistory; }
            set
            {
                _consoleHistory = value;
                NotifyOfPropertyChange(() => ConsoleHistory);
            }
        }

        private void ProcessCommands()
        {
            ConsoleHistory += (Command + "\n");

            if (Command == "/clear")
            {
                ConsoleHistory = String.Empty;
                Command = String.Empty;
                return;
            }

            if (Command == "/module")
            {
                _client.ExecuteCommand(Command);
                ConsoleHistory += (_currentModule + '\n');
                Command = String.Empty;
                return;
            }

            if (Command == "/help")
            {
                ShowHelpCommands();
                Command = String.Empty;
                return;
            }

            if (Command == "/list")
            {
                foreach (MemberInfo t in _context.GetType().GetMembers())
                    if (t.Module.ToString().Equals(_context.GetType().Module.ToString()))
                        ConsoleHistory += (t + "\n");

                Command = String.Empty;
                return;
            }

            if (Command.StartsWith("/mosaic "))
            {
                Command = Command.Remove(0, 7);

                ConsoleHistory += (_client.ExecuteCommand(Command).ToString() + '\n');

                Command = String.Empty;
                return;
            }

            if (String.IsNullOrEmpty(Command))
            {
                return;
            }

            ConsoleHistory += (Command + '\n');
            object returnvalue;

            try
            {
                returnvalue = _session.Execute(Command);
            }
            catch (Exception e)
            {
                ConsoleHistory += ("An error occurred!\n" + e.Message + '\n');
                returnvalue = null;
            }

            if (returnvalue != null)
            {
                ConsoleHistory += (returnvalue.ToString() + '\n');
            }

            Command = String.Empty;
        }

        public void ShowHelpCommands()
        {
            ConsoleHistory += "/clear - clears the console screen\n" +
                              "/module - shows the current context(module)\n" +
                              "/help - displays the commands available\n" +
                              "/mosaic Command - executes Command on Mosaic\n" +
                              "Saber commands (to execute type /saber in front of the command):\n\t" +
                              "list modules - displays all possible modules available for loading\n\t" +
                              "container - loads the Composition Container into LoadedContainer property <- simulation access\n\t" +
                              "load module X - load the module X\n\t" +
                              "list {variablename} - list public members of variablename\n\t" +
                              "list - shows all public members you can use\n" +
                              "/list - shows all available properties, methods, events\n";
        }


        /// <summary>
        /// Loads current session context(ViewModel)
        /// </summary>
        /// <param name="context">the ViewModel</param>
        /// <param name="moduleKey"></param>
        public void LoadSession(object context, string moduleKey)
        {
            if ((!string.IsNullOrEmpty(CurrentModuleKey) && 
                CurrentModuleKey != moduleKey))
                return;

            _currentModuleKey = moduleKey;

            _context = context;
            _engine = new ScriptEngine();
            ScriptEngineLoadAssemblies(_engine, context.GetType().Assembly.Location);
            _currentModule = context.GetType().ToString();

            Session = _engine.CreateSession(context, context.GetType());
        }

        private void EnterKeyPressed(object sender)
        {
            ProcessCommands();
        }
    }

    public class RelayCommand : ICommand
    {
        private Action<object> _action;
        public Key GestureKey { get; set; }

        public RelayCommand(Action<object> action)
        {
            _action = action;
        }

        public bool CanExecute(object parameter)
        {
            return true;
        }

        public event EventHandler CanExecuteChanged { add { } remove { } }

        public void Execute(object parameter)
        {
            _action(parameter);
        }
    }

    public static class Helper
    {
        public static bool GetAutoScroll(DependencyObject obj)
        {
            return (bool) obj.GetValue(AutoScrollProperty);
        }

        public static void SetAutoScroll(DependencyObject obj, bool value)
        {
            obj.SetValue(AutoScrollProperty, value);
        }

        public static readonly DependencyProperty AutoScrollProperty =
            DependencyProperty.RegisterAttached("AutoScroll", typeof(bool), typeof(Helper), new PropertyMetadata(false, AutoScrollPropertyChanged));

        private static void AutoScrollPropertyChanged(DependencyObject d, DependencyPropertyChangedEventArgs e)
        {
            var scrollViewer = d as ScrollViewer;

            if (scrollViewer != null && (bool) e.NewValue)
            {
                scrollViewer.ScrollToBottom();
            }
        }
    }
}
