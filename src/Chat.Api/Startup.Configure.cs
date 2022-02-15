using Autofac.Extensions.DependencyInjection;
using Chat.Api.Middlewares.Correlation;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Hosting;
using Microsoft.Extensions.Hosting;

namespace Chat.Api
{
    public partial class Startup
    {
        public void Configure(IApplicationBuilder app, IWebHostEnvironment env)
        {
            if (env.IsDevelopment())
            {
                app.UseDeveloperExceptionPage();
            }

            app.UseSwagger()
                .UseSwaggerUI(c => { c.SwaggerEndpoint("../swagger/v1/swagger.json", "Chat API V1"); });


            app.UseMiddleware<CorrelationMiddleware>();

            app.UseRouting();

            app.UseEndpoints(endpoints => endpoints.MapControllers());

            AutofacContainer = app.ApplicationServices.GetAutofacRoot();
        }
    }
}
