# Model Context Protocol (MCP): Getting Started (MCP Client)

In this post, we demonstrate how to build a C# Console Application that acts as an MCP (Model Context Protocol) Client.

The goal is to enable OpenAI's API to access and invoke tools exposed by an MCP Server — specifically,

a Node.js-based server provided by the following package (https://www.npmjs.com/package/@modelcontextprotocol/server-everything):

```
@modelcontextprotocol/server-everything (NPM)
```

This server includes a variety of ready-to-use tools (such as echo, math, and more), which can be dynamically invoked by an LLM like GPT-4 via OpenAI's function calling interface.

**Technologies Used**

a) .NET 10

b) Visual Studio 2022 (version 17.4)

c) ModelContextProtocol SDK

d) OpenAI .NET SDK

e) Node.js (via npx) to launch the MCP tool server

## 0. What is Model Context Protocol (MCP)?

**MCP** is an **open protocol** that standardizes how applications **provide context to LLMs**.

Think of MCP like a USB-C port for AI applications.

Just as USB-C provides a standardized way to connect your devices to various peripherals and accessories,

**MCP** provides a standardized way to **connect AI models** to different **data sources** and **tools**.

For more information visit the MCP official web site: 

https://modelcontextprotocol.io/introduction

![image](https://github.com/user-attachments/assets/a97f3e57-7fc2-4597-8d2e-ae950ceed77e)

![image](https://github.com/user-attachments/assets/09e1bbdf-9f1c-49cb-be9a-f4eeb66569b3)

Here are some MCP Servers samples:

https://blog.stackademic.com/whatre-mcp-servers-about-10-mcp-servers-you-can-try-now-57f061479d81

## 1. Prerrequisites

### 1.1. Create the OpenAI API-Key

Create an API-Key in the OpenAI web page:

https://platform.openai.com/api-keys

![image](https://github.com/user-attachments/assets/5868d310-1063-4738-976d-67ddfb432ef7)

### 1.2. Install .NET 10 and Visual Studio 2022 17.4(Preview Edition)

Visit this site to download and install **.NET 10**

https://dotnet.microsoft.com/en-us/download/dotnet/10.0

![image](https://github.com/user-attachments/assets/c8ed8f22-96b3-468d-ba03-f0f488186596)

Visit this site to download and install **Visual Studio 2022 v17.4 (Preview Edition)**

https://visualstudio.microsoft.com/vs/preview/#download-preview

![image](https://github.com/user-attachments/assets/7fb48da9-9ef7-44a4-ada7-6eebdc42a09d)

## 2. Create a .NET 10 C# console application

We run **Visual Studio 2022 v17.4** and create a new project

![image](https://github.com/user-attachments/assets/1901fc87-815d-4228-8870-f26d34bd77c1)

We select the **C# console application template** and press the Next button

![image](https://github.com/user-attachments/assets/78caa1aa-73f4-458d-8005-5065bd6e6ede)

We input the project name and location and press the Next button

![image](https://github.com/user-attachments/assets/bbc2f1c1-e66c-42ed-b982-0d71bf366b30)

We select the **.NET 10** framework a press the Create button

![image](https://github.com/user-attachments/assets/456b7cae-866c-4bdb-9a1d-46baa6e72fe3)

## 3. Load the Nuget Packages

Load these two nuget packages:

```
  <ItemGroup>
    <PackageReference Include="Microsoft.Extensions.AI.OpenAI" Version="9.3.0-preview.1.25161.3" />
    <PackageReference Include="ModelContextProtocol" Version="0.1.0-preview.1.25171.12" />
  </ItemGroup>
```

![image](https://github.com/user-attachments/assets/ca7d53ad-5e96-4955-a1cf-74cadb638d8e)

We can see in the following picture the ModelContextProtocol Nuget package web site:

![image](https://github.com/user-attachments/assets/e44d465e-c974-4565-b913-ebda24d13087)

## 4. Explaining sample source code

This **C#** sample demonstrates how to use the **Model Context Protocol (MCP)** to integrate a tool-based AI system with an LLM (e.g., OpenAI GPT), and have a dynamic conversation where tools can be used during chat interactions.

### 4.1. MCP Client Configuration

This sets up basic **metadata** about your **client**.

```csharp
McpClientOptions options = new()
{
    ClientInfo = new() { Name = "TestClient", Version = "1.0.0" }
};
```

### 4.2. Start MCP Server and Create Client

This launches a local tool server (@modelcontextprotocol/server-everything) using npx.

The transport is via StdIO, which means it communicates via standard input/output streams (like CLI).

This client can now interact with the tools served by that MCP server.

This line of code is the core setup that creates an **MCP client** which connects to a tool **MCP Server** running via **npx**

```csharp
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
```

**IMPORTANT NOTE**: in this sample the **MCP Server** is provided by a NodeJS package

This launches a **Node.js tool server** defined in the MCP ecosystem (@modelcontextprotocol/server-everything).

It’s a prebuilt server that exposes multiple tools (like echo, math, code, etc.).

```
npx -y @modelcontextprotocol/server-everything
```

Visit the following site for more information about this **MCP Server**

https://www.npmjs.com/package/@modelcontextprotocol/server-everything

![image](https://github.com/user-attachments/assets/7c0860c6-392a-4526-95fb-5f1504a4a811)

### 4.3. List Available Tools

Lists all tools made available by the server (e.g., echo, math, etc.).

```csharp
await foreach (var tool in client.ListToolsAsync())
{
    Console.WriteLine($"{tool.Name} ({tool.Description})");
}
```

These are the available tools provided by the MCP Server (https://www.npmjs.com/package/@modelcontextprotocol/server-everything):

![image](https://github.com/user-attachments/assets/c77962a9-8a60-42df-9ac2-88f8f535d4f3)

![image](https://github.com/user-attachments/assets/aff9920c-7fa6-4552-be6f-e025a5214a37)

### 4.4. Call a Tool (Direct Execution)

This calls the echo tool, sending it a message.

It prints the result returned from the tool — in this case, the echoed string.

```csharp
var result = await client.CallToolAsync(
    "echo",
    new() { ["message"] = "Hello MCP!" },
    CancellationToken.None);

Console.WriteLine(result.Content.First(c => c.Type == "text").Text);
```

### 4.5. Get Functions Usable by the LLM

Gets the tools in a format that an LLM (e.g., GPT-4) can recognize and use during chat.

```csharp
IList<AIFunction> tools = await client.GetAIFunctionsAsync();
```

### 4.6. Connect to OpenAI (Chat Client)

Connects to OpenAI using your API key.

Uses GPT-4o-mini.

Enables function/tool invocation, allowing the LLM to use MCP tools when needed.

```csharp
using IChatClient chatClient =
    new OpenAIClient("OpenAI-API-Key").AsChatClient("gpt-4o-mini")
    .AsBuilder().UseFunctionInvocation().Build();
```

Copy the **OpenAI API-Key** and paste in the **OpenAIClient** method call

![image](https://github.com/user-attachments/assets/a613195e-8c15-4829-9220-37814d69480a)

### 4.7. Ask the LLM to Use a Tool

Asks the LLM a question.

The LLM may decide to invoke a tool (like echo) on its own.

The response includes both the model’s text and the tool output.

```csharp
var response = await chatClient.GetResponseAsync(
    "Hello can you echo Hellow World",
    new()
    {
        Tools = [.. tools],
    });
Console.WriteLine(response);
```

### 4.8. Interactive Chat Loop with Tool Access

A continuous conversation with the LLM.

Sends user input.

Streams the model’s response (including tool usage if needed).

Appends both questions and answers to maintain context.

```csharp
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

### 4.9. Summary

This code:

a) Spins up a local MCP tool server.

b) Lets you interact with tools directly or through an LLM.

c) Enables the LLM to invoke tools dynamically as part of a conversation.

d) Uses OpenAI GPT to understand your intent and call tools intelligently.

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

![image](https://github.com/user-attachments/assets/9505d287-7866-4336-b282-c1bc5cd2616b)





