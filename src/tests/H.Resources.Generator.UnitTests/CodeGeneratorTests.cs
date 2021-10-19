namespace H.Resources.Generator.UnitTests;

[TestClass]
public class CodeGeneratorTests
{
    [TestMethod]
    public void GenerateTest()
    {
        var code = CodeGenerator.GenerateResources("H", "internal", "Resources", new[]
        {
                new Resource("path1.png"),
                new Resource("path with whitespaces.png"),
            });

        code.Should().Be(@"
#nullable enable

namespace H
{
    internal static class Resources
    {
        public static Resource path1_png => new Resource(""path1.png"");
        public static Resource path_with_whitespaces_png => new Resource(""path with whitespaces.png"");
    }
}
");
    }
}
