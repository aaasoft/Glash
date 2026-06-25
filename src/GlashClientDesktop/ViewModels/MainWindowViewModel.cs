using System.Reactive;
using Avalonia.Threading;
using GlashClientDesktop.Core;
using GlashClientDesktop.Views;
using Quick.Localize;
using ReactiveUI;
using Ursa.Controls;

namespace GlashClientDesktop.ViewModels
{
    public class MainWindowViewModel : ViewModelBase
    {
        public string Text_Connections => Locale<MainWindowViewModel>.GetString("Connections");
        public string Text_DeleteConfirm => Locale<MainWindowViewModel>.GetString("Delete Confirm");
        public string Text_DeleteConnectionConfirm => Locale<MainWindowViewModel>.GetString("Are you sure to delete selected connection?");

        private ConnectionConsoleViewModel _ConnectionConsoleViewModel;
        public ConnectionConsoleViewModel ConnectionConsoleViewModel
        {
            get => _ConnectionConsoleViewModel;
            set => this.RaiseAndSetIfChanged(ref _ConnectionConsoleViewModel, value);
        }

        private ConnectionContext _CurrentConnectionContext;
        public ConnectionContext CurrentConnectionContext
        {
            get => _CurrentConnectionContext;
            set
            {
                this.RaiseAndSetIfChanged(ref _CurrentConnectionContext, value);
                if (value == null)
                    ConnectionConsoleViewModel = null;
                else
                    ConnectionConsoleViewModel = new ConnectionConsoleViewModel() { Model = value };
            }
        }

        private ConnectionContext[] _ConnectionContexts;
        public ConnectionContext[] ConnectionContexts
        {
            get => _ConnectionContexts;
            set => this.RaiseAndSetIfChanged(ref _ConnectionContexts, value);
        }

        public ReactiveCommand<Unit, Unit> AddCommand { get; }
        public ReactiveCommand<Unit, Unit> EditCommand { get; }
        public ReactiveCommand<Unit, Unit> DeleteCommand { get; }
        public ReactiveCommand<Unit, Unit> FakeDeleteCommand { get; }

        public MainWindowViewModel()
        {
            AddCommand = ReactiveCommand.CreateFromTask(ExecuteCommand_Add);
            EditCommand = ReactiveCommand.CreateFromTask(ExecuteCommand_Edit, this.WhenAnyValue(
                x => x.CurrentConnectionContext,
                new Func<ConnectionContext, bool>(x => x != null)));
            DeleteCommand = ReactiveCommand.Create(ExecuteCommand_Delete, this.WhenAnyValue(
                x => x.CurrentConnectionContext,
                new Func<ConnectionContext, bool>(x => x != null)));
            FakeDeleteCommand = ReactiveCommand.Create(() => { }, this.WhenAnyValue(
                x => x.CurrentConnectionContext,
                new Func<ConnectionContext, bool>(x => x != null)));
            refreshConnectionContexts();
        }

        private void refreshConnectionContexts()
        {
            ConnectionContexts = ConnectionContextManager.Instance.GetConnectionContexts();
        }

        public async Task ExecuteCommand_Add()
        {
            var options = new OverlayDialogOptions()
            {
                Buttons = DialogButton.OKCancel,
                IsCloseButtonVisible = true,
                Title = Locale<MainWindowViewModel>.GetString("Add Connection"),
                CanResize = true,
            };
            var model = new Model.Connection(Guid.NewGuid().ToString("N"));
            var vm = new EditConnectionDialogViewModel() { Model = model };
            var ret = await OverlayDialog.ShowStandardAsync<EditConnectionDialog, EditConnectionDialogViewModel>(vm, null, options);
            if (ret == DialogResult.OK)
            {
                ConnectionContextManager.Instance.Add(model);
                refreshConnectionContexts();
            }
        }

        public async Task ExecuteCommand_Edit()
        {
            var options = new OverlayDialogOptions()
            {
                Buttons = DialogButton.OKCancel,
                IsCloseButtonVisible = true,
                Title = Locale<MainWindowViewModel>.GetString("Edit Connection"),
                CanResize = true,
            };
            var model = CurrentConnectionContext.Connection;
            var editModel = new Model.Connection()
            {
                Id = model.Id,
                Name = model.Name,
                ServerUrl = model.ServerUrl,
                User = model.User,
                Password = model.Password
            };
            var vm = new EditConnectionDialogViewModel() { Model = editModel };
            var ret = await OverlayDialog.ShowStandardAsync<EditConnectionDialog, EditConnectionDialogViewModel>(vm, null, options);
            if (ret == DialogResult.OK)
            {
                ConnectionContextManager.Instance.Update(editModel);
                refreshConnectionContexts();
            }
        }

        public void ExecuteCommand_Delete()
        {
            var uiDispatcher = Dispatcher.CurrentDispatcher;
            Task.Delay(100).ContinueWith(t =>
            {
                uiDispatcher.Invoke(() =>
                {
                    ConnectionContextManager.Instance.Remove(CurrentConnectionContext.Connection);
                    CurrentConnectionContext = null;
                    refreshConnectionContexts();
                });
            });
        }
    }
}
