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
        context.RegisterSourceOutput(
            context.CompilationProvider
                .Combine(context.AnalyzerConfigOptionsProvider)
                .Combine(context.AdditionalTextsProvider.Collect()),
            static (context, tuple) => Execute(tuple.Left.Right, tuple.Right, context));
    }

    private static void Execute(
        AnalyzerConfigOptionsProvider options,
        ImmutableArray<AdditionalText> texts,
        SourceProductionContext context)
    {
        try
        {
            var @namespace = options.GetGlobalOption($"{Name}_Namespace") ?? "H";
            var modifier = options.GetGlobalOption($"{Name}_Modifier") ?? "internal";
            var withSystemDrawing = bool.Parse(options.GetGlobalOption($"{Name}_WithSystemDrawing") ?? "false");
            var className = options.GetGlobalOption($"{Name}_ClassName") ?? "Resources";

            context.AddTextSource(
                hintName: "H.Resource",
                text: CodeGenerator.GenerateResource(
                    @namespace: @namespace,
                    modifier: modifier,
                    withSystemDrawing: withSystemDrawing));
            context.AddTextSource(
                hintName: "H.Resources",
                text: CodeGenerator.GenerateResources(
                    @namespace: @namespace,
                    modifier: modifier,
                    className: className,
                    resources: texts
                        .Select(value => new Resource(value.Path))
                        .ToArray()));
        }
        catch (Exception exception)
        {
            context.ReportException($"{Id}0001", exception);
        }
    }

    #endregion
}
