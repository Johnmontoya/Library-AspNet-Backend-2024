using Backend.Database;
using Backend.Models;
using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.OpenApi.Models;
using Microsoft.IdentityModel.Tokens;
using System.Text;
using Microsoft.AspNetCore.Server.Kestrel.Core;
using Backend.Interfaces;
using Backend.Core;
using Backend.Dtos;
using System.Reflection;
using System.Text.Json;
using FluentValidation;
using Backend.Rules;
using Backend.Validators;
using Backend.Resources;
using System.Globalization;
using Microsoft.AspNetCore.Localization;
using Microsoft.Extensions.Options;
using Backend.Core.Errors;
using Backend.Core.OTP;
using Microsoft.OData.Edm;
using Microsoft.OData.ModelBuilder;
using Microsoft.AspNetCore.OData;
using Backend.Mapper;

namespace Backend
{
    /// <summary>
    /// Clase de configuraciñon de servicios
    /// </summary>
    public class Startup
    {
        /// <summary>
        /// Acceso al archivo de configuración
        /// </summary>
        public IConfiguration Configuration { get; }

        /// <summary>
        /// Acceso al enviroment
        /// </summary>
        public IWebHostEnvironment CurrentEnvironment { get; }

        /// <summary>
        /// Constructor
        /// </summary>
        /// <param name="configuration"></param>
        /// <param name="env"></param>
        public Startup(IConfiguration configuration, IWebHostEnvironment env)
        {
            Configuration = configuration;
            CurrentEnvironment = env;
        }

        /// <summary>
        /// Permite configurar los servicios de la aplicación
        /// </summary>
        /// <param name="services"></param>
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

            services.AddAutoMapper(typeof(MapperClass));

            services.AddIdentity<Authentication, IdentityRole>(
                opt =>
                {
                    opt.SignIn.RequireConfirmedAccount = true;
                    opt.Lockout.DefaultLockoutTimeSpan = TimeSpan.FromMinutes(2);
                    opt.Lockout.MaxFailedAccessAttempts = 5;
                    opt.Lockout.AllowedForNewUsers = true;
                    opt.User.RequireUniqueEmail = true;
                })
                .AddRoles<IdentityRole>()
                .AddEntityFrameworkStores<AppDbContext>()
                .AddDefaultTokenProviders()
                .AddErrorDescriber<CustomValidationIdentityError>()
                .AddApiEndpoints();

            services.AddControllers()
                .AddJsonOptions(options => {
                    options.JsonSerializerOptions.PropertyNamingPolicy = JsonNamingPolicy.CamelCase;
                })
                .AddOData(
                options => options.AddRouteComponents("odata", GetEdmModel())
                    .Select()
                    .Filter()
                    .OrderBy()
                    .Expand()
                    .Count()
                    .SetMaxTop(null)
                );
            
                /*.AddJsonOptions(options =>
                {
                    options.JsonSerializerOptions.ReferenceHandler = System.Text.Json.Serialization.ReferenceHandler.Preserve;
                });*/

            //OTP
            services.AddTransient<IOTPService, OTPService>();

            //Envio de emails
            services.AddTransient<IMailService, MailService>();
            services.Configure<MailSettingDto>(Configuration.GetSection("MailSettings"));

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

            //Cambio de idioma
            services.AddSingleton<LocService>();
            services.AddLocalization(options => options.ResourcesPath = "Resources");

            services.AddMvc()
                .AddDataAnnotationsLocalization(options => 
                {
                    options.DataAnnotationLocalizerProvider = (type, factory) =>
                    {
                        var assemblyName = new AssemblyName(typeof(SharedResource).GetTypeInfo().Assembly.FullName!);
                        return factory.Create("SharedResource", assemblyName.Name!);
                    };
                });            

            services.Configure<RequestLocalizationOptions>(
                options =>
                {
                    var supportedCultures = new[]
                    {
                        new CultureInfo("es-ES"),
                        new CultureInfo("en-US")
                    };

                    options.DefaultRequestCulture = new RequestCulture(culture: "es-ES", uiCulture: "es-ES");
                    options.SupportedCultures = supportedCultures;
                    options.SupportedUICultures = supportedCultures;

                    options.AddInitialRequestCultureProvider(new CustomRequestCultureProvider(async context =>
                    {
                        return await Task.FromResult(new ProviderCultureResult("es"));
                    }));
                });

            //Validaciones de clase
            services.AddValidatorsFromAssemblyContaining<CategoriaValidator>();
            services.AddValidatorsFromAssemblyContaining<UsuarioValidator>();
            services.AddValidatorsFromAssemblyContaining<AutorValidator>();
            services.AddValidatorsFromAssemblyContaining<LibroValidator>();

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

                var xmlFilename = $"{Assembly.GetExecutingAssembly().GetName().Name}.xml";
                swagger.IncludeXmlComments(Path.Combine(AppContext.BaseDirectory, xmlFilename));

            });
            services.AddCors();
        }

        /// <summary>
        /// Permite configurar la aplicación
        /// </summary>
        /// <param name="app"></param>
        /// <param name="env"></param>
        public void Configure(IApplicationBuilder app, IWebHostEnvironment env)
        {
            var locOptions = app.ApplicationServices.GetService<IOptions<RequestLocalizationOptions>>();
            app.UseRequestLocalization(locOptions!.Value);

            if (env.IsDevelopment())
            {
                app.UseSwagger();
                app.UseSwaggerUI();
            } else
            {
                app.UseExceptionHandler("/Error");
                app.UseHsts();
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

        private static IEdmModel GetEdmModel()
        {
            ODataConventionModelBuilder builder = new ODataConventionModelBuilder();
            builder.EntitySet<Autor>("Autors");
            builder.EntitySet<Categoria>("Categorias");
            builder.EntitySet<Libro>("Libros");
            builder.EntitySet<Prestamo>("Prestamos");
            return builder.GetEdmModel();
        }
    }
}
