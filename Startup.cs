using Backend.Database;
using Backend.Models;
using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.OpenApi.Models;
using Microsoft.IdentityModel.Tokens;
using System.Text;
using Microsoft.AspNetCore.Server.Kestrel.Core;
using Microsoft.Extensions.Options;

namespace Backend
{
    public class Startup
    {
        public IConfiguration Configuration { get; }

        public IWebHostEnvironment CurrentEnvironment { get; }

        public Startup(IConfiguration configuration, IWebHostEnvironment env)
        {
            Configuration = configuration;
            CurrentEnvironment = env;
        }

        public void ConfigureServices(IServiceCollection services)
        {
            var serverVersion = new MySqlServerVersion(new Version(8, 0, 29));
            var JWTSetting = Configuration.GetSection("JWTSetting");

            services.Configure<KestrelServerOptions>(options =>
            {
                options.AllowSynchronousIO = true;
            });

            services.AddDbContext<AppDbContext>(options =>
                options.UseMySql(Configuration.GetConnectionString("DefaultConnection"), serverVersion));
            
            services.AddIdentity<Authentication, IdentityRole>(
                opt =>
                {
                    //opt.SignIn.RequireConfirmedAccount = true;
                    opt.Lockout.DefaultLockoutTimeSpan = TimeSpan.FromMinutes(2);
                    opt.Lockout.MaxFailedAccessAttempts = 5;
                    opt.Lockout.AllowedForNewUsers = true;
                    opt.User.RequireUniqueEmail = true;
                })
                .AddEntityFrameworkStores<AppDbContext>()
                .AddDefaultTokenProviders();

            services.AddAuthentication(options =>
            {
                options.DefaultAuthenticateScheme = JwtBearerDefaults.AuthenticationScheme;
                options.DefaultChallengeScheme = JwtBearerDefaults.AuthenticationScheme;
            }).AddJwtBearer(opt =>
            {
                opt.SaveToken = true;
                opt.RequireHttpsMetadata = false;
                opt.TokenValidationParameters = new TokenValidationParameters
                {
                    ValidateIssuer = true,
                    ValidateAudience = true,
                    ValidateLifetime = true,
                    ClockSkew = TimeSpan.Zero,
                    ValidateIssuerSigningKey = true,
                    ValidAudience = JWTSetting["ValidAudience"],
                    ValidIssuer = JWTSetting["ValidIssuer"],
                    IssuerSigningKey = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(JWTSetting.GetSection("securityKey").Value!))
                };
            });

            services.AddAuthorization();
            services.AddSingleton<System.TimeProvider>(System.TimeProvider.System);
            services.AddMvc();
            services.AddEndpointsApiExplorer();
            services.AddSwaggerGen(swagger =>
            {
                swagger.AddSecurityDefinition("Bearer", new OpenApiSecurityScheme
                {
                    Description = @"JWT Authorization: 'Bearer my-security'",
                    Name = "Authorization",
                    In = ParameterLocation.Header,
                    Type = SecuritySchemeType.ApiKey,
                    Scheme = "Bearer"
                });

                swagger.AddSecurityRequirement(new OpenApiSecurityRequirement()
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
                            In = ParameterLocation.Header
                        },
                        new List<string>()
                    }
                });

                swagger.SwaggerDoc("v1", new OpenApiInfo
                {
                    Version = "v1",
                    Title = "Backend Library",
                    Description = "App de una Biblioteca basica con authenticacion, almacenamiento en Mysql y documentación en swagger",
                    Contact = new OpenApiContact
                    {
                        Name = "Github",
                        Url = new Uri("https://github.com/Johnmontoya")
                    }
                });
            });
            services.AddCors();
        }

        public void Configure(IApplicationBuilder app, IWebHostEnvironment env)
        {
            if(env.IsDevelopment())
            {
                app.UseSwagger();
                app.UseSwaggerUI();
            }

            var urlAceptadas = Configuration.GetSection("AllowedHosts").Value!.Split(",");
            app.UseCors(builder =>
                builder.WithOrigins(urlAceptadas)
                    .AllowAnyHeader()
                    .AllowAnyMethod());

            app.UseRouting();
            app.UseAuthentication();
            app.UseAuthorization();

            app.UseEndpoints(endpoints =>
            {
                endpoints.MapControllers();
                /*endpoints.MapIdentityApi<Authentication>();
                endpoints.MapGet("/", () => Results.Redirect("/swagger"));
                endpoints.MapGet("/profile", (HttpContext httpContext) =>
                {
                    return new
                    {
                        httpContext.User.Identity?.Name,
                        httpContext.User.Identity?.AuthenticationType,
                        Claims = httpContext.User.Claims.Select(s => new
                        {
                            s.Type,
                            s.Value
                        }).ToList()
                    };
                }).RequireAuthorization();*/
            });
        }
    }
}
