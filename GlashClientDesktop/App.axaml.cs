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
                    Quick.Protocol.Tcp.QpTcpClientOptions.RegisterUriSchema();
                    Quick.Protocol.WebSocket.Client.QpWebSocketClientOptions.RegisterUriSchema();
                    Quick.Protocol.Http.Client.QpHttpClientOptions.RegisterUriSchema();

                    ProxyTypeManager.Instance.Init();
                    ConfigDbContext.Init("Config.litedb", modelBuilder =>
                    {
                        modelBuilder.Entity<Model.Connection>(c =>
                            c.Include(t => t.Id).
                            Include(t => t.Name).
                            Include(t => t.ServerUrl).
                            Include(t => t.User).
                            Include(t => t.Password).
                            Include(t => t.UsePackageType)
                        );
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