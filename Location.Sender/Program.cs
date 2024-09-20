using Microsoft.AspNetCore.SignalR.Client;

var builder = WebApplication.CreateBuilder(args);

builder.Configuration.AddEnvironmentVariables();
builder.Services.AddSignalR();

var app = builder.Build();

var mechanic1 = new HubConnectionBuilder()
    .WithUrl("https://localhost:7201/mechanic/location", options =>
    {
        options.Headers["Authorization"] = "Bearer eyJhbGciOiJIUzI1NiIsInR5cCI6IkpXVCJ9.eyJzdWIiOiI5NmFkNTliZi0yNGY0LTRkY2EtODFiNy00ZTBiNjI1OWFhMWQiLCJ1bmlxdWVfbmFtZSI6Iis2Mjg1NzkxMzg3NTUxIiwiaHR0cDovL3NjaGVtYXMueG1sc29hcC5vcmcvd3MvMjAwNS8wNS9pZGVudGl0eS9jbGFpbXMvbW9iaWxlcGhvbmUiOiIrNjI4NTc5MTM4NzU1MSIsInVzZXJfdHlwZSI6Ik1lY2hhbmljVXNlciIsInJvbGUiOiJNZWNoYW5pY1VzZXIiLCJuYmYiOjE3MjY0MDE2NDYsImV4cCI6MTcyNjQwNTI0NiwiaWF0IjoxNzI2NDAxNjQ2LCJpc3MiOiJpZGVudGl0eS5zYW50YWltdnAuY29tIiwiYXVkIjoic2FudGFpbXZwLmNvbSJ9.9w4Wk7mvPPfr8_hAtuFJRJmVkJH7mA16KTC8YINnazc";
    }).Build();

var mechanic2 = new HubConnectionBuilder()
    .WithUrl("https://localhost:7201/mechanic/location", options =>
    {
        options.Headers["Authorization"] = "Bearer eyJhbGciOiJIUzI1NiIsInR5cCI6IkpXVCJ9.eyJzdWIiOiI5NmFkNTliZi0yNGY0LTRkY2EtODFiNy00ZTBiNjI1OWFhMWQiLCJ1bmlxdWVfbmFtZSI6Iis2Mjg1NzkxMzg3NTUxIiwiaHR0cDovL3NjaGVtYXMueG1sc29hcC5vcmcvd3MvMjAwNS8wNS9pZGVudGl0eS9jbGFpbXMvbW9iaWxlcGhvbmUiOiIrNjI4NTc5MTM4NzU1MSIsInVzZXJfdHlwZSI6Ik1lY2hhbmljVXNlciIsInJvbGUiOiJNZWNoYW5pY1VzZXIiLCJuYmYiOjE3MjY0MDE2NDYsImV4cCI6MTcyNjQwNTI0NiwiaWF0IjoxNzI2NDAxNjQ2LCJpc3MiOiJpZGVudGl0eS5zYW50YWltdnAuY29tIiwiYXVkIjoic2FudGFpbXZwLmNvbSJ9.9w4Wk7mvPPfr8_hAtuFJRJmVkJH7mA16KTC8YINnazc";
    }).Build();

var mechanic3 = new HubConnectionBuilder()
    .WithUrl("https://localhost:7921/notification", options =>
    {
        options.Headers["Authorization"] = "Bearer eyJhbGciOiJIUzI1NiIsInR5cCI6IkpXVCJ9.eyJzdWIiOiI5NmFkNTliZi0yNGY0LTRkY2EtODFiNy00ZTBiNjI1OWFhMWQiLCJ1bmlxdWVfbmFtZSI6Iis2Mjg1NzkxMzg3NTUxIiwiaHR0cDovL3NjaGVtYXMueG1sc29hcC5vcmcvd3MvMjAwNS8wNS9pZGVudGl0eS9jbGFpbXMvbW9iaWxlcGhvbmUiOiIrNjI4NTc5MTM4NzU1MSIsInVzZXJfdHlwZSI6Ik1lY2hhbmljVXNlciIsInJvbGUiOiJNZWNoYW5pY1VzZXIiLCJuYmYiOjE3MjY0MDE2NDYsImV4cCI6MTcyNjQwNTI0NiwiaWF0IjoxNzI2NDAxNjQ2LCJpc3MiOiJpZGVudGl0eS5zYW50YWltdnAuY29tIiwiYXVkIjoic2FudGFpbXZwLmNvbSJ9.9w4Wk7mvPPfr8_hAtuFJRJmVkJH7mA16KTC8YINnazc";
    }).Build();

try
{
    //await mechanic1.StartAsync();
    await mechanic3.StartAsync();
}
catch(Exception ex)
{
    Console.WriteLine("Error happen");
    Console.WriteLine(ex.Message);
}

mechanic3.On("ReceiveOrderStatusUpdate", (
        string orderId,
        string buyerId,
        string buyerName,
        string mechanicId,
        string mechanicName,
        string update) =>
{
    Console.WriteLine($"Order ID: {orderId}"); 
    Console.WriteLine($"Order ID: {buyerId}"); 
    Console.WriteLine($"Order ID: {buyerName}"); 
    Console.WriteLine($"Order ID: {mechanicId}"); 
    Console.WriteLine($"Order ID: {mechanicName}"); 
    Console.WriteLine($"Order ID: {update}"); 
});


await Task.Run( async() =>
{
    while (true)   
    { 
        await mechanic3.SendAsync("UpdateLocation", -8.143145, 112.2096);

        Console.WriteLine("Location Sent from Mechanic 1");

        await Task.Delay(1000);
    }
});

//await Task.Run(async () =>
//{
//    while (true)
//    {
//        await mechanic2.SendAsync("UpdateLocation", -8.143145, 112.209617); 

//        Console.WriteLine("Location Sent from Mechanic 2");

//        await Task.Delay(1000);
//    }
//});

app.Run(); 

