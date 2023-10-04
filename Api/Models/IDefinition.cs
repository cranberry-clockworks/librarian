using System.Text.Json.Serialization;

namespace Librarian.Api.Models;

[JsonDerivedType(typeof(UnknownDefinition))]
[JsonDerivedType(typeof(NounDefinition))]
[JsonDerivedType(typeof(VerbDefinition))]
public interface IDefinition
{
    public int ArticleId { get; }

    public string Lemma { get; }

    public string Class { get; }
}
