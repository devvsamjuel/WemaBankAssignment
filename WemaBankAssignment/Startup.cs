using AutoMapper;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.HttpsPolicy;
using Microsoft.AspNetCore.Identity;
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
using WemaBankAssignment.Data;
using WemaBankAssignment.Entities;
using WemaBankAssignment.Middleware;

namespace WemaBankAssignment
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
            services.AddControllers().AddNewtonsoftJson();
            services.AddSwaggerDoc();
            services.AddAutoMapper(AppDomain.CurrentDomain.GetAssemblies());
            services.AddDatabaseContexts(Configuration);
            services.AddCorPolicies();
            services.ConfigureSettings(Configuration);
            services.AddAuthenticationServices(Configuration);
            services.AddApiServices();
            services.AddHttpContextAccessor();
            services.Configure<ApiBehaviorOptions>(options => { options.SuppressModelStateInvalidFilter = true; });
        }

        // This method gets called by the runtime. Use this method to configure the HTTP request pipeline.
        public void Configure(IApplicationBuilder app, IWebHostEnvironment env, ILoggerFactory loggerFactory, UserManager<ApplicationUser> appUser, RoleManager<ApplicationRole> appRole)
        {
            if (env.IsDevelopment())
            {
                app.UseDeveloperExceptionPage();
            }

            //Allows for the request content to be read multiple times
            app.Use((context, next) =>
            {
                context.Request.EnableBuffering();
                return next();
            });

            //A middleware that logs the api http request and response
            app.UseMiddleware<HttpLoggerMiddleware>();

            //A middleware that handles global exception
            app.UseMiddleware<ExceptionMiddleware>();


            app.UseCors("AllowAllMethod");
            loggerFactory.AddFile("C:/Logs/WemaBankAssignment-{Date}.txt");

            app.UseSwagger();
            app.UseSwaggerUI(c => c.SwaggerEndpoint("/swagger/v1/swagger.json", "WemaBankAssignment v1"));

            app.UseHttpsRedirection();
            app.UseRouting();
            app.UseAuthentication();
            app.UseAuthorization();

            app.UseEndpoints(endpoints =>
            {
                endpoints.MapControllers();
            });

            //runs default database configurations
            app.SeedRoleData(appUser, appRole);
        }

    }
}
