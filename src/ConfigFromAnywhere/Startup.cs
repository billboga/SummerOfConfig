using ConfigFromAnywhere.Configuration;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Hosting;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Logging;
using System;

namespace ConfigFromAnywhere
{
    public class Startup
    {
        public Startup(IHostingEnvironment env)
        {
            var gpsConfiguration = new ConfigurationBuilder()
                .SetBasePath(env.ContentRootPath)
                .AddJsonFile("gps.config.json")
                .AddJsonFile($"gps.config.{env.EnvironmentName}.json", optional: true, reloadOnChange: true)
                .Build();

            var builder = new ConfigurationBuilder()
                .SetBasePath(env.ContentRootPath)
                .AddGpsProgress(
                    gpsConfiguration,
                    "latitudeStart",
                    "longitudeStart",
                    "latitudeEnd",
                    "longitudeEnd",
                    out DistancePositionChanged)
                .AddEnvironmentVariables();

            Configuration = builder.Build();
        }


        public IConfigurationRoot Configuration { get; }

        /// <summary>
        /// Latitude, longitude, accuracy.
        /// </summary>
        public static Action<double, double, string> DistancePositionChanged;

        // This method gets called by the runtime. Use this method to add services to the container.
        public void ConfigureServices(IServiceCollection services)
        {
            // Add framework services.
            services.AddMvc();
            services.AddSingleton<IConfigurationRoot>(Configuration);
        }

        // This method gets called by the runtime. Use this method to configure the HTTP request pipeline.
        public void Configure(IApplicationBuilder app, IHostingEnvironment env, ILoggerFactory loggerFactory)
        {
            loggerFactory.AddConsole(Configuration.GetSection("Logging"));
            loggerFactory.AddDebug();

            if (env.IsDevelopment())
            {
                app.UseDeveloperExceptionPage();
                app.UseBrowserLink();
            }
            else
            {
                app.UseExceptionHandler("/Home/Error");
            }

            app.UseStaticFiles();

            app.UseMvc(routes =>
            {
                routes.MapRoute(
                    name: "default",
                    template: "{controller=Home}/{action=Index}/{id?}");
            });
        }
    }
}
