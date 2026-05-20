using Mission_Service.Extensions;

WebApplicationBuilder builder = WebApplication.CreateBuilder(args);
builder.Services.AddWebApi();
builder.Services.AddAppConfiguration(builder.Configuration);
builder.Services.AddAssignmentAlgorithmServices();
builder.Services.AddBackgroundServices();
builder.Services.AddHttpClients(builder.Configuration);
builder.Services.AddUAVServices();
builder.Services.AddMongoDbServices();
builder.Services.AddMissionExecutor();

WebApplication app = builder.Build();

app.UseRouting();
app.MapControllers();

app.Run();
