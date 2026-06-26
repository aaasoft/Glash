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
            viewModel.PropertyChanged += (sender, e) =>
            {
                if (e.PropertyName == nameof(viewModel.CurrentLanguage))
                {
                    Task.Delay(100).ContinueWith(t =>
                    {
                        Dispatcher.Invoke(()=>
                        {
                            DataContext = null;
                            DataContext = viewModel;
                        });                        
                    });
                }
            };
        }
    }
}