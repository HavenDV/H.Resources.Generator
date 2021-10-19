using Microsoft.CodeAnalysis;
using Microsoft.CodeAnalysis.CSharp;
using System.Reflection;

namespace H.Resources.Generator.UnitTests;

[TestClass]
public class SourceGeneratorTests
{
    [TestMethod]
    public void CompileTest()
    {
        var inputCompilation = CSharpCompilation.Create(
            "compilation",
            new[] { CSharpSyntaxTree.ParseText(@"
namespace MyCode
{
    public class Program
    {
        public static void Main(string[] args)
        {
        }
    }
}
") },
            new[]
            {
                    MetadataReference.CreateFromFile(typeof(Binder).GetTypeInfo().Assembly.Location),
            },
            new CSharpCompilationOptions(OutputKind.ConsoleApplication));

        var generator = new HResourcesGenerator();
        var driver = (GeneratorDriver)CSharpGeneratorDriver.Create(generator);

        driver.RunGeneratorsAndUpdateCompilation(
            inputCompilation,
            out var outputCompilation,
            out var diagnostics);

        diagnostics.Should().BeEmpty();
        outputCompilation.SyntaxTrees.Should().HaveCount(3);

        foreach (var tree in outputCompilation.SyntaxTrees)
        {
            Console.WriteLine($@"
{tree.FilePath}
{tree.GetText()}");
        }

        foreach (var diagnostic in outputCompilation.GetDiagnostics())
        {
            Console.WriteLine($"{diagnostic}");
        }
    }
}
