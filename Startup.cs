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

            services.AddMvc();
            services.AddEndpointsApiExplorer();
            services.AddSwaggerGen();
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

            app.UseEndpoints(endpoints =>
            {
                endpoints.MapControllers();
            });
        }
    }
}
