﻿using AccountOwner.Contracts;
using AccountOwner.Entities;
using AccountOwner.Helpers;
using AccountOwner.Models;
using AccountOwner.LoggerService;
using Microsoft.AspNetCore.Builder;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using AccountOwner.Repository;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Formatters;
using System.Linq;

namespace AccountOwner.ApiServer.Extensions
{
	public static class ServiceExtensions
	{
		public static void ConfigureCors(this IServiceCollection services)
		{
			services.AddCors(options =>
			{
				options.AddPolicy("CorsPolicy",
					builder => builder.WithOrigins("http://localhost:5000", "http://localhost:8080")
					.AllowAnyMethod()
					.AllowAnyHeader()
					.AllowCredentials());
			});
		}

		public static void ConfigureIISIntegration(this IServiceCollection services)
		{
			services.Configure<IISOptions>(options =>
			{

			});
		}

		public static void ConfigureLoggerService(this IServiceCollection services)
		{
			services.AddSingleton<ILoggerManager, LoggerManager>();
		}

		public static void ConfigureMySqlContext(this IServiceCollection services, IConfiguration config)
		{
			var connectionString = config["mysqlconnection:connectionString"];
			services.AddDbContext<RepositoryContext>(o => o.UseMySql(connectionString));
		}

		public static void ConfigureRepositoryWrapper(this IServiceCollection services)
		{
			services.AddScoped<ISortHelper<Owner>, SortHelper<Owner>>();
			services.AddScoped<ISortHelper<Account>, SortHelper<Account>>();

			services.AddScoped<IDataShaper<Owner>, DataShaper<Owner>>();
			services.AddScoped<IDataShaper<Account>, DataShaper<Account>>();

			services.AddScoped<IRepositoryWrapper, RepositoryWrapper>();
		}

		public static void AddCustomMediaTypes(this IServiceCollection services)
		{
			services.Configure<MvcOptions>(config =>
			{
				var newtonsoftJsonOutputFormatter = config.OutputFormatters
						.OfType<NewtonsoftJsonOutputFormatter>()?.FirstOrDefault();

				if (newtonsoftJsonOutputFormatter != null)
				{
					newtonsoftJsonOutputFormatter.SupportedMediaTypes.Add("application/vnd.codemaze.hateoas+json");
				}

				var xmlOutputFormatter = config.OutputFormatters
						.OfType<XmlDataContractSerializerOutputFormatter>()?.FirstOrDefault();

				if (xmlOutputFormatter != null)
				{
					xmlOutputFormatter.SupportedMediaTypes.Add("application/vnd.codemaze.hateoas+xml");
				}
			});
		}
	}
}
