namespace H.Generators;

public class Resource
{
    public string Path { get; set; } = string.Empty;

    public Resource(string path)
    {
        Path = path ?? throw new ArgumentNullException(nameof(path));
    }
}
