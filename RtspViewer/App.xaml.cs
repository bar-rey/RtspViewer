using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using RtspViewer.GUI.Models;
using RtspViewer.GUI.Services.Dialogs;
using RtspViewer.GUI.Services.Dialogs.Base;
using RtspViewer.GUI.Services.IO;
using RtspViewer.GUI.ViewModels;
using RtspViewer.GUI.Views;
using RtspViewer.ViewModels;
using System;
using System.Collections.Generic;
using System.Configuration;
using System.Data;
using System.Linq;
using System.Threading.Tasks;
using System.Windows;

namespace RtspViewer
{
    /// <summary>
    /// Interaction logic for App.xaml
    /// </summary>
    public partial class App : Application
    {
        private readonly IHost _host;

        public App()
        {
            var settings = RtspViewer.Properties.Settings.Default;
            _host = Host.CreateDefaultBuilder()
            .ConfigureServices((services) =>
            {
                services.AddSingleton<MainWindow>();
                services.AddSingleton<DatabaseContext>();
                services.AddTransient<IRtspConnection, RtspConnection>();
                services.AddTransient<IDialogService, DialogService>();
                services.AddTransient<ISourceDialogService, SourceDialogService>();
                services.AddTransient<IFolderDialogService, FolderDialogService>();
                services.AddTransient<IFileDialogService, FileDialogService>();
                services.AddTransient<ICsvService<Source>, CsvService<Source>>();
                services.AddTransient<ICsvService<Report>, CsvService<Report>>();
                services.AddTransient<MainWindowViewModel>();
            })
            .Build();
        }

        protected override void OnStartup(StartupEventArgs e)
        {
            base.OnStartup(e);
            _host.Start();
            Current.MainWindow = _host.Services.GetRequiredService<MainWindow>();
            Current.MainWindow.Closed += OnMainWindowClosed!;
            Current.MainWindow.Show();
        }

        private void OnMainWindowClosed(object sender, EventArgs e)
        {
            Shutdown();
        }

        protected override void OnExit(ExitEventArgs e)
        {
            _host.StopAsync();
            _host.Dispose();
            base.OnExit(e);
        }
    }
}
