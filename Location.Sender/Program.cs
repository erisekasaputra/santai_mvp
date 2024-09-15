using Microsoft.AspNetCore.SignalR.Client;

var builder = WebApplication.CreateBuilder(args);

builder.Services.AddSignalR();

var app = builder.Build();

var connection = new HubConnectionBuilder()
               .WithUrl("https://localhost:7201/mechanic/location", options =>
               {
                   options.Headers["Authorization"] = "Bearer ";
               })
               .Build();

try
{
    await connection.StartAsync();
}
catch(Exception ex)
{
    Console.WriteLine(ex.Message);
}


await connection.SendAsync("UpdateLocation", Guid.NewGuid(), -8.143145, 112.209617);
await connection.SendAsync("UpdateLocation", Guid.NewGuid(), -8.126070, 112.220602);
await connection.SendAsync("UpdateLocation", Guid.NewGuid(), -8.156657, 112.174672);

app.Run(); 