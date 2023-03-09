using System.Collections.Immutable;
using H.Generators.Extensions;
using Microsoft.CodeAnalysis;
using Microsoft.CodeAnalysis.Diagnostics;

namespace H.Generators;

[Generator]
public class HResourcesGenerator : IIncrementalGenerator
{
    #region Constants

    public const string Name = nameof(HResourcesGenerator);
    public const string Id = "HRG";

    #endregion

    #region Methods

    public void Initialize(IncrementalGeneratorInitializationContext context)
    {
        context.AdditionalTextsProvider
            .Select(static (x, _) => new Resource(x.Path))
            .Collect()
            .Combine(context.AnalyzerConfigOptionsProvider)
            .SelectAndReportExceptions(GetSourceCode, context, Id)
            .AddSource(context);
    }

    private static EquatableArray<FileWithName> GetSourceCode(
        (ImmutableArray<Resource> Resources, AnalyzerConfigOptionsProvider Options) tuple,
        CancellationToken cancellationToken = default)
    {
        var (resources, options) = tuple;
        
        var @namespace = options.GetGlobalOption("Namespace", prefix: Name) ?? "H";
        var modifier = options.GetGlobalOption("Modifier", prefix: Name) ?? "internal";
        var withSystemDrawing = bool.Parse(options.GetGlobalOption("WithSystemDrawing", prefix: Name) ?? "false");
        var className = options.GetGlobalOption("ClassName", prefix: Name) ?? "Resources";

        return new[]
        {
            new FileWithName(
                Name: "H.Resource.generated.cs",
                Text: CodeGenerator.GenerateResource(
                    @namespace: @namespace,
                    modifier: modifier,
                    withSystemDrawing: withSystemDrawing)),
            new FileWithName(
                Name: "H.Resources.generated.cs",
                Text: CodeGenerator.GenerateResources(
                    @namespace: @namespace,
                    modifier: modifier,
                    className: className,
                    resources: resources)),
        }.ToImmutableArray().AsEquatableArray();
    }

    #endregion
}
