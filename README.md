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






## 5. Run the application an verify the results






