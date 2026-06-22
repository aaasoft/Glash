using AtomUI.Desktop.Controls;
using GlashClientDesktop.ViewModels;

namespace GlashClientDesktop.Views
{
    public partial class MainWindow : Window
    {
        public MainWindow()
        {
            InitializeComponent();
        }

        private void Form_Submitted(object sender, FormSubmittedEventArgs e)
        {
            ((MainWindowViewModel)DataContext).Submit(e.Values);
        }
    }
}