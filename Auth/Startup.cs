using Microsoft.AspNetCore.Authentication.Cookies;
using Microsoft.AspNetCore.Authentication.Google;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Mvc.Authorization;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using System.Security.Claims;

namespace Auth
{
    public class Startup
    {
        private readonly IConfiguration _configuration;

        public Startup(IConfiguration configuration)
        {
            _configuration = configuration;
        }

        public void ConfigureServices(IServiceCollection services)
        {
            services.AddControllersWithViews(o => o.Filters.Add(new AuthorizeFilter()));
            services.AddAuthentication(o =>
            {
                o.DefaultScheme = CookieAuthenticationDefaults.AuthenticationScheme;
                o.DefaultChallengeScheme = GoogleDefaults.AuthenticationScheme;
            })
                .AddCookie("Cookies", config =>
                 {

                     config.LoginPath = "/Auth/Login";
                     config.Cookie.Name = "Granma.Cookies";
                     //config.Events = new CookieAuthenticationEvents { OnValidatePrincipal = async (c) => { c.Principal.HasClaim(f=>f.Type==ClaimTypes.Name); } };
                 })
                .AddGoogle(o =>
                {
                    o.ClientId = _configuration["Google:ClientId"];
                    o.ClientSecret = _configuration["Google:ClientSecret"];
                });

        }


        public void Configure(IApplicationBuilder app, IWebHostEnvironment env)
        {
            if (env.IsDevelopment())
            {
                app.UseDeveloperExceptionPage();
            }

            app.UseRouting();
            app.UseAuthentication();
            app.UseAuthorization();
            app.UseEndpoints(endpoints =>
            {
                endpoints.MapControllerRoute("authRoute",
                    "{controller}/{action}/{id?}", new { controller = "Home", action = "Index" });
                //endpoints.MapGet("/", async context =>
                //{
                //    await context.Response.WriteAsync("Hello World!");
                //});
            });
        }
    }
}
