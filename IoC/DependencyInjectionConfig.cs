using Application.Services.Impl;
using Application.Services.Intf;
using Domain.Config;
using Infrastructure.Repositories.Impl;
using Infrastructure.Repositories.Intf;
using Infrastructure.UnitOfWork.Impl;
using Infrastructure.UnitOfWork.Intf;
using Microsoft.Extensions.DependencyInjection;

namespace IoC
{
    public static class DependencyInjectionConfig
    {
        /// <summary>
        /// Registers dependency injections in the container.
        /// </summary>
        /// <param name="services"></param>
        /// <returns><see cref="IServiceCollection"/> with registered dependency injections.</returns> 
        public static IServiceCollection ResolveDependencies(this IServiceCollection services)
        {
            services.AddScoped<IRepository, Repository>();
            services.AddScoped<IService, Service>();
            services.AddScoped<IUnitOfWork, UnitOfWork>();
            services.AddAutoMapper(typeof(AutomapperConfig));

            return services;
        }

    }
}

