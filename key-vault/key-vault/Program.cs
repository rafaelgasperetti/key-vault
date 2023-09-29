using key_vault.Models;
using Newtonsoft.Json.Serialization;
using Newtonsoft.Json;
using Microsoft.AspNetCore.Mvc;
using key_vault.Database.Interfaces;
using key_vault.Database;
using key_vault.Initializer;
using key_vault.Services.Interfaces;
using key_vault.Services;
using key_vault.Initializer.Jwt.Interfaces;
using key_vault.Initializer.Jwt;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.IdentityModel.Tokens;
using System.Text;
using Microsoft.Extensions.Diagnostics.HealthChecks;
using Microsoft.AspNetCore.Diagnostics.HealthChecks;

var builder = WebApplication.CreateBuilder(args);

builder.Services.Configure<APIEnvironment>(builder.Configuration.GetSection(nameof(APIEnvironment)));
var env = Initializer.GetAPIEnvironment(builder.Configuration);

Initializer.Initialize(builder.Services, env);
builder.Services.AddSingleton(env);
builder.Services.AddScoped<IDatabase, MySqlDb>();
builder.Services.AddScoped<IEncryption, Encryption>();
builder.Services.AddScoped<IAccountService, AccountService>();
builder.Services.AddScoped<ISecretService, SecretService>();

builder.Services.AddHealthChecks().AddCheck("health", () =>
{
    return HealthCheckResult.Healthy("Healthier");
    /*try
    {
        using var db = new MySqlDb(env);

        if (db.IsHealhty())
        {
            return HealthCheckResult.Healthy(string.Format(Strings.ApplicationRunningMessage, env.Version.ToString()));
        }
        else
        {
            var notHealthyPingMessage = string.Format(Strings.ApplicationUnhealthyPingFailed, env.DatabaseHost, env.DatabasePort);
            return HealthCheckResult.Unhealthy(string.Format(Strings.ApplicationRunningMessage, env.Version.ToString(), notHealthyPingMessage));
        }
    }
    catch (Exception ex)
    {
        return HealthCheckResult.Unhealthy(string.Format(Strings.ApplicationRunningMessage, env.Version.ToString(), ex.Message));
    }*/
});

builder.Services.AddControllers().AddNewtonsoftJson(options =>
{
    options.SerializerSettings.ContractResolver = new DefaultContractResolver();
    options.SerializerSettings.Formatting = Formatting.None;
    options.SerializerSettings.ReferenceLoopHandling = ReferenceLoopHandling.Ignore;
    options.SerializerSettings.MaxDepth = null;
    options.SerializerSettings.NullValueHandling = NullValueHandling.Ignore;
});
builder.Services.AddEndpointsApiExplorer();
builder.Services.AddApiVersioning(v =>
{
    v.DefaultApiVersion = new ApiVersion(env.Version.Major, env.Version.Minor);
    v.AssumeDefaultVersionWhenUnspecified = true;
    v.ReportApiVersions = true;
});
builder.Services.AddRouting(options => options.LowercaseUrls = true);

builder.Services.AddAuthentication(options =>
{
    options.DefaultAuthenticateScheme = JwtBearerDefaults.AuthenticationScheme;
    options.DefaultChallengeScheme = JwtBearerDefaults.AuthenticationScheme;
})
.AddJwtBearer(o =>
{
    o.RequireHttpsMetadata = false;
    o.SaveToken = false;
    o.TokenValidationParameters = new TokenValidationParameters
    {
        ValidateIssuerSigningKey = true,
        ValidateIssuer = true,
        ValidateAudience = true,
        ValidateLifetime = false,
        ClockSkew = TimeSpan.Zero,
        ValidIssuer = env.JWTIssuer,
        ValidAudience = env.JWTAudience,
        IssuerSigningKey = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(env.Secret))
    };
});
builder.Services.AddAuthorization();
builder.Services.AddResponseCompression();

var app = builder.Build();

app.UseMiddleware<CustomMiddleware>();

app.UseHttpsRedirection();
app.UseStaticFiles();
app.UseRouting();
app.UseAuthorization();

app.UseHealthChecks("/health");

app.UseEndpoints(endpoints =>
{
    endpoints.MapHealthChecks("/health", new HealthCheckOptions
    {
        ResponseWriter = async (context, report) =>
        {
            context.Response.ContentType = "application/json";
            await context.Response.WriteAsync(Enum.GetName(report.Status));
        }
    });

    endpoints.MapControllers();
    endpoints.MapControllerRoute("default", "{controller}/{action}");
});

app.UseResponseCompression();
app.Run();