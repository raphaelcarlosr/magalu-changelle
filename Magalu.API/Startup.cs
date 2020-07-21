using Magalu.API.Data;
using Magalu.API.Services;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.ApiExplorer;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Options;
using Microsoft.IdentityModel.Tokens;
using Microsoft.OpenApi.Models;
using Swashbuckle.AspNetCore.SwaggerGen;
using System;
using System.Linq;
using System.Text;

namespace Magalu.API
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
            services.Configure<ApiBehaviorOptions>(options => {
                options.InvalidModelStateResponseFactory = context =>
                {
                    var problemDetails = new ValidationProblemDetails(context.ModelState);
                    var result = new BadRequestObjectResult(problemDetails);
                    result.ContentTypes.Add("application/problem+json");
                    result.ContentTypes.Add("application/problem+xml");
                    return result;
                };
            });
            services.Configure<MvcOptions>(c => c.Conventions.Add(new SwaggerApplicationConvention()));
            services.AddCors();

            services.AddControllers(options => options.EnableEndpointRouting = false)
                .ConfigureApiBehaviorOptions(options =>
                {
                    options.SuppressConsumesConstraintForFormFileParameters = true;
                    options.SuppressInferBindingSourcesForParameters = true;
                    options.SuppressModelStateInvalidFilter = true;
                    options.SuppressMapClientErrors = true;
                    options.ClientErrorMapping[StatusCodes.Status404NotFound].Link =
                        "https://httpstatuses.com/404";
                });


            services.AddTransient<IConfiguration>(provider => Configuration);
            services.AddScoped<ApplicationDbContext>();
            services.AddScoped<ClientService>();
            services.AddScoped<ProductService>();
            services.AddScoped<JwtService>();

            services.AddAuthentication(x =>
            {
                x.DefaultAuthenticateScheme = JwtBearerDefaults.AuthenticationScheme;
                x.DefaultChallengeScheme = JwtBearerDefaults.AuthenticationScheme;
            })
            .AddJwtBearer(x =>
            {
                x.RequireHttpsMetadata = false;
                x.SaveToken = true;
                x.TokenValidationParameters = new TokenValidationParameters
                {
                    ValidateIssuerSigningKey = true,
                    IssuerSigningKey = new SymmetricSecurityKey(Encoding.ASCII.GetBytes(JwtService.Secret)),
                    ValidateIssuer = false,
                    ValidateAudience = false
                };
            });


            services.AddApiVersioning(p =>
            {
                p.DefaultApiVersion = new ApiVersion(1, 0);
                p.ReportApiVersions = true;
                p.AssumeDefaultVersionWhenUnspecified = true;
            });

            services.AddVersionedApiExplorer(p =>
            {
                p.GroupNameFormat = "'v'VVV";
                p.SubstituteApiVersionInUrl = true;
            });

            //services.AddTransient<IConfigureOptions<SwaggerGenOptions>, ConfigureSwaggerOptions>();
            services.AddSwaggerGen(setup =>
            {
                setup.SwaggerDoc("v1", new OpenApiInfo
                {
                    Title = "Magalu Changelle",
                    Version = "v1",
                    Description = "API para o desafio Magalu",
                    Contact = new OpenApiContact
                    {
                        Name = "Raphael Carlos Rego",
                        Url = new Uri("https://raphaelcarlosr.com")
                    }
                });
                setup.ResolveConflictingActions(apiDescriptions => apiDescriptions.FirstOrDefault());
            });

            //Strict-Transport-Security
            services.AddHsts(options =>
            {
                options.IncludeSubDomains = true;
                options.MaxAge = TimeSpan.FromDays(365);
            });

            services.AddAntiforgery(o => o.SuppressXFrameOptionsHeader = true);
        }

        // This method gets called by the runtime. Use this method to configure the HTTP request pipeline.
        public void Configure(IApplicationBuilder app, IWebHostEnvironment env, IApiVersionDescriptionProvider versionProvider)
        {
            if (env.IsDevelopment())
            {
                app.UseDeveloperExceptionPage();     
            }
            else
            {
                //Strict-Transport-Security
                app.UseHsts();
            }

            //app.UseHttpsRedirection();

            app.UseApiVersioning();
            app.UseSwagger(options =>
            {
                //options.RouteTemplate = "";
            });
            app.UseSwaggerUI(options =>
            {
                string swaggerJsonBasePath = string.IsNullOrWhiteSpace(options.RoutePrefix) ? "." : "..";

                options.DocumentTitle = "Magalu Changelle";
                foreach (var description in versionProvider.ApiVersionDescriptions)
                {
                    options.SwaggerEndpoint(
                        $"/swagger/{description.GroupName.ToLower()}/swagger.json",
                        description.GroupName.ToUpperInvariant()
                    );
                }
                //options.RoutePrefix = "docs/swagger";
                //options.DocExpansion(Swashbuckle.AspNetCore.SwaggerUI.DocExpansion.List);
            });

            app.UseRouting();

            //app.UseCors(x => x
            //   .AllowAnyOrigin()
            //   .AllowAnyMethod()
            //   .AllowAnyHeader());

            app.UseAuthentication();
            app.UseAuthorization();

            app.UseEndpoints(endpoints =>
            {
                endpoints.MapControllers();
            });

            app.Use(async (context, next) =>
            {
                //deny X-Frame-Options
                context.Response.Headers.Add("Header-Name", "Header-Value");
                //X-Xss-Protection
                context.Response.Headers.Add("X-Xss-Protection", "1; mode=block");
                //X-Content-Type-Options
                context.Response.Headers.Add("X-Content-Type-Options", "nosniff");
                //Referrer-Policy
                context.Response.Headers.Add("Referrer-Policy", "no-referrer");
                //X-Permitted-Cross-Domain-Policies
                context.Response.Headers.Add("X-Permitted-Cross-Domain-Policies", "none");
                //Feature-Policy
                context.Response.Headers.Add("Feature-Policy", "accelerometer 'none'; camera 'none'; geolocation 'none'; gyroscope 'none'; magnetometer 'none'; microphone 'none'; payment 'none'; usb 'none'");
                //Content-Security-Policy
                context.Response.Headers.Add("Content-Security-Policy", "default-src 'self'");
                await next();
            });
            //.UseMvcWithDefaultRoute();          

        }
    }
}
