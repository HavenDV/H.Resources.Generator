using FluentAssertions;
using Microsoft.VisualStudio.TestTools.UnitTesting;

namespace H.Resources.Generator.UnitTests
{
    [TestClass]
    public class CodeGeneratorTests
    {
        [TestMethod]
        public void GenerateTest()
        {
            var code = CodeGenerator.Generate("H", "internal", "Resources", new[]
            {
                new Resource
                {
                    Path = "path1.png", 
                    Type = ResourceType.Image,
                },
                new Resource
                {
                    Path = "path with whitespaces.png",
                    Type = ResourceType.Image,
                },
                new Resource
                {
                    Path = "path3.png",
                    Type = ResourceType.Stream,
                },
                new Resource
                {
                    Path = "path4.txt",
                    Type = ResourceType.String,
                },
            });

            code.Should().Be(@"
using System;
using System.IO;
using System.Linq;
using System.Reflection;

#nullable enable

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
        private static System.IO.Stream ReadFileAsStream(string name, Assembly? assembly = null)
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

        /// <summary>
        /// Searches for a file among Embedded resources <br/>
        /// Throws an <see cref=""ArgumentException""/> if nothing is found or more than one match is found <br/>
        /// <![CDATA[Version: 1.0.0.2]]> <br/>
        /// <![CDATA[Dependency: ReadFileAsStream(string name, Assembly? assembly = null)]]> <br/>
        /// </summary>
        /// <param name=""name""></param>
        /// <param name=""assembly""></param>
        /// <exception cref=""ArgumentNullException""></exception>
        /// <exception cref=""ArgumentException""></exception>
        /// <returns></returns>
        public static byte[] ReadFileAsBytes(string name, Assembly? assembly = null)
        {
            name = name ?? throw new ArgumentNullException(nameof(name));

            using var stream = ReadFileAsStream(name, assembly);
            using var memoryStream = new MemoryStream();

            stream.CopyTo(memoryStream);

            return memoryStream.ToArray();
        }

        /// <summary>
        /// Searches for a file among Embedded resources <br/>
        /// Throws an <see cref=""ArgumentException""/> if nothing is found or more than one match is found <br/>
        /// <![CDATA[Version: 1.0.0.2]]> <br/>
        /// <![CDATA[Dependency: ReadFileAsStream(string name, Assembly? assembly = null)]]> <br/>
        /// </summary>
        /// <param name=""name""></param>
        /// <param name=""assembly""></param>
        /// <exception cref=""ArgumentNullException""></exception>
        /// <exception cref=""ArgumentException""></exception>
        /// <returns></returns>
        public static string ReadFileAsString(string name, Assembly? assembly = null)
        {
            name = name ?? throw new ArgumentNullException(nameof(name));

            using var stream = ReadFileAsStream(name, assembly);
            using var reader = new StreamReader(stream);

            return reader.ReadToEnd();
        }

        private static System.Drawing.Image GetBitmap(string name)
        {
            using var stream = ReadFileAsStream(name);

            return System.Drawing.Image.FromStream(stream);
        }

        public static System.Drawing.Image path1_png => GetBitmap(""path1.png"");
        public static System.Drawing.Image path_with_whitespaces_png => GetBitmap(""path with whitespaces.png"");
        public static System.IO.Stream path3_png => ReadFileAsStream(""path3.png"");
        public static string path4_txt => ReadFileAsString(""path4.txt"");
    }
}
");
        }
    }
}
