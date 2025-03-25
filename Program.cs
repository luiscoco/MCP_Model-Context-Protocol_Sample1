using Microsoft.Extensions.AI;
using ModelContextProtocol.Client;
using ModelContextProtocol.Protocol.Transport;
using OpenAI;

McpClientOptions options = new()
{
    ClientInfo = new() { Name = "TestClient", Version = "1.0.0" }
};

var client = await McpClientFactory.CreateAsync(
    new()
    {
        Id = "everything",
        Name = "Everything",
        TransportType = TransportTypes.StdIo,
        TransportOptions = new()
        {
            ["command"] = "npx",
            ["arguments"] = "-y @modelcontextprotocol/server-everything",
        }
    }, options, null, null, default);

// Print the list of tools available from the server.
await foreach (var tool in client.ListToolsAsync())
{
    Console.WriteLine($"{tool.Name} ({tool.Description})");
}

// Execute a tool (this would normally be driven by LLM tool invocations).
var result = await client.CallToolAsync(
    "echo",
    new() { ["message"] = "Hello MCP!" },
    CancellationToken.None);

// echo always returns one and only one text content object
Console.WriteLine(result.Content.First(c => c.Type == "text").Text);

// Get available functions.
IList<AIFunction> tools = await client.GetAIFunctionsAsync();

// Create an IChatClient. (This shows using OpenAIClient, but it could be any other IChatClient implementation.)
// Provide your own OPENAI_API_KEY via an environment variable.
using IChatClient chatClient =
    new OpenAIClient("OpenAI-API-Key").AsChatClient("gpt-4o-mini")
    .AsBuilder().UseFunctionInvocation().Build();

//Send a message to the LLM using the "echo" tool and get the response.
var response = await chatClient.GetResponseAsync(
    "Hello can you echo Hellow World",
    new()
    {
        Tools = [.. tools],
    });

Console.WriteLine(response);

// Have a conversation, making all tools available to the LLM.
List<ChatMessage> messages = [];
while (true)
{
    Console.Write("Q: ");
    messages.Add(new(ChatRole.User, Console.ReadLine()));

    List<ChatResponseUpdate> updates = [];
    await foreach (var update in chatClient.GetStreamingResponseAsync(messages, new() { Tools = [.. tools] }))
    {
        Console.Write(update);
        updates.Add(update);
    }
    Console.WriteLine();

    messages.AddMessages(updates);
}

