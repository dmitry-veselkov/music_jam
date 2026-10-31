namespace Tests.FunctionalTests.Fixtures;

[CollectionDefinition(Name)]
public sealed class TestCollection :
    ICollectionFixture<TestApplicationFixture>
{
    public const string Name = "Studio Functional Tests";
}
