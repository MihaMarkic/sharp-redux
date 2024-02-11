﻿using LiteDB;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Identity;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Sharp.Redux.HubServer.Authentication;
using Sharp.Redux.HubServer.Models;
using Sharp.Redux.HubServer.Services;

namespace Sharp.Redux.HubServer
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
            
            services.AddScoped<IUserStore<ApplicationUser>, UserStore>();
            services.AddScoped<IRoleStore<IdentityRole>, RoleStore>();
            services.AddScoped<IProjectStore, ProjectStore>();
            services.AddScoped<ISessionStore, SessionStore>();
            services.AddScoped<IStepStore, StepStore>();
            services.AddScoped<ITokenStore, TokenStore>();
            services.AddSingleton(new LiteDatabase("datax.db"));
            services.AddIdentity<ApplicationUser, IdentityRole>()
                .AddDefaultTokenProviders();

            // Add application services.
            services.AddTransient<IEmailSender, EmailSender>();
            services.AddAuthentication().AddScheme<ReduxTokenAuthenticationOptions, ReduxTokenAuthentication>(
                ReduxTokenAuthenticationOptions.AuthenticationScheme, 
                options => 
                {
                    var builder = services.BuildServiceProvider();
                    options.TokenStore = builder.GetRequiredService<ITokenStore>();
                });

            services.AddMvc();
        }

        // This method gets called by the runtime. Use this method to configure the HTTP request pipeline.
        public void Configure(IApplicationBuilder app, IHostingEnvironment env)
        {
            if (env.IsDevelopment())
            {
                //app.UseBrowserLink();
                app.UseDeveloperExceptionPage();
                //app.UseDatabaseErrorPage();
            }
            else
            {
                app.UseExceptionHandler("/Home/Error");
            }

            app.UseStaticFiles();

            app.UseAuthentication();

            app.UseMvc(routes =>
            {
                routes.MapRoute(
                    name: "default",
                    template: "{controller=Home}/{action=Index}/{id?}");
            });
        }
    }
}
