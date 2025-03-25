# Model Context Protocol (MCP): getting start sample



## 0. What is Model Context Protocol (MCP)?

**MCP** is an **open protocol** that standardizes how applications **provide context to LLMs**.

Think of MCP like a USB-C port for AI applications.

Just as USB-C provides a standardized way to connect your devices to various peripherals and accessories,

**MCP** provides a standardized way to **connect AI models** to different **data sources** and **tools**.

For more information visit the MCP official web site: 

https://modelcontextprotocol.io/introduction

## 1. Create the OpenAI API-Key

Create an API-Key in the OpenAI web page:

https://platform.openai.com/api-keys

![image](https://github.com/user-attachments/assets/5868d310-1063-4738-976d-67ddfb432ef7)

## 2. Create a C# console application




### 2.1. Install .NET 10 and Visual Studio 2022 17.4(Preview)




### 2.2. Create a console application with Visual Studio




## 3. Load the Nuget Packages

Load these two nuget packages:

```
  <ItemGroup>
    <PackageReference Include="Microsoft.Extensions.AI.OpenAI" Version="9.3.0-preview.1.25161.3" />
    <PackageReference Include="ModelContextProtocol" Version="0.1.0-preview.1.25171.12" />
  </ItemGroup>
```

![image](https://github.com/user-attachments/assets/ca7d53ad-5e96-4955-a1cf-74cadb638d8e)




## 4. Input the source code

This **C# code** demonstrates how to use the **Model Context Protocol (MCP)** to integrate a tool-based AI system with an LLM (e.g., OpenAI GPT),

and have a dynamic conversation where tools can be used during chat interactions.

MCP Client Configuration
csharp
Copy
Edit
McpClientOptions options = new()
{
    ClientInfo = new() { Name = "TestClient", Version = "1.0.0" }
};
This sets up basic metadata about your client.

🚀 Start MCP Server and Create Client
csharp
Copy
Edit
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
This launches a local tool server (@modelcontextprotocol/server-everything) using npx.

The transport is via StdIO, which means it communicates via standard input/output streams (like CLI).

This client can now interact with the tools served by that MCP server.

🔎 List Available Tools
csharp
Copy
Edit
await foreach (var tool in client.ListToolsAsync())
{
    Console.WriteLine($"{tool.Name} ({tool.Description})");
}
Lists all tools made available by the server (e.g., echo, math, etc.).

🛠️ Call a Tool (Direct Execution)
csharp
Copy
Edit
var result = await client.CallToolAsync(
    "echo",
    new() { ["message"] = "Hello MCP!" },
    CancellationToken.None);

Console.WriteLine(result.Content.First(c => c.Type == "text").Text);
This calls the echo tool, sending it a message.

It prints the result returned from the tool — in this case, the echoed string.

🧠 Get Functions Usable by the LLM
csharp
Copy
Edit
IList<AIFunction> tools = await client.GetAIFunctionsAsync();
Gets the tools in a format that an LLM (e.g., GPT-4) can recognize and use during chat.

🤖 Connect to OpenAI (Chat Client)
csharp
Copy
Edit
using IChatClient chatClient =
    new OpenAIClient("OpenAI-API-Key").AsChatClient("gpt-4o-mini")
    .AsBuilder().UseFunctionInvocation().Build();
Connects to OpenAI using your API key.

Uses GPT-4o-mini.

Enables function/tool invocation, allowing the LLM to use MCP tools when needed.

💬 Ask the LLM to Use a Tool
csharp
Copy
Edit
var response = await chatClient.GetResponseAsync(
    "Hello can you echo Hellow World",
    new()
    {
        Tools = [.. tools],
    });
Console.WriteLine(response);
Asks the LLM a question.

The LLM may decide to invoke a tool (like echo) on its own.

The response includes both the model’s text and the tool output.

🧑‍💻 Interactive Chat Loop with Tool Access
csharp
Copy
Edit
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
A continuous conversation with the LLM.

Sends user input.

Streams the model’s response (including tool usage if needed).

Appends both questions and answers to maintain context.

🔑 Summary
This code:

Spins up a local MCP tool server.

Lets you interact with tools directly or through an LLM.

Enables the LLM to invoke tools dynamically as part of a conversation.

Uses OpenAI GPT to understand your intent and call tools intelligently.

```csharp
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
```




## 5. Run the application an verify the results






