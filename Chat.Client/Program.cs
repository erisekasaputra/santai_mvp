using Microsoft.AspNetCore.Http.Connections;
using Microsoft.AspNetCore.SignalR.Client;

string? token = null;
string? destinationUserId = null;
string? url = null;

// Find the token and destination user ID from command-line arguments
for (int i = 0; i < args.Length; i++)
{
    if (args[i] == "--token" && i + 1 < args.Length)
    {
        token = args[i + 1];
        continue;
    }

    if (args[i] == "--destination" && i + 1 < args.Length)
    {
        destinationUserId = args[i + 1];
        continue;
    }

    if (args[i] == "--url" && i + 1 < args.Length)
    {
        url = args[i + 1];
        continue;
    }
}

if (string.IsNullOrEmpty(token))
{
    Console.WriteLine("Please provide the Bearer token using --token <your_token>.");
    return;
}

if (string.IsNullOrEmpty(destinationUserId))
{
    Console.WriteLine("Please provide the destination user ID using --destination <user_id>.");
    return;
}

if (string.IsNullOrEmpty(url))
{
    Console.WriteLine("Please provide the url using --url <url>.");
    return;
}

// Build the SignalR connection
var connection = new HubConnectionBuilder()
    .WithUrl(url, options =>
    {
        options.AccessTokenProvider = async () =>
        {
            return await Task.FromResult(token);
        };

        options.Transports = HttpTransportType.WebSockets;
    })
    .Build();

// Define the handler for receiving chat messages
connection.On<string, string, string, string, string, string>("ReceiveChat", (originUserId, messageId, text, replyMessageId, replyMessageText, timestamp) =>
{
    Console.WriteLine($"On: {replyMessageId} with {replyMessageText}");
    Console.WriteLine($"Received message from {originUserId}: {text}");
});

// Define the handler for server errors
connection.On<string>("InternalServerError", (errorMessage) =>
{
    Console.WriteLine($"Error: {errorMessage}");
});

// Start the SignalR connection
await connection.StartAsync();
Console.WriteLine("Connection started");

// Message sending loop
while (true)    
{
    Console.WriteLine("Enter message to send:");
    var message = Console.ReadLine();

    if (string.IsNullOrEmpty(message))
    {
        Console.WriteLine("Message cannot be empty. Please try again.");
        continue;
    }

    // Send the message to the destination user
    await connection.InvokeAsync("SendMessageToUser", destinationUserId, message, string.Empty, string.Empty);
}
