using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using AcademiaDoZe.Application.Interfaces;
using AcademiaDoZe.Application.Services;
using AcademiaDoZe.Domain.Repositories;
using AcademiaDoZe.Infrastructure.Data;
using AcademiaDoZe.Infrastructure.Repositories;
using Microsoft.Extensions.DependencyInjection;
namespace AcademiaDoZe.Application.DependencyInjection
{
    public static class ApplicationDependencyInjection
    {
        public static IServiceCollection AddApplicationServices(this IServiceCollection services)
        {
            // Registra os serviços da camada de aplicação
            services.AddTransient<ILogradouroService, LogradouroService>();
            services.AddTransient<IColaboradorService, ColaboradorService>();
            services.AddTransient<IAlunoService, AlunoService>();
            services.AddTransient<IMatriculaService, MatriculaService>();
            services.AddTransient<IPlanoService, PlanoService>(); 
            
            services.AddTransient(provider =>
            {
                var config = provider.GetRequiredService<RepositoryConfig>();
                return (Func<ILogradouroRepository>)(() => new LogradouroRepository(config.ConnectionString, (DatabaseType)config.DatabaseType));
            });

            services.AddTransient(provider =>
            {
                var config = provider.GetRequiredService<RepositoryConfig>();
                return (Func<IPlanoRepository>)(() => new PlanoRepository(config.ConnectionString, (DatabaseType)config.DatabaseType));
            });

            services.AddTransient(provider =>
            {
                var config = provider.GetRequiredService<RepositoryConfig>();
                return (Func<IColaboradorRepository>)(() => new ColaboradorRepository(config.ConnectionString, (DatabaseType)config.DatabaseType));
            });
            
            services.AddTransient(provider =>
            {
            var config = provider.GetRequiredService<RepositoryConfig>();
            return (Func<IAlunoRepository>)(() => new AlunoRepository(config.ConnectionString, (DatabaseType)config.DatabaseType));
            });

            services.AddTransient(provider =>
            {
                var config = provider.GetRequiredService<RepositoryConfig>();
                return (Func<IMatriculaRepository>)(() => new MatriculaRepository(config.ConnectionString, (DatabaseType)config.DatabaseType));
            });

            return services;
        }
    }
}