using System;
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
                var paths = context.AdditionalFiles
                    .Select(static value => value.Path)
                    .ToArray();

                var code = CodeGenerator.Generate("H", "internal", "Resources", paths);
                context.AddSource("H.Resources.Generator Generated CSharp Code", SourceText.From(code, Encoding.UTF8));
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

        private static string GetGlobalOption(GeneratorExecutionContext context, string name, string? defaultValue = null)
        {
            return context.AnalyzerConfigOptions.GlobalOptions.TryGetValue($"build_property.{name}", out var result) &&
                   !string.IsNullOrWhiteSpace(result)
                ? result
                : defaultValue ?? string.Empty;
        }

        #endregion
    }
}