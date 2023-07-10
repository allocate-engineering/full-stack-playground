using Allocate.Common.Database.Repositories;
using Allocate.Common.Database.Services;
using Allocate.Common.Database.Settings;

using Dapper.Contrib.Extensions;

using Microsoft.AspNetCore.Rewrite;
using Microsoft.OpenApi.Models;

namespace Allocate.Web.DemoApi;

public class Startup
{
    private readonly IWebHostEnvironment _env;

    public Startup(IConfiguration configuration, IWebHostEnvironment env)
    {
        Configuration = configuration;

        _env = env;

        // By default, Dapper.Contrib will create an insert script for the User table 
        // that look like `insert into users...` This changes that behavior to `insert into "User"`.
        // However, we need a custom name for activity because it has a generic and when that gets converted
        // to a string it gets confused and adds a apostrophe which is not how the DB table is named.
        SqlMapperExtensions.TableNameMapper = (type) =>
        {
            var attr =
                type.GetCustomAttributes(typeof(TableAttribute), true);
            if (attr != null && attr.Length > 0)
            {
                return $"\"{((TableAttribute)attr[0]).Name}\"";
            }
            return $"\"{type.Name}\"";
        };
    }

    public IConfiguration Configuration { get; }

    // This method gets called by the runtime. Use this method to add services to the container.
    public void ConfigureServices(IServiceCollection services)
    {
        services.AddHttpClient();

        // This automatically finds and includes everything that extends AutoMapper.Profile
        services.AddAutoMapper(typeof(Startup));

        services.AddHttpContextAccessor();

        services.AddResponseCompression(options =>
        {
            options.EnableForHttps = true;
        });

        services.AddControllers()
            .AddApplicationPart(typeof(Startup).Assembly)
            .AddControllersAsServices()
            .AddJsonOptions(options =>
            {
                options.JsonSerializerOptions.AllowTrailingCommas = true;
            });

        services.AddEndpointsApiExplorer();
        services.AddSwaggerGen(c =>
        {
            c.SwaggerDoc("v1", new OpenApiInfo { Title = "Allocate - .NET Core API Template", Version = "v1" });

            var xmlFile = $"{typeof(Allocate.Web.DemoApi.Startup).Assembly.GetName().Name}.xml";
            var xmlPath = Path.Combine(AppContext.BaseDirectory, xmlFile);
            c.IncludeXmlComments(xmlPath);

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
                        In = ParameterLocation.Header
                    },
                    new List<string>()
                }
            });
        });

        services.AddScoped<IDatabaseService, DatabaseService>();
        services.AddScoped<IGenericRepository, GenericRepository>();
        services.AddSingleton<DatabaseSettings>(GetSettings<DatabaseSettings>(Configuration));
    }

    public static T GetSettings<T>(IConfiguration configuration, string customSectionName = null) where T : class
    {
        // start with a default instance
        var defaultConfig = Activator.CreateInstance<T>();
        // by convention, look for settings using the name of the settings object (minus the word "Settings)
        var sectionName = defaultConfig.GetType().Name.Replace("Settings", "");
        // require a config section to exist for the configuration (this will throw if not present,
        // meaning that you must opt-in for "Default" configuration)
        var configSection = configuration.GetRequiredSection(customSectionName ?? sectionName);
        // otherwise, parse the config from the section
        var config = configSection.Get<T>();
        if (config is null)
        {
            if (configSection.Value == "Default")
            {
                // allow explicit default config return if no custom config is required
                config = defaultConfig;
            }
            else
            {
                throw new Exception("Config for " + sectionName + " not loaded, can't start");
            }
        }
        return config;
    }

    // This method gets called by the runtime. Use this method to configure the HTTP request pipeline.
    public void Configure(IApplicationBuilder app, IWebHostEnvironment env)
    {
        app.UseForwardedHeaders();
        app.UseResponseCompression();

        // app.UseDeveloperExceptionPage();

        app.UseRouting();

        app.UseEndpoints(endpoints =>
        {
            endpoints.MapControllers();
        });

        app.UseSwagger();
        app.UseSwaggerUI(c =>
        {
            c.SwaggerEndpoint("v1/swagger.json", "Allocate - User API v1");
        });
    }
}
