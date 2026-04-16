namespace ArchQ.Application.Adrs.Queries.GetTemplate;

public class TemplateResponse
{
    public string Body { get; set; } = string.Empty;
    public List<string> RequiredSections { get; set; } = new();
    public bool IsCustom { get; set; }
    public DateTime? UpdatedAt { get; set; }
}
