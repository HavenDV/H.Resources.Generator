namespace H.Ipc.Generator.IntegrationTests;

[TestClass]
public class HResourcesGeneratorIntegrationTests
{
    [TestMethod]
    public void GeneratesHelloWorldAsStringCorrectly()
    {
        H.Resources.helloworld_txt.AsString().Should().Be("Hello, World!");
    }
}