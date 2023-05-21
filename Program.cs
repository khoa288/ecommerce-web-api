using LoginJWT;
using LoginJWT.Services;
using LoginJWT.Utils;

var builder = WebApplication.CreateBuilder(args);
ConfigurationManager configuration = builder.Configuration;

builder.Services.AddControllers();
builder.Services.AddEndpointsApiExplorer();

builder.Services.Configure<AppSettings>(
    builder.Configuration.GetSection("ApplicationSettings"));

builder.Services.AddCors(options =>
{
    options.AddDefaultPolicy(
                          policy =>
                          {
                              policy.WithOrigins("http://localhost:3001")
                                                  .AllowAnyHeader()
                                                  .AllowAnyMethod()
                                                  .AllowCredentials();
                          });
});

builder.Services.AddScoped<UserService>();
builder.Services.AddScoped<TwoFactorAuthService>();
builder.Services.AddScoped<AuthorizeFilter>();

var app = builder.Build();

app.UseCors();
app.MapControllers();
app.Run();
