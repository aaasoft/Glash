﻿using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Diagnostics.Metrics;
using System.Drawing;
using System.IO.Pipes;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;
using Microsoft.AspNetCore.Components.WebView.WindowsForms;
using Microsoft.Extensions.DependencyInjection;
using Quick.Localize;
using static Glash.Client.Razor.Login;

namespace Glash.Client.WinForm
{
    public partial class MainForm : Form
    {
        private FormWindowState preFormWindowState = FormWindowState.Maximized;

        public MainForm()
        {
            InitializeComponent();
            ensureOnlyOne();

            Global.Instance.Init();
            Global.Instance.LanguageChanged += Instance_LanguageChanged;
            var services = new ServiceCollection();
            services.AddWindowsFormsBlazorWebView();
            blazorWebView1.HostPage = "wwwroot/index.html";
            blazorWebView1.Services = services.BuildServiceProvider();
            blazorWebView1.RootComponents.Add<Razor.Login>("#app");

            Instance_LanguageChanged(this, EventArgs.Empty);
        }

        private void Instance_LanguageChanged(object sender, EventArgs e)
        {
            this.Text = $"{Global.Instance.TextManager.GetText(Razor.Login.Texts.Title)} v{Application.ProductVersion}";
            niMain.Text = this.Text;
        }

        private NamedPipeServerStream createNewNamedPipedServerStream(String pipeName)
        {
            return new NamedPipeServerStream(
                    pipeName,
                    PipeDirection.InOut,
                    1,
                    PipeTransmissionMode.Byte,
                    PipeOptions.Asynchronous);
        }

        private void ensureOnlyOne()
        {
            var pipeName = this.GetType().FullName;
            try
            {
                var serverStream = createNewNamedPipedServerStream(pipeName);
                AsyncCallback ac = null;
                ac = ar =>
                {
                    Invoke(() => showForm());
                    serverStream.Close();
                    serverStream = createNewNamedPipedServerStream(pipeName);
                    serverStream.BeginWaitForConnection(ac, null);
                };
                serverStream.BeginWaitForConnection(ac, null);
            }
            catch
            {
                try
                {
                    var clientStream = new NamedPipeClientStream(pipeName);
                    clientStream.Connect();
                    clientStream.Close();
                }
                finally
                {
                    this.DialogResult = DialogResult.Cancel;
                    this.Close();
                    Environment.Exit(0);
                }
            }
        }
        
        private void showForm()
        {
            WindowState = preFormWindowState;
            ShowInTaskbar = true;            
            Activate();
        }

        private void niMain_MouseClick(object sender, MouseEventArgs e)
        {
            showForm();
        }

        private void MainForm_SizeChanged(object sender, EventArgs e)
        {
            if (WindowState == FormWindowState.Minimized)
            {
                this.ShowInTaskbar = false;
            }
            else
            {
                preFormWindowState = WindowState;
            }
        }

    }
}