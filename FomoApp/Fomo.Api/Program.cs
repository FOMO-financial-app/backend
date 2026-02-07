using Fomo.Api.Helpers;
using Fomo.Application.Services;
using Fomo.Infrastructure.ExternalServices.MailService;
using Fomo.Infrastructure.ExternalServices.MailService.Alerts;
using Fomo.Infrastructure.ExternalServices.StockService;
using Fomo.Infrastructure.Persistence;
using Fomo.Infrastructure.Repositories;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.EntityFrameworkCore;
using Microsoft.IdentityModel.Tokens;


var builder = WebApplication.CreateBuilder(args);

builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen();

builder.Services.AddControllers();
builder.Services.AddOpenApi();

builder.Services.AddHttpClient<IExternalApiHelper, ExternalApiHelper>();

var Policy = "AllowFrontend";

builder.Services.AddCors(options =>
{
    options.AddPolicy(name: Policy,
    policy =>
    {
        policy
            .WithOrigins($"{builder.Configuration["FrontendUrl:Url"]}")
            .AllowAnyMethod()
            .AllowAnyHeader();
    });
});

builder.Services
    .AddAuthentication(JwtBearerDefaults.AuthenticationScheme)
    .AddJwtBearer(options =>
    {
        options.Authority = $"https://{builder.Configuration["Auth0:Domain"]}/";

        options.TokenValidationParameters = new TokenValidationParameters
        {
            ValidateIssuer = true,
            ValidIssuer = $"https://{builder.Configuration["Auth0:Domain"]}/",

            ValidateAudience = true,
            ValidAudiences = new[]
            {
                builder.Configuration["Auth0:Audience"]
            },

            ValidateLifetime = true
        };

        options.Events = new JwtBearerEvents
        {
            OnTokenValidated = _ =>
            {
                Console.WriteLine(" TOKEN VALIDATED");
                return Task.CompletedTask;
            },
            OnAuthenticationFailed = context =>
            {
                Console.WriteLine(" AUTH FAILED");
                Console.WriteLine(context.Exception);
                return Task.CompletedTask;
            }
        };
    });

builder.Services.AddAuthorization();

builder.Services.AddDbContext<EFCoreDbContext>(options =>
{
    options.UseSqlServer(builder.Configuration["ConnectionString:EFCoreDBConnection"]);
});

builder.Services.Configure<ApiSettings>(builder.Configuration.GetSection("ApiSettings"));

builder.Services.Configure<TwelveData>(builder.Configuration.GetSection("TwelveData"));

builder.Services.Configure<EmailSettings>(builder.Configuration.GetSection("EmailSettings"));

builder.Services.AddScoped<IExternalApiHelper, ExternalApiHelper>();

builder.Services.AddScoped<ITwelveDataService, TwelveDataService>();

builder.Services.AddScoped<IIndicatorService, IndicatorService>();

builder.Services.AddScoped<IUserRepository, UserRepository>();

builder.Services.AddScoped<ITradeResultRepository, TradeResultRepository>();

builder.Services.AddScoped<IUserValidateHelper, UserValidateHelper>();

builder.Services.AddScoped<ITradeResultValidateHelper, TradeResultValidateHelper>();

builder.Services.AddScoped<IStockRepository, StockRepository>();

builder.Services.AddScoped<IEmailService, EmailService>();

builder.Services.AddScoped<IGenericAlert, GenericAlert>();

builder.Services.AddScoped<IAlertService, AlertService>();

builder.Services.AddScoped<IStockSyncService, StockSyncService>();

var app = builder.Build();

if (app.Environment.IsDevelopment())
{
    app.MapOpenApi();
    app.UseSwagger();
    app.UseSwaggerUI();
}

app.UseHttpsRedirection();

app.UseCors(Policy);

app.UseAuthentication();
app.UseAuthorization();

app.MapControllers();

using (var scope = app.Services.CreateScope())
{
    var syncService = scope.ServiceProvider.GetRequiredService<IStockSyncService>();
    await syncService.SyncStockDb();
}

app.Run();
