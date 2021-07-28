using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Http;

namespace AWSServerlessApplication.Helpers
{
    public static class ApplicationBuilderExtensions
    {
        public static void ConfigureCors(this IApplicationBuilder app)
        {
           
            app
            .UseCors(builder => builder
            .SetIsOriginAllowed(origin => true)   
            .AllowAnyMethod()
            .AllowAnyHeader()
            .AllowCredentials());
        }

        public static void ConfigureEndpoints(this IApplicationBuilder app)
        {
            app.UseEndpoints(endpoints =>
            {
                endpoints.MapControllers();
                endpoints.MapGet("/", async context =>
                {
                    await context.Response.WriteAsync("Welcome to running ASP.NET Core on AWS Lambda");
                });
            });
        }
    }
}