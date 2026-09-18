using Microsoft.Agents.AI;

namespace Runtime.Application.Models;

internal static class Store
{
    public static AgentSession? Session { get; set; }
}