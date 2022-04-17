using System.Collections.Immutable;
using H.Generators;
using Microsoft.CodeAnalysis;
using Microsoft.CodeAnalysis.CSharp;
using Microsoft.CodeAnalysis.Testing;

namespace H.Ipc.Generator.IntegrationTests;

public static class TestHelper
{
    public static async Task CheckSourceAsync(
        this VerifyBase verifier,
        AdditionalText[] additionalTexts,
        CancellationToken cancellationToken = default)
    {
        var referenceAssemblies = ReferenceAssemblies.Net.Net60;
        var references = await referenceAssemblies.ResolveAsync(null, cancellationToken);
        var compilation = (Compilation)CSharpCompilation.Create(
            assemblyName: "Tests",
            references: references,
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