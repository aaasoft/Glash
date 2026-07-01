using Avalonia.Interactivity;
using GlashClientDesktop.ViewModels;
using Ursa.Controls;

namespace GlashClientDesktop.Views
{
    public partial class MainWindow : UrsaWindow
    {
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
        }
    }
}