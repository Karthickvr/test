using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Diagnostics;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Http;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using System.Net;
using XOProject.Repository;
using XOProject.Services;

namespace XOProject.Api
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
            services.AddSingleton<IDataSeed>(new DataSeed());
            RepositoryModule.Register(services,
                Configuration.GetConnectionString("DefaultConnection"),
                GetType().Assembly.FullName);
            ServicesModule.Register(services);
            services.AddMvc();
        }

        // This method gets called by the runtime. Use this method to configure the HTTP request pipeline.
        public void Configure(IApplicationBuilder app, IHostingEnvironment env)
        {
            //TO DO: make this exception handling environment specific - this is just base implementation to wrap the response
            app.UseExceptionHandler(err =>
            {
                err.Run(async context =>
                {
                    context.Response.StatusCode = (context.Response.StatusCode != (int)HttpStatusCode.BadRequest && context.Response.StatusCode != (int)HttpStatusCode.NotFound) ? (int)HttpStatusCode.BadRequest : context.Response.StatusCode;
                    context.Response.ContentType = "application/json";

                    var contextFeature = context.Features.Get<IExceptionHandlerFeature>();
                    if (contextFeature != null)
                    {
                        
                        // TO DO: Timebeing using the Application Exception to catch the cutom exception - ehnace it right
                        if (contextFeature.Error != null && contextFeature.Error.GetType() == typeof(System.ApplicationException))
                        {
                            await context.Response.WriteAsync(new
                            {
                                StatusCode = context.Response.StatusCode,
                                Message = contextFeature.Error.Message
                            }.ToString());
                        }
                        else
                        {
                            await context.Response.WriteAsync(new
                            {
                                StatusCode = context.Response.StatusCode,
                                Message = "Bad Request."
                            }.ToString());
                        }
                    }
                });
            });
            //if (env.IsDevelopment())
            //{
            //    //app.UseDeveloperExceptionPage();
            //}
            //else
            //{
            //    app.UseExceptionHandler();
            //}

            app.UseStaticFiles();

            app.UseMvc();
        }
    }
}
