namespace H.Generators;

public static class CodeGenerator
{
    #region Methods

    public static string GenerateResource(
        string @namespace,
        string modifier,
        bool withSystemDrawing)
    {
        return @$"
using System;
using System.IO;
using System.Linq;
using System.Reflection;

#nullable enable

namespace {@namespace}
{{
    {modifier} class Resource
    {{
        public string FileName {{ get; set; }}

        public Resource(string fileName)
        {{
            FileName = fileName ?? throw new ArgumentNullException(nameof(fileName));
        }}

        /// <summary>
        /// Searches for a file among Embedded resources <br/>
        /// Throws an <see cref=""ArgumentException""/> if nothing is found or more than one match is found <br/>
        /// </summary>
        /// <param name=""assembly""></param>
        /// <exception cref=""ArgumentNullException""></exception>
        /// <exception cref=""ArgumentException""></exception>
        /// <returns></returns>
        public System.IO.Stream AsStream(Assembly? assembly = null)
        {{
            assembly ??= Assembly.GetExecutingAssembly();

            try
            {{
                return assembly.GetManifestResourceStream(
                           assembly
                               .GetManifestResourceNames()
                               .Single(resourceName => resourceName.EndsWith($"".{{FileName}}"", StringComparison.InvariantCultureIgnoreCase)))
                       ?? throw new ArgumentException($""\""{{FileName}}\"" is not found in embedded resources"");
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
        /// </summary>
        /// <param name=""assembly""></param>
        /// <exception cref=""ArgumentNullException""></exception>
        /// <exception cref=""ArgumentException""></exception>
        /// <returns></returns>
        public byte[] AsBytes(Assembly? assembly = null)
        {{
            using var stream = AsStream(assembly);
            using var memoryStream = new MemoryStream();

            stream.CopyTo(memoryStream);

            return memoryStream.ToArray();
        }}

        /// <summary>
        /// Searches for a file among Embedded resources <br/>
        /// Throws an <see cref=""ArgumentException""/> if nothing is found or more than one match is found <br/>
        /// </summary>
        /// <param name=""assembly""></param>
        /// <exception cref=""ArgumentNullException""></exception>
        /// <exception cref=""ArgumentException""></exception>
        /// <returns></returns>
        public string AsString(Assembly? assembly = null)
        {{
            using var stream = AsStream(assembly);
            using var reader = new StreamReader(stream);

            return reader.ReadToEnd();
        }}
{(withSystemDrawing ? @"
        public System.Drawing.Image AsBitmap(Assembly? assembly = null)
        {
            using var stream = AsStream(assembly);

            return System.Drawing.Image.FromStream(stream);
        }
" : "")}
    }}
}}
";
    }

    public static string GenerateResources(
        string @namespace,
        string modifier,
        string className,
        IReadOnlyCollection<Resource> resources)
    {
        var properties = resources
            .Select(static resource => (
                name: SanitizeName(Path.GetFileName(resource.Path)),
                fileName: Path.GetFileName(resource.Path)
            ))
            .ToArray();

        return @$"
#nullable enable

namespace {@namespace}
{{
    {modifier} static class {className}
    {{
{
string.Join(Environment.NewLine, properties.Select(static resource =>
$"        public static Resource {resource.name} => new Resource(\"{resource.fileName}\");"))
}
    }}
}}
";
    }
    
    internal static string SanitizeName(string? name)
    {
        static bool InvalidFirstChar(char ch)
            => ch is not ('_' or >= 'A' and <= 'Z' or >= 'a' and <= 'z');

        static bool InvalidSubsequentChar(char ch)
            => ch is not (
                '_'
                or >= 'A' and <= 'Z'
                or >= 'a' and <= 'z'
                or >= '0' and <= '9'
                );
        
        if (name is null || name.Length == 0)
        {
            return "";
        }

        if (InvalidFirstChar(name[0]))
        {
            name = "_" + name;
        }

        if (!name.Skip(1).Any(InvalidSubsequentChar))
        {
            return name;
        }

        Span<char> buf = stackalloc char[name.Length];
        name.AsSpan().CopyTo(buf);
        
        for (var i = 1; i < buf.Length; i++)
        {
            if (InvalidSubsequentChar(buf[i]))
            {
                buf[i] = '_';
            }
        }

        // Span<char>.ToString implementation checks for char type, new string(&buf[0], buf.length)
        return buf.ToString();
    }

    #endregion
}
