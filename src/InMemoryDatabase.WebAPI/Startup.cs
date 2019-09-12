using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Hosting;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Swashbuckle.AspNetCore.Swagger;
using System;
using System.IO;
using System.Reflection;

namespace InMemoryDatabase.WebAPI
{
	public class Startup
	{
		public Startup(IConfiguration configuration)
		{
			Configuration = configuration;
		}

		public IConfiguration Configuration { get; }

		// This method gets called by the runtime. Use this method to add services to the container.
		public void ConfigureServices(IServiceCollection services)
		{
			services.AddCors();
			services.AddMvc();

			services.AddRouting(options =>
			{
				options.LowercaseUrls = true;
				options.LowercaseQueryStrings = true;
			});

			services.AddSwaggerGen(options =>
			{
				options.DescribeAllParametersInCamelCase();
				options.SwaggerDoc("v1", new Info
				{
					Title = "InMemoryDatabase.WebAPI",
					Version = "v1",
					Description = "InMemoryDatabase.WebAPI"
				});

				var appPath = AppDomain.CurrentDomain.BaseDirectory;

				var entryAssembly = Assembly.GetEntryAssembly();
				var aseemblyName = entryAssembly?.GetName();
				var appName = aseemblyName?.Name;

				var filePath = Path.Combine(appPath, $"{appName ?? "InMemoryDatabase.WebAPI"}.xml");

				options.IncludeXmlComments(filePath);
			});
		}

		// This method gets called by the runtime. Use this method to configure the HTTP request pipeline.
		public void Configure(IApplicationBuilder app, IHostingEnvironment env)
		{
			if (env.IsDevelopment())
			{
				app.UseDeveloperExceptionPage();
			}

			app.UseStaticFiles();
			app.UseMvcWithDefaultRoute();

			app.UseSwagger(setupAction: null);
			app.UseSwaggerUI(options =>
			{
				options.SwaggerEndpoint("/swagger/v1/swagger.json", "InMemoryDatabase.WebAPI");
				options.RoutePrefix = "swagger";
			});
		}
	}
}
