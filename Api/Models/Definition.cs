using System.Text.Json.Serialization;
using Swashbuckle.AspNetCore.Annotations;

namespace Librarian.Api.Models;

[JsonDerivedType(typeof(UnknownDefinition), WordClass.Unknown)]
[JsonDerivedType(typeof(NounDefinition), WordClass.Noun)]
[JsonDerivedType(typeof(VerbDefinition), WordClass.Verb)]
[SwaggerDiscriminator("$type")]
[SwaggerSubType(typeof(NounDefinition), DiscriminatorValue = WordClass.Noun)]
[SwaggerSubType(typeof(VerbDefinition), DiscriminatorValue = WordClass.Verb)]
[SwaggerSubType(typeof(UnknownDefinition), DiscriminatorValue = WordClass.Unknown)]
public abstract class Definition
{
    protected Definition(int articleId, string lemma)
    {
        ArticleId = articleId;
        Lemma = lemma;
    }

    public int ArticleId { get; }

    public string Lemma { get; }
}
