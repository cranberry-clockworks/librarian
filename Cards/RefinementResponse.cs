using System.Text.Json.Serialization;

namespace Librarian.Cards;

public class RefinementResponse
{
    [JsonPropertyName("id")] 
    public required int Id { get; init; }
    
    [JsonPropertyName("no")] 
    public required string PhraseNorwegian { get; init; }
    
    [JsonPropertyName("en")] 
    public required string PhraseEnglish { get; init; }
    
    [JsonPropertyName("example-no")] 
    public required string ExampleNorwegian { get; init; }
    
    [JsonPropertyName("example-en")] 
    public required string ExampleEnglish { get; init; }
}