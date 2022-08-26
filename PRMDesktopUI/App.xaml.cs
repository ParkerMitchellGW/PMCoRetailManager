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
                    services.AddSingleton<ShellView>();
                    services.AddSingleton<IAPIHelper, APIHelper>();
                    services.AddSingleton<ILoggedInUserModel, LoggedInUserModel>();

                    RegisterAllViewModels(services);
                })
                .Build();
        }

        private void RegisterAllViewModels(IServiceCollection services)
        {
            GetType().Assembly.GetTypes()
                    .Where(type => type.IsClass && type.Name.EndsWith("ViewModel"))
                    .ToList()
                    .ForEach(viewModelType => services.AddTransient(viewModelType));
        }

        protected override async void OnStartup(StartupEventArgs e)
        {
            await AppHost!.StartAsync();

            var startupForm = AppHost.Services.GetRequiredService<ShellView>();
            startupForm.WindowStartupLocation = WindowStartupLocation.CenterScreen;
            startupForm.Show();

            base.OnStartup(e);
        }
    }
}