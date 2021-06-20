using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Logging;
using Microsoft.OpenApi.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.EntityFrameworkCore;
using DAL.Data;
using Models.Hubs;
using Services;
using Interfaces;
using BL;
using System.IO;
using System.Diagnostics;

namespace AirportSystemFinalProjectServer
{
    public class Startup
    {
        public Startup(IConfiguration configuration)
        {
            Configuration = configuration;
        }

        public IConfiguration Configuration { get; }
        public IRepository IDbRepository { get; set; }
        public IWebHostEnvironment Env { get;set; }

        // This method gets called by the runtime. Use this method to add services to the container.
        public void ConfigureServices(IServiceCollection services)
        {
            services.AddTransient<IRepository, Repository>();
            string connectionString = Configuration.GetConnectionString("AirportDbContext");
            services.AddDbContext<AirportDbContext>(options => options.UseSqlServer(connectionString));

            services.AddSingleton<ITakeoffsManager, TakeoffsManager>();
            services.AddSingleton<ILandingsManager, LandingsManager>();
            services.AddSingleton<IAirport, BL.Airport>();
            services.AddSingleton<IGenerator, PlanesGenerator>();
            services.AddControllers();
            services.AddSwaggerGen(c =>
            {
                c.SwaggerDoc("v1", new OpenApiInfo { Title = "AirportSystemFinalProjectServer", Version = "v1" });
            });


            services.AddCors(o => o.AddPolicy("CorsPolicy", builder =>
            {
                builder
                .AllowAnyMethod()
                .AllowAnyHeader()
                .AllowCredentials()
                .WithOrigins("http://localhost:4200");
            }));

            services.AddSignalR();

            services.AddDbContext<AirportDbContext>(options =>
                    options.UseSqlServer(Configuration.GetConnectionString("AirportDbContext")));
            
        }

        // This method gets called by the runtime. Use this method to configure the HTTP request pipeline.
        public void Configure(IApplicationBuilder app, IWebHostEnvironment env, IHostApplicationLifetime lifetime, IRepository repository)
        {
            Env = env;
            if (env.IsDevelopment())
            {
                app.UseDeveloperExceptionPage();
                //app.UseSwagger();
                //app.UseSwaggerUI(c => c.SwaggerEndpoint("/swagger/v1/swagger.json", "AirportSystemFinalProjectServer v1"));
            }

            IDbRepository = repository;

            lifetime.ApplicationStopped.Register(OnAppStopped);
            lifetime.ApplicationStopping.Register(OnAppStopped);

            app.UseRouting();
            

            app.UseAuthorization();

            app.UseCors("CorsPolicy");

            app.UseEndpoints(endpoints =>
            {
                endpoints.MapHub<BroadcastHub>("/notify");
            });

            app.UseEndpoints(endpoints =>
            {
                endpoints.MapControllers();
            });
        }

      
        private void OnAppStopped()
        {
            string path = $"AppLog.txt";
            string contents = $"App stopped at {DateTime.Now}";
            File.AppendAllText(path, contents);
            IDbRepository.RemoveGarbagePlanes();           
        }
    }
}
