﻿using System.Collections.Immutable;
using H.Generators;
using Microsoft.CodeAnalysis;
using Microsoft.CodeAnalysis.CSharp;

namespace H.Ipc.Generator.IntegrationTests;

public static class TestHelper
{
    public static async Task CheckSource(
        this VerifyBase verifier,
        params AdditionalText[] texts)
    {
        var dotNetFolder = Path.GetDirectoryName(typeof(object).Assembly.Location) ?? string.Empty;
        var compilation = (Compilation)CSharpCompilation.Create(
            assemblyName: "Tests",
            syntaxTrees: new[]
            {
                CSharpSyntaxTree.ParseText(@"
namespace MyCode
{
    public class Program
    {
        public static void Main(string[] args)
        {
        }
    }
}
"),
            },
            references: new[]
            {
                MetadataReference.CreateFromFile(typeof(object).Assembly.Location),
                MetadataReference.CreateFromFile(Path.Combine(dotNetFolder, "System.Runtime.dll")),
                MetadataReference.CreateFromFile(Path.Combine(dotNetFolder, "System.Linq.dll")),
                MetadataReference.CreateFromFile(Path.Combine(dotNetFolder, "netstandard.dll")),
            });
        var generator = new HResourcesGenerator();
        var driver = CSharpGeneratorDriver
            .Create(generator)
            .AddAdditionalTexts(ImmutableArray.Create(texts));
        driver = driver.RunGenerators(compilation);
        
        driver = driver.RunGeneratorsAndUpdateCompilation(
            compilation,
            out compilation,
            out _);
        var diagnostics = compilation.GetDiagnostics();

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