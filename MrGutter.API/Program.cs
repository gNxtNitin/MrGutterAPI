
using MrGutter.Domain;
using MrGutter.Services.IServices;
using MrGutter.Services.Services;
using MrGutter.Utility;
using Microsoft.OpenApi.Models;
using System.Reflection;

using Microsoft.IdentityModel.Tokens;
using System.Text;
using MrGutter.WebAPI;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using AuthLibrary.Interface;
using AuthLibrary;
using PasswordManagementLibrary;
using MenuManagementLib;
using UserManagementLibrary;

var builder = WebApplication.CreateBuilder(args);
// Add services to the container.
builder.Services.AddControllers();
builder.Services.AddScoped<IMiscDataSetting, MiscDataSetting>();
builder.Services.AddScoped<IAccountService, AccountService>();
builder.Services.AddScoped<IUserManager, UserManager>();//Own services
builder.Services.AddScoped<IUserManagerServiceLib, UserManagerServiceLib>(); //In this service we are calling the Libraries
builder.Services.AddScoped<IUMSService, UmSService>();
builder.Services.AddScoped<IJwtAuthService, JwtAuthService>();
builder.Services.AddScoped<IAuthService, AuthService>();//Authentication with OTP and Jwt Lib
builder.Services.AddScoped<IPasswordRepository, PasswordRepository>(); //Password Lib
builder.Services.AddScoped<IMenuManagementService, MenuManagementService>(); //MenuManagementLib
builder.Services.AddScoped<IUserManagerService, UserManagerService>(); //UserManagementLibrary

#region "JWT Token"
builder.Services.AddAuthentication(JwtBearerDefaults.AuthenticationScheme).AddJwtBearer(options =>
{
    options.IncludeErrorDetails = true;
    options.RequireHttpsMetadata = false;
    options.SaveToken = true;
    options.TokenValidationParameters = new TokenValidationParameters()
    {
        ValidateIssuer = true,
        ValidateAudience = true,
        ValidAudience = builder.Configuration["Jwt:Audience"],
        ValidIssuer = builder.Configuration["Jwt:Issuer"],
        IssuerSigningKey = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(builder.Configuration["Jwt:Key"]))
    };
});
#endregion

// Learn more about configuring Swagger/OpenAPI at https://aka.ms/aspnetcore/swashbuckle
builder.Services.AddEndpointsApiExplorer();
//builder.Services.AddSwaggerGen();

builder.Services.AddSwaggerGen(c =>
{
    c.SwaggerDoc("v1", new OpenApiInfo { Title = "Mr Gutter", Version = "V1.0" });
    c.AddSecurityDefinition("Bearer", new OpenApiSecurityScheme
    {
        Description = @"JWT Authorization header using the Bearer scheme. \r\n\r\n 
                      Enter 'Bearer' [space] and then your token in the text input below.
                      \r\n\r\nExample: 'Bearer 12345abcdef'",
        Name = "Authorization",
        In = ParameterLocation.Header,
        Type = SecuritySchemeType.ApiKey,
        Scheme = "Bearer"
    });
    c.AddSecurityRequirement(new OpenApiSecurityRequirement()
      {
        {
          new OpenApiSecurityScheme
          {
            Reference = new OpenApiReference
              {
                Type = ReferenceType.SecurityScheme,
                Id = "Bearer"
              },
              Scheme = "oauth2",
              Name = "Bearer",
              In = ParameterLocation.Header,

            },
            new List<string>()
          }
        });
    var xmlFile = $"{Assembly.GetExecutingAssembly().GetName().Name}.xml";
    var xmlPath = Path.Combine(AppContext.BaseDirectory, xmlFile);
    // c.IncludeXmlComments(xmlPath);
});

var app = builder.Build();

if (app.Environment.IsDevelopment())
{
    app.UseSwagger();
    app.UseSwaggerUI(options =>
    {
        options.DefaultModelsExpandDepth(-1);
    });
}

app.UseHttpsRedirection();

app.UseAuthentication();

app.UseAuthorization();

app.MapControllers();

UMSResources.configuration = app.Configuration;
app.Run();
