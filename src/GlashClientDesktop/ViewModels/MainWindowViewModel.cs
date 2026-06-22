using System.Reactive;
using AtomUI.Controls;
using AtomUI.Desktop.Controls;
using GlashClientDesktop.Core;
using Quick.Localize;
using ReactiveUI;

namespace GlashClientDesktop.ViewModels
{
    public class MainWindowViewModel : ViewModelBase
    {
        public string Text_ConnectionName { get; } = Locale.GetString("Connection Name");
        public string Text_ServerUrl { get; } = Locale.GetString("Server Url");
        public string Text_User { get; } = Locale.GetString("User");
        public string Text_Password { get; } = Locale.GetString("Password");

        private ConnectionContext _CurrentConnectionContext;
        public ConnectionContext CurrentConnectionContext
        {
            get => _CurrentConnectionContext;
            set => this.RaiseAndSetIfChanged(ref _CurrentConnectionContext, value);
        }

        private ConnectionContext[] _ConnectionContexts;
        public ConnectionContext[] ConnectionContexts
        {
            get => _ConnectionContexts;
            set => this.RaiseAndSetIfChanged(ref _ConnectionContexts, value);
        }

        private bool _EditDrawerIsOpen = false;
        public bool EditDrawerIsOpen
        {
            get => _EditDrawerIsOpen;
            set => this.RaiseAndSetIfChanged(ref _EditDrawerIsOpen, value);
        }
        private string _EditDrawerTitle;
        public string EditDrawerTitle
        {
            get => _EditDrawerTitle;
            set => this.RaiseAndSetIfChanged(ref _EditDrawerTitle, value);
        }
        private IFormValues _EditDrawerFormValues;
        public IFormValues EditDrawerFormValues
        {
            get => _EditDrawerFormValues;
            set => this.RaiseAndSetIfChanged(ref _EditDrawerFormValues, value);
        }

        private Action<IFormValues> SubmitAction;
        public ReactiveCommand<Unit, Unit> AddCommand { get; }
        public ReactiveCommand<Unit, Unit> EditCommand { get; }
        public ReactiveCommand<Unit, Unit> DeleteCommand { get; }

        public MainWindowViewModel()
        {
            AddCommand = ReactiveCommand.Create(ExecuteCommand_Add);
            EditCommand = ReactiveCommand.Create(ExecuteCommand_Edit, this.WhenAnyValue(
                x => x.CurrentConnectionContext,
                new Func<ConnectionContext, bool>(x => x != null)));
            DeleteCommand = ReactiveCommand.Create(ExecuteCommand_Delete, this.WhenAnyValue(
                x => x.CurrentConnectionContext,
                new Func<ConnectionContext, bool>(x => x != null)));
            refreshConnectionContexts();
        }

        private void refreshConnectionContexts()
        {
            ConnectionContexts = ConnectionContextManager.Instance.GetConnectionContexts();
        }

        public void ExecuteCommand_Add()
        {
            EditDrawerFormValues = new FormValues()
            {
                [nameof(Model.Connection.Id)] = Guid.NewGuid().ToString("N"),
                [nameof(Model.Connection.Name)] = "连接1"
            };
            EditDrawerTitle = Locale.GetString("Add Connection");
            EditDrawerIsOpen = true;
            SubmitAction = t =>
            {
                try
                {
                    var newModel = new Model.Connection()
                    {
                        Id = (string)t[nameof(Model.Connection.Id)],
                        Name = (string)t[nameof(Model.Connection.Name)],
                        ServerUrl = (string)t[nameof(Model.Connection.ServerUrl)],
                        User = (string)t[nameof(Model.Connection.User)],
                        Password = (string)t[nameof(Model.Connection.Password)]
                    };
                    ConnectionContextManager.Instance.Add(newModel);
                    refreshConnectionContexts();
                    EditDrawerIsOpen = false;
                }
                catch (Exception ex)
                {
                    
                }
            };
        }

        public void ExecuteCommand_Edit()
        {
            EditDrawerTitle = Locale.GetString("Edit Connection");
            EditDrawerIsOpen = true;
            SubmitAction = t =>
            {
                EditDrawerIsOpen = false;
            };
        }

        public void ExecuteCommand_Delete()
        {
            ConnectionContextManager.Instance.Remove(CurrentConnectionContext.Connection);
            CurrentConnectionContext = null;
        }

        public void Submit(IFormValues values)
        {
            SubmitAction?.Invoke(values);
            SubmitAction = null;
        }
    }
}
