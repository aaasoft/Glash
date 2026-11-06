using System.Diagnostics.Eventing.Reader;
using Avalonia.Interactivity;
using GlashClientDesktop.Core;
using GlashClientDesktop.ViewModels;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Hosting;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Logging;
using Quick.Localize;
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
            GettextResourceManager.CurrentCultureChanged += (sender, e) =>
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
            int[] tryPorts = [80, 8000, 60080];
            foreach (var i in tryPorts)
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
                    ReverseProxyManager.Instance.ListenPort = i;
                    break;
                }
                catch { }
            }
            if (ReverseProxyManager.Instance.ListenPort < 0)
                MessageBox.ShowAsync(Locale<MainWindow>.GetString("All web proxy ports not avaliable, Web proxy domain will not avaliable."), Locale<MainWindow>.GetString("Warn"));
            else if (ReverseProxyManager.Instance.ListenPort != 80)
                MessageBox.ShowAsync(Locale<MainWindow>.GetString("Web proxy listen port 80 error, will use other port for web proxy."), Locale<MainWindow>.GetString("Warn"));
        }

        protected override void OnClosed(EventArgs e)
        {
            app?.DisposeAsync();
            base.OnClosed(e);
        }
    }
}