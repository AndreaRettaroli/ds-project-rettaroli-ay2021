using System;
using Amazon.CognitoIdentityProvider;
using Amazon.DynamoDBv2;
using Amazon.S3;
using AWSServerlessApplication.Authentication;
using AWSServerlessApplication.AWS;
using AWSServerlessApplication.Services;
using AWSServerlessApplication.Services.Interfaces;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;



namespace AWSServerlessApplication.Helpers
{
    public static class ServiceCollectionExtensions
    {
        public static void ConfigureJsonOptions(this IServiceCollection services)
        {
            services
            .AddControllers()
            .AddJsonOptions(options =>
            {
                options.JsonSerializerOptions.IgnoreNullValues = true;
            });
        }

        public static void ConfigureAuthentication(this IServiceCollection services, IConfiguration configuration)
        {
            var jwtManager = new JwtBearerManager(
                userPoolId: configuration["UserPoolId"],
                audiences: new[] { configuration["UserPoolClientId"] });

            services
            .AddAuthentication(JwtBearerDefaults.AuthenticationScheme)
            .AddJwtBearer(jwtManager.SetJwtBearerOptions);
        }

        public static void ConfigureAWSServices(this IServiceCollection services)
        {
            services.AddAWSService<IAmazonCognitoIdentityProvider>();
            services.AddAWSService<IAmazonDynamoDB>();
            services.AddAWSService<IAmazonS3>();
        }

        public static void ConfigureApplicationServices(this IServiceCollection services)
        {

            services.AddSingleton<IAuthService, AuthService>();
            services.AddScoped<ICognitoService, CognitoService>();
            services.AddScoped<IUsersService, UsersService>();

        }

        //public static void ConfigureHttpClients(this IServiceCollection services, IConfiguration configuration)
        //{
        //    services.AddHttpClient<IProjectsService, ProjectsService>(c =>
        //    {
        //        c.BaseAddress = new Uri(configuration["ProjectServiceBaseAddress"]);
        //    });
        //}
    }
}