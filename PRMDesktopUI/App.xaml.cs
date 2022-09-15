using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.DependencyInjection;
using System;
using System.Collections.Generic;
using System.Configuration;
using System.Data;
using System.Linq;
using System.Threading.Tasks;
using System.Windows;
using PRMDesktopUI.Views;
using PRMDesktopUI.ViewModels;
using PRMDesktopUI.Library.Api;
using PRMDesktopUI.Library.Models;
using PRMDesktopUI.Library.Helpers;
using AutoMapper;
using PRMDesktopUI.Models;
using PRMDesktopUI.Services;

namespace PRMDesktopUI
{
    public partial class App : Application
    {
        public static IHost? AppHost { get; set; }

        public App()
        {
            AppHost = Host.CreateDefaultBuilder()
                .ConfigureServices((hostContext, services) =>
                {
                    DependencyInjection.ConfigureDependencyInjection(services);
                })
                .Build();
        }

        protected override async void OnStartup(StartupEventArgs e)
        {
            await AppHost!.StartAsync();

            var startupForm = GetRequiredService<ShellView>();
            startupForm.WindowStartupLocation = WindowStartupLocation.CenterScreen;
            startupForm.Show();

            base.OnStartup(e);
        }

        public static T GetRequiredService<T>() where T : notnull
        {
            return AppHost!.Services.GetRequiredService<T>();
        }
    }
}