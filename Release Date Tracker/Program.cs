using Microsoft.OpenApi.Models;
using Release_Date_Tracker.Accessors;
using Release_Date_Tracker.Managers;
using Release_Date_Tracker.Models.Configuration_Settings;

var builder = WebApplication.CreateBuilder(args);
ConfigureServices(builder.Services, builder.Configuration);

var app = builder.Build();
Configure(app, builder.Services);

app.Run();


void ConfigureServices(IServiceCollection services, ConfigurationManager configuration)
{
    services.AddSingleton<IIgdbAccessor, IgdbAccessor>();
    services.AddSingleton<IGameTitleManager, GameTitleManager>();
    services.AddControllers();
    services.AddEndpointsApiExplorer();

    // IGDB API configuration
    var igdbSection = configuration.GetSection("Igdb");
    var igdbConfiguration = igdbSection.Get<IgdbConfiguration>();
    services.AddSingleton(igdbConfiguration);

    // Swagger set up
    services.AddMvc();

    services.AddSwaggerGen(c =>
    {
        c.SwaggerDoc("v1", new OpenApiInfo
        {
            Version = "v1",
            Title = "Release Date Tracker API",
            Description = "Release Date Tracker API",
            Contact = new OpenApiContact
            {
                Name = "Michael QUintana",
                Email = "mquintana78750@gmail.com",
            }
        });
    });
}

void Configure(WebApplication app, IServiceCollection services)
{
    // Swagger Configuration
    app.MapControllers();
    app.UseDeveloperExceptionPage();

    app.UseSwagger(c =>
    {
        // This section populates the host section
        c.PreSerializeFilters.Add((swagger, httpReq) =>
        {
            swagger.Servers = new List<OpenApiServer> { new OpenApiServer { Url = $"{httpReq.Scheme}://{httpReq.Host.Value}" } };
        });
    });
    app.UseSwaggerUI(c =>
    {
        c.SwaggerEndpoint("/swagger/v1/swagger.json", "Release Date Tracker API V1");
        c.RoutePrefix = string.Empty;
    });

  
}
