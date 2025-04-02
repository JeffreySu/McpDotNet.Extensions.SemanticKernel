using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Logging;
using Microsoft.SemanticKernel;
using Microsoft.SemanticKernel.Connectors.OpenAI;
using ModelContextProtocol.SemanticKernel.Extensions;

var cts = new CancellationTokenSource();

var builder = Kernel.CreateBuilder();
builder.Services.AddLogging(c => c.AddDebug().SetMinimumLevel(LogLevel.Trace));

builder.Services.AddOpenAIChatCompletion(
    serviceId: "openai",
    modelId: "gpt-4o",
    apiKey: Environment.GetEnvironmentVariable("OPENAI_API_KEY")!);

var kernel = builder.Build();

await kernel.Plugins.AddToolsFromClaudeDesktopConfigAsync(cancellationToken: cts.Token);

//var everyThingTransportOptions = new Dictionary<string, string>
//{
//    ["command"] = "npx",
//    ["arguments"] = "-y @modelcontextprotocol/server-everything"
//};
//await kernel.Plugins.AddMcpFunctionsFromStdioServerAsync("Everything", everyThingTransportOptions, cancellationToken: cts.Token);

//var everyThingTransportOptions = new Dictionary<string, string>
//{
//    ["command"] = "mcpserver.everything.stdio"
//};
//await kernel.Plugins.AddMcpFunctionsFromStdioServerAsync("Everything", everyThingTransportOptions, cancellationToken: cts.Token);

//var fileSystemTransportOptions = new Dictionary<string, string>
//{
//    ["command"] = "npx",
//    ["arguments"] = "-y @modelcontextprotocol/server-filesystem c:/temp"
//};
//await kernel.Plugins.AddMcpFunctionsFromStdioServerAsync("FileSystem", fileSystemTransportOptions, cancellationToken: cts.Token);

var githubTransportOptions = new Dictionary<string, string>
{
    ["command"] = "npx",
    ["arguments"] = "-y @modelcontextprotocol/server-github"
};
await kernel.Plugins.AddMcpFunctionsFromStdioServerAsync("GitHub", githubTransportOptions, cancellationToken: cts.Token);

// https://github.com/Tiberriver256/mcp-server-azure-devops
//var azureDevOpsTransportOptions = new Dictionary<string, string>
//{
//    ["command"] = "npx",
//    ["arguments"] = "-y @tiberriver256/mcp-server-azure-devops",
//    ["env:AZURE_DEVOPS_ORG_URL"] = "https://dev.azure.com/mstack",
//    ["env:AZURE_DEVOPS_AUTH_METHOD"] = "pat",
//    ["env:AZURE_DEVOPS_PAT"] = Environment.GetEnvironmentVariable("MCP_PAT")!,
//    ["env:AZURE_DEVOPS_DEFAULT_PROJECT"] = "AzureExampleProjects"
//};
//await kernel.Plugins.AddMcpFunctionsFromStdioServerAsync("AzureDevOps2", azureDevOpsTransportOptions, cancellationToken: cts.Token);

var azureDevOpsTransportOptions = new Dictionary<string, string>
{
    ["command"] = "dotnet run --project",
    ["arguments"] = "C:\\dev\\GitHub\\mcpserver.azuredevops\\src\\mcpserver.azuredevops.stdio\\mcpserver.azuredevops.stdio.csproj",
    ["env:AZURE_DEVOPS_ORG_URL"] = "https://dev.azure.com/mstack",
    ["env:AZURE_DEVOPS_AUTH_METHOD"] = "pat",
    ["env:AZURE_DEVOPS_PAT"] = Environment.GetEnvironmentVariable("MCP_PAT")!,
    ["env:AZURE_DEVOPS_DEFAULT_PROJECT"] = "AzureExampleProjects"
};
await kernel.Plugins.AddMcpFunctionsFromStdioServerAsync("AzureDevOpsStef", azureDevOpsTransportOptions, cancellationToken: cts.Token);

//dotnet run --project C:\\dev\\GitHub\\mcpserver.azuredevops\\src\\mcpserver.azuredevops.stdio\\mcpserver.azuredevops.stdio.csproj\" env:AZURE_DEVOPS_ORG_URL=https://dev.azure.com/mstack env:AZURE_DEVOPS_PAT=Bu2aQnaiRGdSGMGDBhAR9qBPeDXvCJo87QhIicNOIL80xs7ZvuJoJQQJ99BCACAAAAAo759gAAASAZDO1CXQ"

//var openXmlTransportOptions = new Dictionary<string, string>
//{
//    //["command"] = @"C:\dev\GitHub\McpDotNet.Extensions.SemanticKernel\wip\ModelContextProtocolServer.OpenXml.Stdio\bin\Release\net8.0\ModelContextProtocolServer.OpenXml.Stdio.exe",
//    ["command"] = "mcpserver.openxml.stdio",
//    ["arguments"] = "allowedPath=c:\\temp"
//};
//await kernel.Plugins.AddMcpFunctionsFromStdioServerAsync("OpenXML", openXmlTransportOptions, cancellationToken: cts.Token);

var executionSettings = new OpenAIPromptExecutionSettings
{
    Temperature = 0.1,
    FunctionChoiceBehavior = FunctionChoiceBehavior.Auto()
};

var result = await kernel.InvokePromptAsync("Which tools are currently registered?", new(executionSettings)).ConfigureAwait(false);
Console.WriteLine($"\n\nTools:\n{result}");

var promptReadFile = "Read the file 'CV.docx' and return all text and format as markdown.";
var resultReadFile = await kernel.InvokePromptAsync(promptReadFile, new(executionSettings)).ConfigureAwait(false);
Console.WriteLine($"\n\n{promptReadFile}\n{resultReadFile}");

//var prompt1 = "Please call the echo tool with the string 'Hello Stef!' and give me the response as-is.";
//var result1 = await kernel.InvokePromptAsync(prompt1, new(executionSettings)).ConfigureAwait(false);
//Console.WriteLine($"\n\n{prompt1}\n{result1}");

//var prompt2 = "Summarize the last 3 commits to the StefH/FluentBuilder repository.";
//var result2 = await kernel.InvokePromptAsync(prompt2, new(executionSettings)).ConfigureAwait(false);
//Console.WriteLine($"\n\n{prompt2}\n{result2}");

var promptAzureDevops1 = "Get 3 commits from Azure DevOps for repository 'mstack-skills', for each commit get all details.";
var resultAzureDevops1 = await kernel.InvokePromptAsync(promptAzureDevops1, new(executionSettings)).ConfigureAwait(false);
Console.WriteLine($"\n\n{promptAzureDevops1}\n{resultAzureDevops1}");

await cts.CancelAsync().ConfigureAwait(false);
cts.Dispose();