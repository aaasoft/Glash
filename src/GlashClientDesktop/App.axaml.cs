using Avalonia;
using Avalonia.Controls.ApplicationLifetimes;
using Avalonia.Markup.Xaml;
using GlashClientDesktop.Core.ProxyTypes;
using GlashClientDesktop.ViewModels;
using GlashClientDesktop.Views;
using Quick.LiteDB.Plus;
using System.Globalization;

namespace GlashClientDesktop
{
    public partial class App : Application
    {
        public override void Initialize()
        {
            Quick.Protocol.WebSocket.Client.QpWebSocketClientOptions.RegisterUriSchema();
            Quick.Protocol.Http.Client.QpHttpClientOptions.RegisterUriSchema();
            ProxyTypeManager.Instance.Init();

            // Read dbFile path from environment variable, default to "Config.litedb" if not set
            var dbFile = Environment.GetEnvironmentVariable("GLASH_DB_FILE_PATH") ?? "Config.litedb";
            #if DEBUG
            dbFile = Path.Combine(Path.GetDirectoryName(typeof(Program).Assembly.Location), dbFile);
            #endif
            ConfigDbContext.Init(dbFile, modelBuilder =>
            {
                Global.Instance.OnModelCreating(modelBuilder);
            });
            ConfigDbContext.CacheContext.LoadCache();

            AvaloniaXamlLoader.Load(this);
        }

        public override void OnFrameworkInitializationCompleted()
        {
            if (ApplicationLifetime is IClassicDesktopStyleApplicationLifetime desktop)
            {
                desktop.MainWindow = new MainWindow
                {
                    DataContext = new MainWindowViewModel(),
                };
            }

            base.OnFrameworkInitializationCompleted();
        }
    }
}