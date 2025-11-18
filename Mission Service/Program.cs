using Mission_Service.Extensions;

var builder = WebApplication.CreateBuilder(args);
builder.Services.AddWebApi();
builder.Services.AddAppConfiguration(builder.Configuration);
var app = builder.Build();
app.Run();
