using Microsoft.AspNetCore.SignalR.Client;

var builder = WebApplication.CreateBuilder(args);

builder.Configuration.AddEnvironmentVariables();
builder.Services.AddSignalR();

var app = builder.Build(); 

var connection = new HubConnectionBuilder()
    .WithUrl("wss://notification.santaitechnology-app.com/v1/ReceiveOrderStatusUpdate", options =>
    {
        options.Headers.Add("Authorization", "Bearer eyJhbGciOiJIUzI1NiIsInR5cCI6IkpXVCJ9.eyJzdWIiOiJkMTdkNGVmOC1hZTMyLTQ5ODEtYjkyZS1mZTA4ODYxMDU4N2UiLCJlbWFpbCI6ImVyaXNla2FzYXB1dHJhMjgyMDAwQGdtYWlsLmNvbSIsInVuaXF1ZV9uYW1lIjoiKzYyODU3OTEzODc1NTgiLCJodHRwOi8vc2NoZW1hcy54bWxzb2FwLm9yZy93cy8yMDA1LzA1L2lkZW50aXR5L2NsYWltcy9tb2JpbGVwaG9uZSI6Iis2Mjg1NzkxMzg3NTU4Iiwicm9sZSI6IkFkbWluaXN0cmF0b3IiLCJ1c2VyX3R5cGUiOiJBZG1pbmlzdHJhdG9yIiwibmJmIjoxNzI3ODEwNTg3LCJleHAiOjE3Mjc4MTQxODcsImlhdCI6MTcyNzgxMDU4NywiaXNzIjoic2FudGFpdGVjaG5vbG9neS1hcGkuY29tIiwiYXVkIjoic2FudGFpdGVjaG5vbG9neS1hcGkuY29tIn0.2-6t3kDNEbAaTVuIxHNgYDoVCA1N8VAqOuMX1hJk9Xo");
        options.Headers.Add("x-api-key", "udIfHWsDMN26rglBkGqa9jCuQysIkUn2RqB3eEL1");
        options.Transports = Microsoft.AspNetCore.Http.Connections.HttpTransportType.WebSockets;
    })
    .WithAutomaticReconnect()
    .Build();

connection.On<string, string, string, string, string, string, string>("ReceiveOrderStatusUpdate",
    (orderId, buyerId, buyerName, mechanicId, mechanicName, orderStatus, actionUrl) =>
    {
        Console.WriteLine("Order Status Updated:");
        Console.WriteLine($"Order ID: {orderId}");
        Console.WriteLine($"Buyer ID: {buyerId}");
        Console.WriteLine($"Buyer Name: {buyerName}");
        Console.WriteLine($"Mechanic ID: {mechanicId}");
        Console.WriteLine($"Mechanic Name: {mechanicName}");
        Console.WriteLine($"Order Status: {orderStatus}");
        Console.WriteLine($"Action URL: {actionUrl}");
    });

try
{ 
    await connection.StartAsync();
    Console.WriteLine("SignalR client connected."); 
    Console.WriteLine("Listening for events. Press any key to exit.");
    Console.ReadKey();
}
catch(Exception ex)
{ 
    Console.WriteLine(ex.Message);
    Console.ReadKey();
} 
app.Run(); 

