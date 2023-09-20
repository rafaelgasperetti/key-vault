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
using key_vault.Controllers.Interfaces;

var builder = WebApplication.CreateBuilder(args);

builder.Services.Configure<APIEnvironment>(builder.Configuration.GetSection(nameof(APIEnvironment)));
var env = Initializer.GetEnvironment(builder.Configuration);

Initializer.Migrate(env);

builder.Services.AddSingleton(env);
builder.Services.AddScoped<IDatabase, MySqlDb>();
builder.Services.AddScoped<IEncryption, Encryption>();
builder.Services.AddScoped<IAccountService, AccountService>();
builder.Services.AddScoped<ISecretService, SecretService>();

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
    v.DefaultApiVersion = new ApiVersion(BaseController.GetMajorVersion(), BaseController.GetMinorVersion());
    v.AssumeDefaultVersionWhenUnspecified = true;
    v.ReportApiVersions = true;
});
builder.Services.AddRouting(options => options.LowercaseUrls = true);
builder.Services.AddHttpContextAccessor();

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

app.UseEndpoints(endpoints =>
{
    endpoints.MapControllers();
    endpoints.MapControllerRoute("default", "{controller}/{action}");
});

app.UseResponseCompression();
app.Run();