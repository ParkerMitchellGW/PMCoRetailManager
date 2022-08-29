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
                    services.AddSingleton<IConfigHelper, ConfigHelper>();

                    services.AddTransient<IProductEndpoint, ProductEndpoint>();
                    services.AddTransient<ISaleEndpoint, SaleEndpoint>();

                    RegisterAllViewModels(services);
                    ConfigureAutoMapper(services);
                })
                .Build();
        }

        private void ConfigureAutoMapper(IServiceCollection services)
        {
            var config = new MapperConfiguration(cfg => {
                cfg.CreateMap<ProductModel, ProductDisplayModel>();
                cfg.CreateMap<CartItemModel, CartItemDisplayModel>();
            });
            var mapper = config.CreateMapper();

            services.AddSingleton(mapper);
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

        public static T GetRequiredService<T>() where T : notnull
        {
            return AppHost!.Services.GetRequiredService<T>();
        }
    }
}