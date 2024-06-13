
using AppServiceInterfice.Extensions;
using DomainModel;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;
using Microsoft.IdentityModel.Tokens;
using WebServer.AppDbContexts;
using WebSever.Service;

namespace WebServer
{
    public class Program
    {
        public static void Main(string[] args)
        {
            var builder = WebApplication.CreateBuilder(args);

            // Add services to the container.

            builder.Services.AddControllers();
            builder.Services.AddDbContext<MainDbContext>(opt => opt.UseSqlServer(@"Server=WIN-Q0NS67721NA\SQLEXPRESS;Database=ApplicationAuthorizationDataBase;Trusted_Connection=True;TrustServerCertificate=True;"));
            builder.Services.AddCors(opt =>
            {
                opt.AddPolicy(builder.Configuration.GetSection("Polices")["First"] ?? "Default", conf =>
                {
                    conf.WithOrigins(builder.Configuration["ClientHost"]!)
                    .AllowAnyMethod()
                    .AllowCredentials();

                });
            });
            builder.Services.AddTransient<JwtManager>();
            builder.Services.AddAuthentication(opt =>
            {
                opt.DefaultAuthenticateScheme = JwtBearerDefaults.AuthenticationScheme;
                opt.DefaultChallengeScheme = JwtBearerDefaults.AuthenticationScheme;
            }).AddJwtBearer(options =>
            {
                options.TokenValidationParameters = new TokenValidationParameters()
                {
                    RequireExpirationTime = true,
                    ValidateIssuer = true,
                    ValidateAudience = true,
                    ValidateLifetime = true,
                    ValidIssuer = builder.Configuration["JwtSettings:Issuer"],
                    ValidAudience = builder.Configuration["JwtSettings:Audience"],
                    IssuerSigningKey = AuthExtensions.GetSymmetricSecurityKey(builder.Configuration["JwtSettings:SecurityKey"]!),
                    ValidateIssuerSigningKey = true,
                };
            }).AddBearerToken(IdentityConstants.BearerScheme);
            // вспомним анонимные методы :)
            builder.Services.AddIdentity<User, IdentityRole>(delegate (IdentityOptions opt)
            {
                opt.SignIn.RequireConfirmedAccount = true;
                opt.User.AllowedUserNameCharacters = null;
            }).AddEntityFrameworkStores<MainDbContext>();
            builder.Services.AddEndpointsApiExplorer();
            builder.Services.AddSwaggerGen();
           
            var app = builder.Build();

            // Configure the HTTP request pipeline.
            if (app.Environment.IsDevelopment())
            {
                app.UseSwagger();
                app.UseSwaggerUI();
            }
            app.UseCors(builder.Configuration.GetSection("Polices")["First"] ?? "Default");
            app.UseHttpsRedirection();

            app.UseAuthorization();


            app.MapControllers();

            app.Run();
        }
    }
}
