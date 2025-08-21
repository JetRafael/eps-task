using System.Net.WebSockets;
using System.Net;
using EPS.Server.Services;
using EPS.Data.DatabaseConnection;
using Microsoft.EntityFrameworkCore;
using EPS.Data.Services;

var builder = WebApplication.CreateBuilder(args);
//builder.WebHost.UseUrls("http://localhost:6969");

builder.Services.AddDbContext<ApplicationDBContext>(options =>
    options.UseSqlServer(builder.Configuration.GetConnectionString("DefaultConnection"))
);

//Register DB Data service
builder.Services.AddScoped<DataService>();
// Register the Code Generator service
builder.Services.AddScoped<CodeGeneratorService>();
// Register the Code Activation service
builder.Services.AddScoped<CodeActivationService>();

var app = builder.Build();

app.UseWebSockets();

app.Map("/generate", async (HttpContext context, CodeGeneratorService generatorService) => {
    if (context.WebSockets.IsWebSocketRequest)
    {

        var websocket = await context.WebSockets.AcceptWebSocketAsync();

        //
        // TODO
        //

        await generatorService.HandleRequest(context, websocket);
    }
    else {
        context.Response.StatusCode = (int)HttpStatusCode.BadRequest;
    }
});

app.Map("/usecode", async (HttpContext context, CodeActivationService activationService) => {
    if (context.WebSockets.IsWebSocketRequest)
    {
        var websocket = await context.WebSockets.AcceptWebSocketAsync();
        await activationService.HandleRequest(context, websocket);
    }
    else
    {
        context.Response.StatusCode = (int)HttpStatusCode.BadRequest;
    }
});



await app.RunAsync();
