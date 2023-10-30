using System.Text.Json.Serialization;
using Swashbuckle.AspNetCore.Annotations;

namespace Librarian.Api.Models.Definitions;

[JsonDerivedType(typeof(Phrase), WordClass.Phrase)]
[JsonDerivedType(typeof(Noun), WordClass.Noun)]
[JsonDerivedType(typeof(Verb), WordClass.Verb)]
[SwaggerDiscriminator("$type")]
[SwaggerSubType(typeof(Noun), DiscriminatorValue = WordClass.Noun)]
[SwaggerSubType(typeof(Verb), DiscriminatorValue = WordClass.Verb)]
[SwaggerSubType(typeof(Phrase), DiscriminatorValue = WordClass.Phrase)]
public abstract class Definition
{
    protected Definition(string lemma)
    {
        Lemma = lemma;
    }

    public string Lemma { get; }
}
