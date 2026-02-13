namespace DataverseToolbox.Models;

public class DataverseConnection
{
    public string Id { get; set; } = Guid.NewGuid().ToString();

    public string Name { get; set; } = string.Empty;

    public string Url { get; set; } = string.Empty;

    public string OrgId { get; set; } = string.Empty;

    public DateTime LastUsed { get; set; } = DateTime.UtcNow;
}
