using Autofac;
using Autofac.Extensions.DependencyInjection;
using BankSystem.Data;
using BankSystem.Data.Repositories;
using BankSystem.Middleware;
using BankSystem.Repository;
using BankSystem.Service;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.IdentityModel.Tokens;
using Microsoft.OpenApi.Models;
using Swashbuckle.AspNetCore.Filters;
using System.Security.Claims;
using System.Text;

var builder = WebApplication.CreateBuilder(args);

var containerBuilder = new ContainerBuilder();

builder.Host.UseServiceProviderFactory(new AutofacServiceProviderFactory());
builder.Host.ConfigureContainer<ContainerBuilder>(containerBuilder =>
{
#pragma warning disable CS8604 // Possible null reference argument.
    containerBuilder.RegisterType<DataAccess>().As<IDataAccess>().WithParameter("connectionString", builder.Configuration.GetConnectionString("BankSystemConn"));
#pragma warning restore CS8604 // Possible null reference argument.

    containerBuilder.RegisterType<PdfService>().As<IPdfService>();

    containerBuilder.RegisterType<PasswordHashingService>().AsSelf();

    containerBuilder.RegisterType<UserRepository>().AsSelf();
    containerBuilder.RegisterType<UserService>().AsSelf();

    containerBuilder.RegisterType<AuthRepository>().AsSelf();
    containerBuilder.RegisterType<AuthService>().AsSelf();

    containerBuilder.RegisterType<TransactionRepository>().AsSelf();
    containerBuilder.RegisterType<TransactionService>().As<ITransactionService>();

    containerBuilder.RegisterType<CustomerRepository>().AsSelf();
    containerBuilder.RegisterType<CustomerService>().As<ICustomerService>();

    containerBuilder.RegisterType<BalanceRepository>().AsSelf();
    containerBuilder.RegisterType<BalanceService>().As<IBalanceService>();

    containerBuilder.RegisterType<SignatureService>().As<ISignatureService>();
});

// Add services to the container.
builder.Services.AddCors();
builder.Services.AddControllers();

// Learn more about configuring Swagger/OpenAPI at https://aka.ms/aspnetcore/swashbuckle
builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen();

builder.Services.AddAuthentication(options =>
{
    options.DefaultAuthenticateScheme = JwtBearerDefaults.AuthenticationScheme;
    options.DefaultChallengeScheme = JwtBearerDefaults.AuthenticationScheme;
    options.DefaultScheme = JwtBearerDefaults.AuthenticationScheme;
}).AddJwtBearer(options =>
{
    options.SaveToken = true;
    options.RequireHttpsMetadata = false;
#pragma warning disable CS8604 // Possible null reference argument.
    options.TokenValidationParameters = new TokenValidationParameters()
    {
        ValidateIssuer = true,
        ValidateAudience = true,
        ValidateIssuerSigningKey = true,
        ValidateLifetime = true,
        ValidAudience = builder.Configuration["Jwt:Audience"],
        ValidIssuer = builder.Configuration["Jwt:Issuer"],
        ClockSkew = TimeSpan.Zero,// It forces tokens to expire exactly at token expiration time instead of 5 minutes later
        IssuerSigningKey = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(builder.Configuration["Jwt:Key"])),
        RoleClaimType = ClaimTypes.Role
    };
#pragma warning restore CS8604 // Possible null reference argument.
});

builder.Services.AddSwaggerGen(options =>
{
    options.AddSecurityDefinition("oauth2", new OpenApiSecurityScheme
    {
        In = ParameterLocation.Header,
        Name = "Authorization",
        Type = SecuritySchemeType.ApiKey,
    });

    options.OperationFilter<SecurityRequirementsOperationFilter>();
});


var app = builder.Build();
app.UseMiddleware<JwtMiddleware>();

// Configure the HTTP request pipeline.
if (app.Environment.IsDevelopment())
{
    app.UseSwagger();
    app.UseSwaggerUI();
}

app.UseHttpsRedirection();

app.UseAuthentication();
app.UseAuthorization();


// global cors policy
app.UseCors(x => x
    .AllowAnyOrigin()
    .AllowAnyMethod()
    .AllowAnyHeader());

app.MapControllers();

app.Run();
