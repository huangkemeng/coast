using Coast.Api.Engines.Bases;
using Coast.Api.FilterAndMiddlewares;

var builder = WebApplication.CreateBuilder(args);
builder.Services.AddScoped<AutoResolveFilter>();
var app = builder.BuildWithEngines();
app.Run();