using Greggs.Products.Api.Business;
using Greggs.Products.Api.DataAccess;
using Greggs.Products.Api.JWT;
using Greggs.Products.Api.Models;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Diagnostics;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Http;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using Microsoft.IdentityModel.Tokens;
using Microsoft.OpenApi.Models;
using System;
using System.IO;
using System.Net;
using System.Reflection;
using System.Text;

namespace Greggs.Products.Api;
/// <summary>
/// Startup - TODO move some of the bulky middleware to extion classes to slim down 
/// this class 
/// </summary>
public class Startup
{
  private readonly IConfiguration _configuration;
  public Startup(IConfiguration configuration)
  {
    _configuration = configuration;
  }
  public void ConfigureServices(IServiceCollection services)
  {

    services.Configure<ProductExchangeRate>(
           _configuration.GetSection("ProductExchangeRate"));

    var key = _configuration.GetValue<string>("JWT:key");

    services.AddSingleton<IJwtAuthenticationManager>(new JwtAuthenticationManager(key));
    services.AddTransient(typeof(IProductService), typeof(ProductService));
    services.AddTransient(typeof(IDataAccess<Product>), typeof(ProductAccess));

    services.AddControllers();

    services.AddAuthentication(x =>
    {
      x.DefaultAuthenticateScheme = JwtBearerDefaults.AuthenticationScheme;
      x.DefaultChallengeScheme = JwtBearerDefaults.AuthenticationScheme;
    }).AddJwtBearer(x =>
    {
      x.RequireHttpsMetadata = false;
      x.SaveToken = true;
      x.TokenValidationParameters = new TokenValidationParameters
      {
        ValidateIssuerSigningKey = true,
        IssuerSigningKey = new SymmetricSecurityKey(Encoding.ASCII.GetBytes(key)),
        ValidateIssuer = false,
        ValidateAudience = false
      };
    });

    services.AddSwaggerGen(option =>
{
  var xmlFilename = $"{Assembly.GetExecutingAssembly().GetName().Name}.xml";
  option.IncludeXmlComments(Path.Combine(AppContext.BaseDirectory, xmlFilename));
  option.SwaggerDoc("v1", new OpenApiInfo { Title = "Greggs Products API", Version = "v1" });
  option.AddSecurityDefinition("Bearer", new OpenApiSecurityScheme
  {
    In = ParameterLocation.Header,
    Description = "Please enter a valid token",
    Name = "Authorization",
    Type = SecuritySchemeType.Http,
    BearerFormat = "JWT",
    Scheme = "bearer"
  });
  option.AddSecurityRequirement(new OpenApiSecurityRequirement
    {
        {
            new OpenApiSecurityScheme
            {
                Reference = new OpenApiReference
                {
                    Type=ReferenceType.SecurityScheme,
                    Id="Bearer"
                }
            },
            new string[]{}
        }
    });
});

  }

  public void Configure(IApplicationBuilder app, IWebHostEnvironment env)
  {
    if (env.IsDevelopment())
    {
      app.UseDeveloperExceptionPage();
    }

    app.UseSwagger();
    app.UseSwaggerUI(c => { c.SwaggerEndpoint("/swagger/v1/swagger.json", "Greggs Products API V1"); });

    app.UseHttpsRedirection();

    app.UseExceptionHandler(options =>
    {
      options.Run(
        async context =>
        {
          context.Response.StatusCode = (int)HttpStatusCode.InternalServerError;
          var ex = context.Features.Get<IExceptionHandlerFeature>();
          if (ex != null)
          {
            await context.Response.WriteAsync(ex.Error.Message);
          }
        });
    });

    app.UseRouting();

    app.UseAuthentication();
    app.UseAuthorization();

    app.UseEndpoints(endpoints => { endpoints.MapControllers(); });
  }
}