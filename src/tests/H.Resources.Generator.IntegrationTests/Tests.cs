namespace H.Ipc.Generator.IntegrationTests;

[TestClass]
public class Tests
{
    [TestMethod]
    public void GeneratesHelloWorldAsStringCorrectly()
    {
        H.Resources.hello_world_txt.AsString().Should().Be("Hello, World!");
        H.Resources.hello_world_txt.FileName.Should().Be("hello-world.txt");
    }
}