using Microsoft.AspNetCore.Authentication.Cookies;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Mvc.Authorization;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using System.Security.Claims;

namespace Auth
{
    public class Startup
    {

        public void ConfigureServices(IServiceCollection services)
        {
            services.AddAuthentication(CookieAuthenticationDefaults.AuthenticationScheme)
                .AddCookie("Cookies", config =>
                 {
                     config.LoginPath = "/Auth/Login";
                     config.Cookie.Name = "Granma.Cookies";
                     //config.Events = new CookieAuthenticationEvents { OnValidatePrincipal = async (c) => { c.Principal.HasClaim(f=>f.Type==ClaimTypes.Name); } };
                 });
            services.AddControllersWithViews(o => o.Filters.Add(new AuthorizeFilter()));
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
