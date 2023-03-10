namespace Glash.Client.WinForm
{
    internal static class Program
    {
        /// <summary>
        ///  The main entry point for the application.
        /// </summary>
        [STAThread]
        static void Main()
        {
            Quick.Protocol.QpAllClients.RegisterUriSchema();
            
            ApplicationConfiguration.Initialize();
            Application.Run(new MainForm());
        }
    }
}