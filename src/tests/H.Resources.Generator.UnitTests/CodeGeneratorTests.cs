using Microsoft.VisualStudio.TestTools.UnitTesting;

namespace H.Resources.Generator.UnitTests
{
    [TestClass]
    public class CodeGeneratorTests
    {
        [TestMethod]
        public void GenerateTest()
        {
            var code = CodeGenerator.Generate("H", "internal", "Resources", new []{ "path1.png" });

            Assert.AreEqual(@"
namespace H
{
    internal static class Resources
    {

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
        {
            name = name ?? throw new ArgumentNullException(nameof(name));
            assembly ??= Assembly.GetExecutingAssembly();

            try
            {
                return assembly.GetManifestResourceStream(
                           assembly
                               .GetManifestResourceNames()
                               .Single(resourceName => resourceName.EndsWith(name, StringComparison.InvariantCultureIgnoreCase)))
                       ?? throw new ArgumentException($""\""{name}\"" is not found in embedded resources"");
            }
            catch (InvalidOperationException exception)
            {
                throw new ArgumentException(
                    ""Not a single one was found or more than one resource with the given name was found. "" +
                    ""Make sure there are no collisions and the required file has the attribute \""Embedded resource\"""", 
                    exception);
            }
        }

        private static System.Drawing.Image GetBitmap(string name)
        {
            using var stream = ReadFileAsStream(name);

            return System.Drawing.Image.FromStream(stream);
        }

        internal static System.Drawing.Image path1 => GetBitmap(""path1.png"");
    }
}
", code);
        }
    }
}
