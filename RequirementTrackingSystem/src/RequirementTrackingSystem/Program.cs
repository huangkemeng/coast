using RequirementTrackingSystem.Engines.Bases;
using RequirementTrackingSystem.FilterAndMiddlewares;

var builder = WebApplication.CreateBuilder(args);
builder.Services.AddScoped<AutoResolveFilter>();
var app = builder.BuildWithEngines();
app.Run();