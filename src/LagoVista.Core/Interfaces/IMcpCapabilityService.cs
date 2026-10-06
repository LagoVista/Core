using LagoVista.Core.Models;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace LagoVista.Core.Interfaces
{
    /// <summary>
    /// Generic MCP transport/capability boundary. Process orchestration policy remains
    /// outside this service; the MCP server remains authoritative for final validation.
    /// </summary>
    public interface IMcpCapabilityService
    {
        Task RegisterAsync(McpServerConnection connection);
        Task<McpServerCapabilitySnapshot> TestConnectionAsync(string serverId);
        Task<McpServerCapabilitySnapshot> DiscoverAsync(string serverId);
        Task<IReadOnlyCollection<McpServerCapabilitySnapshot>> GetRegisteredServersAsync();
        Task<McpToolInvocationResult> InvokeAsync(McpToolInvocationRequest request);
    }
}
