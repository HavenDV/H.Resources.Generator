using System.Collections.Immutable;
using H.Generators;
using H.Generators.Tests.Extensions;
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
        var globalOptions = new Dictionary<string, string>();
        
        var additionalTextOptions = new Dictionary<string, Dictionary<string, string>>();
        foreach (var additionalText in additionalTexts)
        {
            var options = new Dictionary<string, string>();
            options.TryAdd("build_metadata.AdditionalFiles.HResourcesGenerator_Resource", "true");
            additionalTextOptions.Add(additionalText.Path, options);
        }

        var referenceAssemblies = ReferenceAssemblies.Net.Net60;
        var references = await referenceAssemblies.ResolveAsync(null, cancellationToken);
        var compilation = (Compilation)CSharpCompilation.Create(
            assemblyName: "Tests",
            references: references,
            options: new CSharpCompilationOptions(OutputKind.DynamicallyLinkedLibrary));
        var driver = CSharpGeneratorDriver
            .Create(new HResourcesGenerator())
            .AddAdditionalTexts([..additionalTexts])
            .WithUpdatedAnalyzerConfigOptions(new DictionaryAnalyzerConfigOptionsProvider(globalOptions, additionalTextOptions: additionalTextOptions))
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