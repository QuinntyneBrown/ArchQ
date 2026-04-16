namespace ArchQ.Core.Entities;

public class TemplateConfig
{
    public string Type { get; set; } = "config";
    public string Id { get; set; } = Guid.NewGuid().ToString();
    public string Key { get; set; } = "adr_template";
    public string Body { get; set; } = string.Empty;
    public List<string> RequiredSections { get; set; } = new();
    public int ApprovalThreshold { get; set; }
    public DateTime UpdatedAt { get; set; } = DateTime.UtcNow;
    public string UpdatedBy { get; set; } = string.Empty;
}
