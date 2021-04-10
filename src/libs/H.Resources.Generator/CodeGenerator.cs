using System;
using System.IO;
using System.Linq;

namespace H.Resources.Generator
{
    public static class CodeGenerator
    {
        #region Methods

        public static string Generate(string @namespace, string modifier, string className, string[] paths)
        {
            var resources = paths
                .Select(static path => new Resource
                {
                    Name = Path.GetFileNameWithoutExtension(path),
                    Type = "System.Drawing.Image",
                    Method = "GetBitmap",
                    FileName = Path.GetFileName(path),
                })
                .ToArray();


            return @$"
using System;
using System.IO;
using System.Linq;
using System.Reflection;

#nullable enable

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
        {{
            name = name ?? throw new ArgumentNullException(nameof(name));

            using var stream = ReadFileAsStream(name, assembly);
            using var memoryStream = new MemoryStream();

            stream.CopyTo(memoryStream);

            return memoryStream.ToArray();
        }}

        private static System.Drawing.Image GetBitmap(string name)
        {{
            using var stream = ReadFileAsStream(name);

            return System.Drawing.Image.FromStream(stream);
        }}

{
string.Join(Environment.NewLine, resources.Select(static resource =>
$"        public static {resource.Type} {resource.Name} => {resource.Method}(\"{resource.FileName}\");"))
}
    }}
}}
";
        }

        #endregion
    }
}