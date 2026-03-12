namespace AdvanceRequests.IntegrationTests.Infrastructure;

[CollectionDefinition("IntegrationTests")]
public sealed class IntegrationTestCollection : ICollectionFixture<IntegrationTestWebAppFactory>
{
}