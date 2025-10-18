using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.EntityFrameworkCore;
using Microsoft.IdentityModel.Tokens;
using SignalrMvc.Data;
using SignalrMvc.Hubs;
using SignalrMvc.Services;
using SignalrMvc.UnitOfWork;
using System.Text;

namespace SignalrMvc
{
    public class Program
    {
        public static void Main(string[] args)
        {
            var builder = WebApplication.CreateBuilder(args);

            // Add services to the container.
            builder.Services.AddControllersWithViews();
            
            // Add DbContext
            builder.Services.AddDbContext<ApplicationDbContext>(options =>
                options.UseSqlServer(builder.Configuration.GetConnectionString("DefaultConnection")));

            // Add UnitOfWork and Services
            builder.Services.AddScoped<IUnitOfWork, UnitOfWork.UnitOfWork>();
            builder.Services.AddScoped<IAuthService, AuthService>();
            builder.Services.AddScoped<IChatService, ChatService>();

            // Add JWT Authentication
            var jwtKey = builder.Configuration["Jwt:Key"] ?? throw new InvalidOperationException("JWT Key not configured");
            builder.Services.AddAuthentication(options =>
            {
                options.DefaultAuthenticateScheme = JwtBearerDefaults.AuthenticationScheme;
                options.DefaultChallengeScheme = JwtBearerDefaults.AuthenticationScheme;
            })
            .AddJwtBearer(options =>
            {
                options.TokenValidationParameters = new TokenValidationParameters
                {
                    ValidateIssuer = true,
                    ValidateAudience = true,
                    ValidateLifetime = true,
                    ValidateIssuerSigningKey = true,
                    ValidIssuer = builder.Configuration["Jwt:Issuer"],
                    ValidAudience = builder.Configuration["Jwt:Audience"],
                    IssuerSigningKey = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(jwtKey)),
                    ClockSkew = TimeSpan.Zero
                };

                // Add SignalR support for JWT
                options.Events = new JwtBearerEvents
                {
                    OnMessageReceived = context =>
                    {
                        var accessToken = context.Request.Query["access_token"];
                        var path = context.HttpContext.Request.Path;
                        
                        if (!string.IsNullOrEmpty(accessToken) && path.StartsWithSegments("/chat"))
                        {
                            context.Token = accessToken;
                        }
                        
                        return Task.CompletedTask;
                    }
                };
            });

            builder.Services.AddAuthorization();

            // Add SignalR
            builder.Services.AddSignalR();

            // Add CORS
            builder.Services.AddCors(options =>
            {
                options.AddDefaultPolicy(policy =>
                {
                    policy.WithOrigins("*")
                          .AllowAnyHeader()
                          .AllowAnyMethod();
                });
            });

            var app = builder.Build();

            // Configure the HTTP request pipeline.
            if (!app.Environment.IsDevelopment())
            {
                app.UseExceptionHandler("/Home/Error");
                app.UseHsts();
            }

            app.UseHttpsRedirection();
            app.UseStaticFiles();

            app.UseRouting();

            app.UseCors();
            app.UseAuthentication();
            app.UseAuthorization();

            app.MapHub<ChatHub>("/chat");
            
            app.MapControllerRoute(
                name: "default",
                pattern: "{controller=Auth}/{action=Login}/{id?}");

            app.Run();
        }
    }
}

