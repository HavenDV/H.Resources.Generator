using System;
using System.IO;
using System.Linq;
using System.Text;
using Microsoft.CodeAnalysis;
using Microsoft.CodeAnalysis.Text;

namespace H.Resources.Generator
{
    [Generator]
    public class ResourcesGenerator : ISourceGenerator
    {
        #region Methods

        public void Execute(GeneratorExecutionContext context)
        {
            try
            {
                const string @namespace = "H";
                const string modifier = "internal";
                const string className = "Resources";
                var resources = context.AdditionalFiles
                    .Select(static value => new Resource
                    {
                        Name = Path.GetFileNameWithoutExtension(value.Path),
                        Type = "System.Drawing.Image",
                        Method = "GetBitmap",
                        FileName = Path.GetFileName(value.Path),
                    })
                    .ToArray();
                

                context.AddSource("NSwag Generated CSharp Code", SourceText.From(@$"
namespace {@namespace}
{{
    {modifier} static class {className}
    {{

        /// <summary>
        /// Searches for a file among Embedded resources <br/>
        /// Throws an <see cref=""ArgumentException""/> if nothing is found or more than one match is found <br/>
        /// <![CDATA[Version: 1.0.0.4]]> <br/>
        /// </summary>
        /// <param name=""name""></param>
        /// <param name=""assembly""></param>
        /// <exception cref=""ArgumentNullException""></exception>
        /// <exception cref=""ArgumentException""></exception>
        /// <returns></returns>
        private static Stream ReadFileAsStream(string name, Assembly? assembly = null)
        {{
            name = name ?? throw new ArgumentNullException(nameof(name));
            assembly ??= Assembly.GetExecutingAssembly();

            try
            {{
                return assembly.GetManifestResourceStream(
                           assembly
                               .GetManifestResourceNames()
                               .Single(resourceName => resourceName.EndsWith(name, StringComparison.InvariantCultureIgnoreCase)))
                       ?? throw new ArgumentException($""\""{{name}}\"" is not found in embedded resources"");
            }}
            catch (InvalidOperationException exception)
            {{
                throw new ArgumentException(
                    ""Not a single one was found or more than one resource with the given name was found. "" +
                    ""Make sure there are no collisions and the required file has the attribute \""Embedded resource\"""", 
                    exception);
            }}
        }}

        private static System.Drawing.Image GetBitmap(string name)
        {{
            using var stream = ReadFileAsStream(name);

            return System.Drawing.Image.FromStream(stream);
        }}

{
string.Join(Environment.NewLine, resources.Select(static resource => 
$"        {modifier} static {resource.Type} {resource.Name} => {resource.Method}(\"{resource.FileName}\");"))
}
    }}
}}
", Encoding.UTF8));
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