using Avalonia;
using Avalonia.Controls.ApplicationLifetimes;
using Avalonia.Markup.Xaml;
using GlashClientDesktop.Core.ProxyTypes;
using GlashClientDesktop.ViewModels;
using GlashClientDesktop.Views;
using Quick.LiteDB.Plus;
using Quick.Utils;
using Ursa.Controls;

namespace GlashClientDesktop
{
    public partial class App : Application
    {
        public override void Initialize()
        {
            AvaloniaXamlLoader.Load(this);
        }

        public override void OnFrameworkInitializationCompleted()
        {
            if (ApplicationLifetime is IClassicDesktopStyleApplicationLifetime desktop)
            {
                try
                {
                    Quick.Protocol.WebSocket.Client.QpWebSocketClientOptions.RegisterUriSchema();
                    Quick.Protocol.Http.Client.QpHttpClientOptions.RegisterUriSchema();
                    ProxyTypeManager.Instance.Init();
                    var configFolder = Path.Combine(Environment.GetFolderPath(Environment.SpecialFolder.ApplicationData), nameof(GlashClientDesktop));
                    if (!Directory.Exists(configFolder))
                        Directory.CreateDirectory(configFolder);
                    var dbFile = Path.Combine(configFolder, "Config.litedb");

                    ConfigDbContext.Init(dbFile, modelBuilder =>
                    {
                        modelBuilder.Entity<Model.Connection>();
                    });
                    ConfigDbContext.CacheContext.LoadCache();

                }
                catch (Exception ex)
                {
                    MessageBox.ShowAsync(ExceptionUtils.GetExceptionMessage(ex), "Error", MessageBoxIcon.Error);
                    return;
                }
                desktop.MainWindow = new MainWindow
                {
                    DataContext = new MainWindowViewModel(),
                };
            }

            base.OnFrameworkInitializationCompleted();
        }
    }
}