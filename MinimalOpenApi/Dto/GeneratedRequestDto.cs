using System.Text.Json.Nodes;

namespace MinimalOpenApi.Dto;

public class GeneratedRequestDto
{
    public string Model { get; set; } = string.Empty;
    public string Prompt { get; set; } = string.Empty;
    public bool Stream { get; set; }
    public JsonObject? Format { get; set; }
    public string? Suffix { get; set; }
    public IEnumerable<string>? Images { get; set; }
}