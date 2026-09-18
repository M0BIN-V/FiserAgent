using System.ComponentModel;
using System.Linq.Expressions;
using System.Reflection;
using DiServiceInstaller;
using Microsoft.Agents.AI;
using Microsoft.Extensions.AI;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using Runtime.Application.Common.Abstractions;
using Runtime.Application.Common.Services.Mcp;

namespace Runtime.Application.Installers;

public class AgentFactoryInstaller : IServiceInstaller
{
    public void Install(IHostApplicationBuilder builder)
    {
        builder.Services.AddSingleton<McpTransportFactory>();

        builder.Services.AddSingleton<IMcpToolProvider, McpToolProvider>();

        builder.Services.AddScoped<AgentFactory>();
    }
}

public class AgentFactory(IChatClient chatClient, IEnumerable<ITool> toolTypes, IMcpToolProvider mcpTools)
{
    public async Task<ChatClientAgent> CreateAsync(CancellationToken ct = default)
    {
        var systemPrompt = await File.ReadAllTextAsync(Path.Combine(AppContext.BaseDirectory, "system.md"), ct);
        var tools = GetToolFunctions()
            .ToList();

        tools.AddRange(await mcpTools.GetToolsAsync(ct));

        var options = new ChatClientAgentOptions
        {
            Name = "fiser",
            ChatOptions = new ChatOptions
            {
                Instructions = systemPrompt,
                Tools = [..tools]
            }
        };

        var agent = chatClient.AsAIAgent(options);

        return agent;
    }

    private static Delegate CreateDelegate(object target, MethodInfo method)
    {
        var parameterTypes = method
            .GetParameters()
            .Select(p => p.ParameterType)
            .Append(method.ReturnType)
            .ToArray();

        var delegateType = Expression.GetDelegateType(parameterTypes);

        return method.CreateDelegate(delegateType, target);
    }

    public IEnumerable<AIFunction> GetToolFunctions()
    {
        foreach (var tool in toolTypes)
        foreach (var method in tool.GetType().GetMethods(BindingFlags.Instance | BindingFlags.Public))
        {
            if (method.IsSpecialName || method.DeclaringType == typeof(object)) continue;

            var @delegate = CreateDelegate(tool, method);

            yield return AIFunctionFactory.Create(@delegate, method.Name,
                method.GetCustomAttribute<DescriptionAttribute>()?.Description);
        }
    }
}