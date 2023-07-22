using System.Text;
using IdentityDotnet7.Api.Configuration;
using IdentityDotnet7.Api.Configuration.EmailSender;
using IdentityDotnet7.Api.Configuration.JwtTokenGenerator;
using IdentityDotnet7.Api.Models;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Options;
using Microsoft.IdentityModel.Tokens;

var builder = WebApplication.CreateBuilder(args);

// Add services to the container.

builder.Services.AddControllers();
// Learn more about configuring Swagger/OpenAPI at https://aka.ms/aspnetcore/swashbuckle
builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen();

var connectionString = builder.Configuration.GetConnectionString("DbConnection");

builder.Services.AddDbContextPool<AppDbContext>(options => options.UseSqlServer(connectionString));

builder.Services.AddIdentity<IdentityUser, IdentityRole>(options =>
{
    options.SignIn.RequireConfirmedEmail = true;
    // After 5 request failed the account will be locked
    options.Lockout.MaxFailedAccessAttempts = 5;
    // After 5 minutes the account will be unlocked
    options.Lockout.DefaultLockoutTimeSpan = TimeSpan.FromMinutes(5);
}).AddEntityFrameworkStores<AppDbContext>()
    .AddRoles<IdentityRole>()
    .AddDefaultTokenProviders();

//Jwt Configuration
var jwtSettings = new JwtSettings();
builder.Configuration.Bind(JwtSettings.SectionName, jwtSettings);

builder.Services.AddSingleton(Options.Create(jwtSettings));

builder.Services.AddSingleton<IJwtTokenGenerator, JwtTokenGenerator>();

builder.Services.AddAuthentication(JwtBearerDefaults.AuthenticationScheme).AddJwtBearer(options => 
    options.TokenValidationParameters = new TokenValidationParameters
    {
        ValidateIssuer = true,
        ValidateAudience = true,
        ValidateLifetime = true,
        ValidateIssuerSigningKey = true,
        ValidIssuer = jwtSettings.Issuer,
        ValidAudience = jwtSettings.Audience,
        IssuerSigningKey = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(jwtSettings.Secret))
    });

//Email Configuration
var emailSettings = new EmailSettings();
builder.Configuration.Bind(EmailSettings.SectionName, emailSettings);

builder.Services.AddSingleton(Options.Create(emailSettings));

builder.Services.AddSingleton<IMailService, MailService>();

var app = builder.Build();

// Configure the HTTP request pipeline.
if (app.Environment.IsDevelopment())
{
    app.UseSwagger();
    app.UseSwaggerUI();
}

app.UseHttpsRedirection();

app.UseAuthorization();

app.MapControllers();

app.Run();