using Mission_Service.Extensions;
using Mission_Service.Common.Constants;

var builder = WebApplication.CreateBuilder(args);
builder.Services.AddWebApi();
builder.Services.AddAppConfiguration(builder.Configuration);
builder.Services.AddAssignmentAlgorithmServices();
builder.Services.AddBackgroundServices();
builder.Services.AddHttpClients(builder.Configuration);
builder.Services.AddUAVServices();
builder.Services.AddMongoDbServices();
builder.Services.AddQuartzServices();

builder.Services.AddHttpClient(MissionServiceConstants.HttpClients.SIMULATOR_CLIENT, client =>
{
    client.BaseAddress = new Uri(builder.Configuration[MissionServiceConstants.Configuration.SIMULATOR_BASE_URL_KEY]);
    client.Timeout = TimeSpan.FromMinutes(5);
});

var app = builder.Build();

app.UseRouting();
app.MapControllers();

app.Run();
