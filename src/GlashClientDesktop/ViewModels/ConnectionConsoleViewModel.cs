using System.Reactive;
using GlashClientDesktop.Core;
using Quick.Localize;
using Quick.Utils;
using ReactiveUI;
using Ursa.Controls;

namespace GlashClientDesktop.ViewModels;

public class ConnectionConsoleViewModel : ViewModelBase
{
    public string Text_CurrentNotConntected =>Locale.GetString("Current not connected.");
    private ConnectionContext _Model;
    public ConnectionContext Model
    {
        get => _Model;
        set => this.RaiseAndSetIfChanged(ref _Model, value);
    }

    public ReactiveCommand<Unit, Unit> StartCommand { get; }
    public ReactiveCommand<Unit, Unit> StopCommand { get; }
    public ReactiveCommand<Unit, Unit> LogCommand { get; }

    private string _LoadingMessage;
    public string LoadingMessage
    {
        get => _LoadingMessage;
        set => this.RaiseAndSetIfChanged(ref _LoadingMessage, value);
    }
    private bool _IsLoading;
    public bool IsLoading
    {
        get => _IsLoading;
        set => this.RaiseAndSetIfChanged(ref _IsLoading, value);
    }

    public ConnectionConsoleViewModel()
    {
        StartCommand = ReactiveCommand.CreateFromTask(ExecuteCommand_Start);
        StopCommand = ReactiveCommand.CreateFromTask(ExecuteCommand_Stop);
        LogCommand = ReactiveCommand.CreateFromTask(ExecuteCommand_Log);
    }

    public async Task ExecuteCommand_Start()
    {
        try
        {
            LoadingMessage = Locale.GetString("Starting...");
            IsLoading = true;
            await Model.Start();
        }
        catch (Exception ex)
        {
            await MessageBox.ShowAsync(Locale.GetString("Start error,reason: {0}", ExceptionUtils.GetExceptionMessage(ex)), Locale.GetString("Error"), MessageBoxIcon.Warning);
        }
        finally
        {
            IsLoading = false;
        }
    }

    public async Task ExecuteCommand_Stop()
    {
        try
        {
            LoadingMessage = Locale.GetString("Stoping...");
            IsLoading = true;
            await Task.Run(Model.Stop);
        }
        catch (Exception ex)
        {
            await MessageBox.ShowAsync(Locale.GetString("Stop error,reason: {0}", ExceptionUtils.GetExceptionMessage(ex)), Locale.GetString("Error"), MessageBoxIcon.Warning);
        }
        finally
        {
            IsLoading = false;
        }
    }

    public async Task ExecuteCommand_Log()
    {
        await MessageBox.ShowAsync(string.Join(Environment.NewLine, Model.Logs), Locale.GetString("Logs"), MessageBoxIcon.Information);
    }
}
