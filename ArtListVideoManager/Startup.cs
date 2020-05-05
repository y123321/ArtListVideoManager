using ArtListVideoManager.DAL;
using ArtListVideoManager.DAL.Entities;
using ArtListVideoManager.Interfaces;
using ArtListVideoManager.Model;
using ArtListVideoManager.Services;
using AutoMapper;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.SpaServices.AngularCli;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.FileProviders;
using Microsoft.Extensions.Hosting;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Reflection;
using System.Text.Json;

namespace ArtListVideoManager
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
            services.AddLogging();

            services.AddControllersWithViews()
                .AddJsonOptions(options =>
                    {
                        options.JsonSerializerOptions.Converters.Add(new System.Text.Json.Serialization.JsonStringEnumConverter());
                            
                    });
            var conString = Configuration.GetConnectionString("DefaultConnection");
            services.AddDbContext<VideoManagerContext>(options =>
            {
                options.UseSqlServer(conString,
                     b => b.MigrationsAssembly(Assembly.GetExecutingAssembly().GetName().Name));
            });
            services.AddAutoMapper(config =>
            {
                config.CreateMap<Video, VideoModel>().ReverseMap();
                config.CreateMap<string, ThumbnailLink>()
                    .ForMember(s => s.Link, opt => opt.MapFrom(s => s));

            }, typeof(Video), typeof(VideoModel));
            services.AddScoped<IVideoService, VideoService>();
            services.AddScoped<IVideoConverter, FFMpegConverter>();
            services.AddScoped<IVideoRepository, VideoRepository>();
            // In production, the Angular files will be served from this directory
            services.AddSpaStaticFiles(configuration =>
            {
                configuration.RootPath = "ClientApp/dist";
            });
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
                app.UseExceptionHandler("/Error");
            }

            app.UseStaticFiles(new StaticFileOptions
            {
                FileProvider = new PhysicalFileProvider(
                Path.Combine(Directory.GetCurrentDirectory(), "StaticFiles")),
                RequestPath = "/StaticFiles"
            });
            if (!env.IsDevelopment())
            {
                app.UseSpaStaticFiles();
            }
            app.UseRouting();

            app.UseEndpoints(endpoints =>
            {
                endpoints.MapControllerRoute(
                    name: "default",
                    pattern: "{controller}/{action=Index}/{id?}");
            });
            app.UseSpa(spa =>
            {
                // To learn more about options for serving an Angular SPA from ASP.NET Core,
                // see https://go.microsoft.com/fwlink/?linkid=864501

                spa.Options.SourcePath = "ClientApp";

                if (env.IsDevelopment())
                {
                    spa.UseAngularCliServer(npmScript: "start");
                }
            });
        }
    }
}
