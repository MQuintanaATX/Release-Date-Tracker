using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.IdentityModel.Tokens;
using Microsoft.OpenApi.Models;
using Release_Date_Tracker.Accessors;
using Release_Date_Tracker.Clients;
using Release_Date_Tracker.Managers;
using Release_Date_Tracker.Models;
using Release_Date_Tracker.Models.Configuration_Settings;
using System.Text;

var builder = WebApplication.CreateBuilder(args);
ConfigureServices(builder.Services, builder.Configuration);

var app = builder.Build();
Configure(app, builder.Services);

app.Run();


void ConfigureServices(IServiceCollection services, ConfigurationManager configuration)
{
    services.AddSingleton<IIgdbAccessor, IgdbAccessor>();
    services.AddSingleton<IGameTitleManager, GameTitleManager>();
    services.AddSingleton<NodaTime.IClock>(NodaTime.SystemClock.Instance);
    services.AddControllers();
    services.AddEndpointsApiExplorer();

    // IGDB API configuration
    var igdbSection = configuration.GetSection("Igdb");
    var igdbConfiguration = igdbSection.Get<IgdbConfiguration>();
    if (igdbConfiguration != null)
    {
        services.AddSingleton(igdbConfiguration);
        services.AddSingleton<ITwitchClient, TwitchClient>();
    }
    else throw new Exception("No IGDB Configuration Section Present");

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
                Name = "Michael Quintana",
                Email = "mquintana78750@gmail.com",
            }
        });
    });

    // Authentication Set up
    var key = configuration["Key"];
    var issuer = configuration["Issuer"];

    var credentials = new Credentials
    {
        Key = key,
        Issuer = issuer
    };

    services.AddSingleton(credentials);

    services.AddAuthentication(JwtBearerDefaults.AuthenticationScheme)
    .AddJwtBearer(x =>
    {
        x.TokenValidationParameters = new TokenValidationParameters
        {
            ValidateIssuer = true,
            ValidateLifetime = true,
            ValidateAudience = true,
            ValidateIssuerSigningKey = true,
            ValidIssuer = issuer,
            ValidAudience = issuer,
            IssuerSigningKey = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(credentials.Key))
       };
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

    // Enable Auth
    app.UseHttpsRedirection();
    app.UseAuthentication();
    app.UseAuthorization(); 
}
