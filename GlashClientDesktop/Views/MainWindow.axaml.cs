using Avalonia.Interactivity;
using GlashClientDesktop.ViewModels;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Hosting;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Logging;
using Ursa.Controls;

namespace GlashClientDesktop.Views
{
    public partial class MainWindow : UrsaWindow
    {
        private WebApplication app;

        public MainWindow()
        {
            InitializeComponent();
        }

        protected override void OnLoaded(RoutedEventArgs e)
        {
            base.OnLoaded(e);
            var viewModel = (MainWindowViewModel)DataContext;
            viewModel.RefreshConnectionContexts();
            Quick.Localize.GettextResourceManager.CurrentCultureChanged += (sender, e) =>
            {
                Task.Run(() =>
                {
                    Dispatcher.Invoke(() =>
                    {
                        DataContext = null;
                        DataContext = viewModel;
                    });
                });
            };
            for (var i = 80; i < 1000; i++)
            {
                try
                {
                    var builder = WebApplication.CreateSlimBuilder();
                    builder.Logging.ClearProviders();
                    builder.WebHost
                            .UseUrls([$"http://127.0.0.1:{i}"]);
                    Core.ReverseProxyManager.Instance.Load(builder.Services.AddReverseProxy());
                    app = builder.Build();
                    app.MapReverseProxy();
                    app.Start();
                    break;
                }
                catch { }
            }
        }

        protected override void OnClosed(EventArgs e)
        {
            app?.DisposeAsync();
            base.OnClosed(e);
        }
    }
}