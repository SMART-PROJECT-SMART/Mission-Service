using Mission_Service.Extensions;

var builder = WebApplication.CreateBuilder(args);
builder.Services.AddWebApi();
builder.Services.AddAppConfiguration(builder.Configuration);
builder.Services.AddAssignmentAlgorithmServices();
builder.Services.AddLongLastingRequestProcessing();

var app = builder.Build();

app.UseRouting();
app.MapControllers();

app.Run();
