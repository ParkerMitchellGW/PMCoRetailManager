using AutoMapper;
using Microsoft.Extensions.DependencyInjection;
using PRMDesktopUI.Library.Api;
using PRMDesktopUI.Library.Helpers;
using PRMDesktopUI.Library.Models;
using PRMDesktopUI.Models;
using PRMDesktopUI.Services;
using PRMDesktopUI.Views;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace PRMDesktopUI
{
    public static class DependencyInjection
    {
        /// <summary>
        /// Registers all necessary services for use in the Dependency Injection system.
        /// Add any services in this method.
        /// </summary>
        /// <param name="services">The IServiceCollection to add all required services to.</param>
        public static void ConfigureDependencyInjection(IServiceCollection services)
        {
            services.AddSingleton<ShellView>();
            services.AddSingleton<IAPIHelper, APIHelper>();
            services.AddSingleton<ILoggedInUserModel, LoggedInUserModel>();
            services.AddSingleton<IConfigHelper, ConfigHelper>();
            services.AddSingleton<IStatusInfoDisplay, StatusInfoDisplay>();

            services.AddTransient<IProductEndpoint, ProductEndpoint>();
            services.AddTransient<ISaleEndpoint, SaleEndpoint>();
            services.AddTransient<IUserEndpoint, UserEndpoint>();

            RegisterAllViewModels(services);
            ConfigureAutoMapper(services);
        }

        private static void ConfigureAutoMapper(IServiceCollection services)
        {
            var config = new MapperConfiguration(cfg => {
                cfg.CreateMap<ProductModel, ProductDisplayModel>();
                cfg.CreateMap<CartItemModel, CartItemDisplayModel>();
            });
            var mapper = config.CreateMapper();

            services.AddSingleton(mapper);
        }

        /// <summary>
        /// This registers all class types which end with the string <b>ViewModel</b>
        /// to the service collection for use with dependency collection.
        /// </summary>
        /// <param name="services">The service collection to add all of our viewmodels to.</param>
        private static void RegisterAllViewModels(IServiceCollection services)
        {
            App.Current.GetType().Assembly.GetTypes()
                    .Where(type => type.IsClass && type.Name.EndsWith("ViewModel"))
                    .ToList()
                    .ForEach(viewModelType => services.AddTransient(viewModelType));
        }
    }
}
