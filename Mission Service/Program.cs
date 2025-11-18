using Mission_Service.Extensions;

var builder = WebApplication.CreateBuilder(args);
builder.Services.AddWebApi();
builder.Services.AddAlgorithmConfig(builder.Configuration);
builder.Services.AddTelemetryWeightsConfig(builder.Configuration);
var app = builder.Build();
app.Run();
