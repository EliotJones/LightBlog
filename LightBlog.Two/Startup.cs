using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Hosting;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Logging;
using LightBlog.Models;
using LightBlog.Models.Images;
using LightBlog.Models.Posts;
using Microsoft.AspNetCore.Mvc.Infrastructure;

namespace LightBlog.Two
{
    public class Startup
    {
        public Startup(IHostingEnvironment env)
        {
            var builder = new ConfigurationBuilder()
                .SetBasePath(env.ContentRootPath)
                .AddJsonFile("appsettings.json", optional: false, reloadOnChange: true)
                .AddJsonFile($"appsettings.{env.EnvironmentName}.json", optional: true)
                .AddEnvironmentVariables();

            Configuration = builder.Build();
        }

        public IConfigurationRoot Configuration { get; }

        // This method gets called by the runtime. Use this method to add services to the container.
        public void ConfigureServices(IServiceCollection services)
        {
            // Add framework services.
            services.AddMvc();

            services.AddOptions();

            services.Configure<SiteOptions>(Configuration.GetSection("SiteOptions"));
            services.Configure<UploadOptions>(Configuration.GetSection("UploadOptions"));


            services.AddSingleton<IActionContextAccessor, ActionContextAccessor>();
            services.AddTransient<IPostRepository, PostRepository>();
            services.AddTransient<IRssFeedFactory, RssFeedFactory>();
            services.AddTransient<IImageRepository, ImageRepository>();
            services.AddTransient<IUploadAuthentication, UploadAuthentication>();

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

                routes.MapRoute(
                    name: "post",
                    template: "{year:int}/{month:int}/{name}",
                    defaults: new { controller = "Post", action = "Index" });
            });
        }
    }
}
