using Ocelot.Cache.CacheManager;
using Ocelot.DependencyInjection;
using ECommerce.SharedLibrary.DependencyInjection;
using ApiGateway.Presentation.Middlewares;
using Ocelot.Middleware;

var builder = WebApplication.CreateBuilder(args);


builder.Configuration.AddJsonFile("ocelot.json", optional:false, reloadOnChange:true);
builder.Services.AddOcelot().AddCacheManager(x=> x.WithDictionaryHandle());
builder.Services.AddJWTAuthenticationScheme(builder.Configuration);
builder.Services.AddCors(options =>
{
    options.AddDefaultPolicy(builder =>
        {
            builder.AllowAnyHeader()
            .AllowAnyMethod()
            .AllowAnyOrigin();
        });
});
var app = builder.Build();

// Configure the HTTP request pipeline.
app.UseCors();
app.UseMiddleware<AttachSignatureToRequest>();
app.UseOcelot().Wait();
app.UseHttpsRedirection();

app.Run();
