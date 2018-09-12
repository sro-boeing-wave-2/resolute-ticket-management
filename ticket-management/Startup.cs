﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.HttpsPolicy;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;
using Microsoft.EntityFrameworkCore;
using ticket_management.Models;
using ticket_management.contract;
using ticket_management.Services;

namespace ticket_management
{
    public class Startup
    {
        public Startup(IConfiguration configuration, IHostingEnvironment env)
        {
            Configuration = configuration;
            _env = env;
        }

        public IConfiguration Configuration { get; }
        public IHostingEnvironment _env { get; }

        // This method gets called by the runtime. Use this method to add services to the container.
        public void ConfigureServices(IServiceCollection services)
        {
            services.AddMvc().SetCompatibilityVersion(CompatibilityVersion.Version_2_1);

            if (_env.EnvironmentName == "Testing")
            {
                services.AddDbContext<TicketContext>(Options =>
                Options.UseInMemoryDatabase("TestDb"));

            }
            
            
                services.AddDbContext<TicketContext>(options =>
                     options.UseSqlServer(Configuration.GetConnectionString("TicketContext")));
            

            services.AddScoped<ITicketService, TicketService>();

            services.AddCors(
                options => options.AddPolicy("allowaccess",
                builder => builder.AllowAnyOrigin().AllowAnyMethod().AllowAnyHeader()
                ));


        }

        // This method gets called by the runtime. Use this method to configure the HTTP request pipeline.
        public void Configure(IApplicationBuilder app, IHostingEnvironment env, TicketContext context)
        {
            if (env.IsDevelopment())
            {
                app.UseDeveloperExceptionPage();
            }
            else
            {
                app.UseHsts();
            }
            context.Database.Migrate();
            app.UseCors("allowaccess");
            app.UseHttpsRedirection();
            app.UseMvc();
        }
    }
}
