using System.Collections.Immutable;
using System.Text;
using Microsoft.CodeAnalysis;
using Microsoft.CodeAnalysis.Diagnostics;
using Microsoft.CodeAnalysis.Text;

namespace H.Resources.Generator;

[Generator]
public class HResourcesGenerator : IIncrementalGenerator
{
    #region Methods

    public void Initialize(IncrementalGeneratorInitializationContext context)
    {
        context.RegisterSourceOutput(
            context.CompilationProvider
                .Combine(context.AnalyzerConfigOptionsProvider)
                .Combine(context.AdditionalTextsProvider.Collect()),
            (context, tuple) => Execute(tuple.Left.Right, tuple.Right, context));
    }

    private static void Execute(
        AnalyzerConfigOptionsProvider optionsProvider,
        ImmutableArray<AdditionalText> texts,
        SourceProductionContext context)
    {
        try
        {
            context.AddSource(
                "H.Resource",
                SourceText.From(
                    CodeGenerator.GenerateResource(
                        GetGlobalOption(optionsProvider, "Namespace") ?? "H",
                        GetGlobalOption(optionsProvider, "Modifier") ?? "internal",
                        bool.Parse(GetGlobalOption(optionsProvider, "WithSystemDrawing") ?? "false")),
                    Encoding.UTF8));
            context.AddSource(
                "H.Resources",
                SourceText.From(
                    CodeGenerator.GenerateResources(
                        GetGlobalOption(optionsProvider, "Namespace") ?? "H",
                        GetGlobalOption(optionsProvider, "Modifier") ?? "internal",
                        GetGlobalOption(optionsProvider, "ClassName") ?? "Resources",
                        texts
                            .Select(value => new Resource(value.Path))
                            .ToArray()),
                    Encoding.UTF8));
        }
        catch (Exception exception)
        {
            context.ReportDiagnostic(
                Diagnostic.Create(
                    new DiagnosticDescriptor(
                        "HRG0001",
                        "Exception: ",
                        $"{exception}",
                        "Usage",
                        DiagnosticSeverity.Error,
                        true),
                    Location.None));
        }
    }

    #endregion

    #region Utilities

    private static string? GetGlobalOption(
        AnalyzerConfigOptionsProvider optionsProvider,
        string name)
    {
        return optionsProvider.GlobalOptions.TryGetValue(
            $"build_property.{nameof(HResourcesGenerator)}_{name}",
            out var result) &&
            !string.IsNullOrWhiteSpace(result)
            ? result
            : null;
    }

    //private static string? GetOption(
    //    AnalyzerConfigOptionsProvider optionsProvider, 
    //    string name, 
    //    AdditionalText text)
    //{
    //    return optionsProvider.GetOptions(text).TryGetValue(
    //        $"build_metadata.AdditionalFiles.{nameof(HResourcesGenerator)}_{name}", 
    //        out var result) &&
    //        !string.IsNullOrWhiteSpace(result)
    //        ? result
    //        : null;
    //}

    #endregion
}
