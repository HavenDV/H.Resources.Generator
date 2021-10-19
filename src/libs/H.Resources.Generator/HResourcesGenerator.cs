using System.Text;
using Microsoft.CodeAnalysis;
using Microsoft.CodeAnalysis.Text;

namespace H.Resources.Generator;

[Generator]
public class HResourcesGenerator : ISourceGenerator
{
    #region Methods

    public void Execute(GeneratorExecutionContext context)
    {
        try
        {
            context.AddSource(
                "H.Resource",
                SourceText.From(
                    CodeGenerator.GenerateResource(
                        GetGlobalOption(context, "Namespace") ?? "H",
                        GetGlobalOption(context, "Modifier") ?? "internal",
                        bool.Parse(GetGlobalOption(context, "WithSystemDrawing") ?? "false")),
                    Encoding.UTF8));
            context.AddSource(
                "H.Resources",
                SourceText.From(
                    CodeGenerator.GenerateResources(
                        GetGlobalOption(context, "Namespace") ?? "H",
                        GetGlobalOption(context, "Modifier") ?? "internal",
                        GetGlobalOption(context, "ClassName") ?? "Resources",
                        context.AdditionalFiles
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

    public void Initialize(GeneratorInitializationContext context)
    {
    }

    #endregion

    #region Utilities

    private static string? GetGlobalOption(GeneratorExecutionContext context, string name)
    {
        return context.AnalyzerConfigOptions.GlobalOptions.TryGetValue(
            $"build_property.{nameof(HResourcesGenerator)}_{name}",
            out var result) &&
            !string.IsNullOrWhiteSpace(result)
            ? result
            : null;
    }

    //private static string? GetOption(
    //    GeneratorExecutionContext context, 
    //    string name, 
    //    AdditionalText text)
    //{
    //    return context.AnalyzerConfigOptions.GetOptions(text).TryGetValue(
    //        $"build_metadata.AdditionalFiles.{nameof(HResourcesGenerator)}_{name}", 
    //        out var result) &&
    //        !string.IsNullOrWhiteSpace(result)
    //        ? result
    //        : null;
    //}

    #endregion
}
