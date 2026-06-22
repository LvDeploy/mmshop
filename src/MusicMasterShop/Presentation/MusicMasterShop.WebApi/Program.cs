using MusicMasterShop.Application.Middleware.Correlation;
using MusicMasterShop.WebApi.Configuration;
using MusicMasterShop.WebApi.Configuration.Logging;
using MusicMasterShop.WebApi.DependencyInjection;

var builder = WebApplication.CreateBuilder(args);

// Add services to the container.

builder.Services.AddControllers();
// Learn more about configuring OpenAPI at https://aka.ms/aspnet/openapi
builder.Services.AddOpenApi();
builder.Services.AddHttpContextAccessor();
builder.AddAppServices();
builder.AddInfraServices();
builder.AddWebApiConfiguration();
builder.AddCorsConfiguration();
builder.AddSwaggerConfiguration();
builder.AddEFContextConfiguration();
builder.AddAuthenticationConfiguration();
builder.AddOpenTelemetryConfiguration();
builder.AddOpenTelemetryLoggingConfiguration();
builder.Services.Configure<WebApiTransactionLogOptions>(
    builder.Configuration.GetSection(WebApiTransactionLogOptions.SectionName));
var app = builder.Build();
EFContextConfiguration.CreateDataBase(app); //CRIA BD APENAS PRA TESTE
// Configure the HTTP request pipeline.
if (app.Environment.IsDevelopment())
{
    app.MapOpenApi();
}

app.UseHttpsRedirection();
app.UseWebApplicationConfiguration();
app.UseMiddleware<CorrelationIdMiddleware>();
app.UseMiddleware<WebApiTransactionLogMiddleware>();
app.UseAuthenticationConfiguration();
app.MapControllers();
app.UseSwaggerConfiguration(app.Environment, app.DescribeApiVersions());
app.Run();
