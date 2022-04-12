using System.Collections.Immutable;
using H.Generators;
using Microsoft.CodeAnalysis;
using Microsoft.CodeAnalysis.CSharp;

namespace H.Ipc.Generator.IntegrationTests;

public static class TestHelper
{
    public static async Task CheckSourceAsync(
        this VerifyBase verifier,
        AdditionalText[] additionalTexts,
        CancellationToken cancellationToken = default)
    {
        var dotNetFolder = Path.GetDirectoryName(typeof(object).Assembly.Location) ?? string.Empty;
        var compilation = (Compilation)CSharpCompilation.Create(
            assemblyName: "Tests",
            references: new[]
            {
                MetadataReference.CreateFromFile(typeof(object).Assembly.Location),
                MetadataReference.CreateFromFile(Path.Combine(dotNetFolder, "System.Runtime.dll")),
                MetadataReference.CreateFromFile(Path.Combine(dotNetFolder, "System.Linq.dll")),
                MetadataReference.CreateFromFile(Path.Combine(dotNetFolder, "netstandard.dll")),
            },
            options: new CSharpCompilationOptions(OutputKind.DynamicallyLinkedLibrary));
        var driver = CSharpGeneratorDriver
            .Create(new HResourcesGenerator())
            .AddAdditionalTexts(ImmutableArray.Create(additionalTexts))
            .RunGeneratorsAndUpdateCompilation(compilation, out compilation, out _, cancellationToken);
        var diagnostics = compilation.GetDiagnostics(cancellationToken);

        await Task.WhenAll(
            verifier
                .Verify(diagnostics)
                .UseDirectory("Snapshots")
                .UseTextForParameters("Diagnostics"),
            verifier
                .Verify(driver)
                .UseDirectory("Snapshots"));
    }
}