using Microsoft.Extensions.DependencyInjection;

namespace ArchitectureTests.IntegrationTests;

public class BaseIntegrationTest : IClassFixture<IntegrationTestWebAppFactory>, IAsyncLifetime
{
    protected readonly HttpClient HttpClient;
    protected readonly IServiceScope Scope;

    protected BaseIntegrationTest(IntegrationTestWebAppFactory factory)
    {
        HttpClient = factory.HttpClient;
        Scope = factory.Services.CreateScope();
    }

    public virtual Task InitializeAsync() => Task.CompletedTask;

    public virtual Task DisposeAsync()
    {
        Scope.Dispose();
        return Task.CompletedTask;
    }
}
