using System;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Components;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.ResponseCompression;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using Newtonsoft.Json.Serialization;
using System.Linq;
using System.Net.Http;

namespace prerendering.Server
{
    public class Startup
    {
        // This method gets called by the runtime. Use this method to add services to the container.
        // For more information on how to configure your application, visit https://go.microsoft.com/fwlink/?LinkID=398940
        public void ConfigureServices(IServiceCollection services)
        {
            services.AddMvc().AddNewtonsoftJson();
            services.AddResponseCompression(opts =>
            {
                opts.MimeTypes = ResponseCompressionDefaults.MimeTypes.Concat(
                    new[] { "application/octet-stream" });
            });
            services.AddScoped<HttpClient>(s =>
            {
                var uriHelper = s.GetRequiredService<IUriHelper>();
                return new HttpClient
                {
                    BaseAddress = new Uri(uriHelper.GetBaseUri())
                };
            });
        }

        // This method gets called by the runtime. Use this method to configure the HTTP request pipeline.
        public void Configure(IApplicationBuilder app, IWebHostEnvironment env)
        {
            app.UseResponseCompression();

            if (env.IsDevelopment())
            {
                app.UseDeveloperExceptionPage();
                app.UseBlazorDebugging();
            }

            app.UseClientSideBlazorFiles<Client.Startup>();

            app.UseRouting();

            app.UseEndpoints(endpoints =>
            {
                #region Pre Rendering
                endpoints.MapDefaultControllerRoute();
                endpoints.MapFallbackToPage("/_Host");
                #endregion

                #region Client side  Rendering
                //endpoints.MapDefaultControllerRoute();
                //endpoints.MapFallbackToClientSideBlazor<Client.Startup>("index.html"); 
                #endregion
            });
        }
    }
}
