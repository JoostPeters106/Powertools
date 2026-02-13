using System.Text.Json;
using DataverseToolbox.Models;

namespace DataverseToolbox.Services;

public class ConnectionManager
{
    private static readonly string ConnectionsFile = Path.Combine(AppContext.BaseDirectory, "connections.json");

    public List<DataverseConnection> LoadConnections()
    {
        if (!File.Exists(ConnectionsFile))
        {
            return new List<DataverseConnection>();
        }

        try
        {
            var json = File.ReadAllText(ConnectionsFile);
            return JsonSerializer.Deserialize<List<DataverseConnection>>(json) ?? new List<DataverseConnection>();
        }
        catch
        {
            return new List<DataverseConnection>();
        }
    }

    public void SaveConnection(string url, string orgName, string orgId)
    {
        var connections = LoadConnections();
        var existing = connections.FirstOrDefault(c => c.OrgId == orgId || c.Url.Equals(url, StringComparison.OrdinalIgnoreCase));

        if (existing is null)
        {
            connections.Add(new DataverseConnection
            {
                Name = orgName,
                Url = url,
                OrgId = orgId,
                LastUsed = DateTime.UtcNow
            });
        }
        else
        {
            existing.Name = orgName;
            existing.Url = url;
            existing.OrgId = orgId;
            existing.LastUsed = DateTime.UtcNow;
        }

        var json = JsonSerializer.Serialize(connections.OrderByDescending(c => c.LastUsed), new JsonSerializerOptions
        {
            WriteIndented = true
        });

        File.WriteAllText(ConnectionsFile, json);
    }
}
