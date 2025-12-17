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
builder.Services.AddSimulatorHttpClient(builder.Configuration);

var app = builder.Build();

app.UseRouting();
app.MapControllers();

app.Run();
