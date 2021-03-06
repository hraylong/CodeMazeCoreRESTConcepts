﻿using AccountOwner.ApiServer.Extensions;
using AccountOwner.ApiServer.Filters;
using AccountOwner.Entities;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Diagnostics;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.HttpOverrides;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using NLog;
using System;
using System.IO;
using System.Net;

namespace AccountOwner.ApiServer
{
	public class Startup
	{
		public Startup(IConfiguration configuration)
		{
			LogManager.LoadConfiguration(String.Concat(Directory.GetCurrentDirectory(), "/nlog.config"));
			Configuration = configuration;
		}

		public IConfiguration Configuration { get; }

		// This method gets called by the runtime. Use this method to add services to the container.
		public void ConfigureServices(IServiceCollection services)
		{
			services.AddDbContext<RepositoryContext>(opts =>
					opts.UseMySql(Configuration.GetConnectionString("mysqlconnection"),
					 options => options.MigrationsAssembly("AccountOwner.ApiServer")
					 ));

			services.ConfigureCors();

			services.ConfigureIISIntegration();

			services.ConfigureLoggerService();

			services.ConfigureMySqlContext(Configuration);

			services.ConfigureRepositoryWrapper();

			services.AddControllers(config =>
			{
				config.RespectBrowserAcceptHeader = true;
				config.ReturnHttpNotAcceptable = false;
			})
			.AddXmlDataContractSerializerFormatters()
			.AddNewtonsoftJson();

			services.AddCustomMediaTypes();

			services.AddScoped<ValidateMediaTypeAttribute>();
		}

		// This method gets called by the runtime. Use this method to configure the HTTP request pipeline.
		public void Configure(IApplicationBuilder app, IWebHostEnvironment env)
		{
			if (env.IsDevelopment())
			{
				app.UseDeveloperExceptionPage();
			}
			else
			{
				// The default HSTS value is 30 days. You may want to change this for production scenarios, see https://aka.ms/aspnetcore-hsts.
				app.UseHsts();
			}

			app.UseExceptionHandler(appError =>
			{
				appError.Run(async context =>
				{
					context.Response.StatusCode = (int)HttpStatusCode.InternalServerError;
					context.Response.ContentType = "application/json";

					var contextFeature = context.Features.Get<IExceptionHandlerFeature>();
					if (contextFeature != null)
					{
						Console.WriteLine($"Something went wrong: {contextFeature.Error}");

						await context.Response.WriteAsync(new
						{
							context.Response.StatusCode,
							Message = "Internal Server Error."
						}.ToString());
					}
				});
			});

			app.UseHttpsRedirection();			

			app.UseForwardedHeaders(new ForwardedHeadersOptions
			{
				ForwardedHeaders = ForwardedHeaders.All
			});

			app.UseStaticFiles();

			app.UseRouting();

			app.UseCors("CorsPolicy");

			app.UseEndpoints(endpoints =>
			{
				endpoints.MapControllers();
			});
		}
	}
}
