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
                            (TryGetOption(context, nameof(Resource.Type), value, out var result) ? result : null) ?? 
                            CodeGenerator.GetTypeByExtension(Path.GetExtension(value.Path)),
                    })
                    .ToArray();

                var code = CodeGenerator.Generate("H", "internal", "Resources", resources);
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

        private static bool TryGetGlobalOption(GeneratorExecutionContext context, string name, out string? result)
        {
            return context.AnalyzerConfigOptions.GlobalOptions.TryGetValue($"build_property.{name}", out result);
        }

        private static bool TryGetOption(GeneratorExecutionContext context, string name, AdditionalText obj, out string? result)
        {
            return context.AnalyzerConfigOptions.GetOptions(obj).TryGetValue($"build_property.{name}", out result);
        }

        #endregion
    }
}