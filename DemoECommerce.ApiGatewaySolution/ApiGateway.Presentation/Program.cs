using Ocelot.Cache.CacheManager;
using Ocelot.DependencyInjection;
using ECommerce.SharedLibrary.DependencyInjection;

var builder = WebApplication.CreateBuilder(args);
var app = builder.Build();

builder.Configuration.AddJsonFile("ocelot.json", optional:false, reloadOnChange:true);
builder.Services.AddOcelot().AddCacheManager(x=> x.WithDictionaryHandle());
builder.Services.AddJWTAuthenticationScheme(builder.Configuration);

// Configure the HTTP request pipeline.
app.UseHttpsRedirection();



app.Run();
