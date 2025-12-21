using Mission_Service.Common.Constants;
using Mission_Service.Extensions;

var builder = WebApplication.CreateBuilder(args);
builder.Services.AddWebApi();
builder.Services.AddAppConfiguration(builder.Configuration);
builder.Services.AddAssignmentAlgorithmServices();
builder.Services.AddBackgroundServices();
builder.Services.AddHttpClients(builder.Configuration);
builder.Services.AddUAVServices();
builder.Services.AddMongoDbServices();
builder.Services.AddMissionExecutor();
builder.Services.AddSimulatorHttpClient(builder.Configuration);

var app = builder.Build();

app.UseRouting();
app.MapControllers();

app.Run();
