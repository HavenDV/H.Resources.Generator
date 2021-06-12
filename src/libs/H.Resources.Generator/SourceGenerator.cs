using System;
using System.IO;
using System.Linq;
using System.Text;
using Microsoft.CodeAnalysis;
using Microsoft.CodeAnalysis.Text;

namespace H.Resources.Generator
{
    [Generator]
    public class SourceGenerator : ISourceGenerator
    {
        #region Methods

        public void Execute(GeneratorExecutionContext context)
        {
            try
            {
                var resources = context.AdditionalFiles
                    .Select(value => new Resource
                    {
                        Path = value.Path,
                        Type = 
                            GetOption(context, nameof(Resource.Type), value) ?? 
                            CodeGenerator.GetTypeByExtension(Path.GetExtension(value.Path)),
                    })
                    .ToArray();

                var code = CodeGenerator.Generate(
                    GetGlobalOption(context, "Namespace") ?? "H",
                    GetGlobalOption(context, "Modifier") ?? "internal",
                    GetGlobalOption(context, "ClassName") ?? "Resources", 
                    resources);

                context.AddSource(
                    "H.Resources", 
                    SourceText.From(code, Encoding.UTF8));
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
                $"build_property.HResourcesGenerator_{name}", 
                out var result) &&
                !string.IsNullOrWhiteSpace(result)
                ? result
                : null;
        }

        private static string? GetOption(
            GeneratorExecutionContext context, 
            string name, 
            AdditionalText text)
        {
            return context.AnalyzerConfigOptions.GetOptions(text).TryGetValue(
                $"build_metadata.AdditionalFiles.HResourcesGenerator_{name}", 
                out var result) &&
                !string.IsNullOrWhiteSpace(result)
                ? result
                : null;
        }

        #endregion
    }
}