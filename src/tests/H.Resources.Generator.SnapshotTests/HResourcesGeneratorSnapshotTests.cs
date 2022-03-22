namespace H.Ipc.Generator.IntegrationTests;

[TestClass]
public class HResourcesGeneratorSnapshotTests : VerifyBase
{
    [TestMethod]
    public Task GeneratesEmptyResourcesCorrectly()
    {
        return this.CheckSource();
    }

    //[TestMethod]
    //public Task GeneratesPngCorrectly()
    //{
    //    return this.CheckSource(
    //        new CustomAdditionalText("path1.png", "PNG"),
    //        new CustomAdditionalText("path with whitespaces.png", "PNG"));
    //}
}