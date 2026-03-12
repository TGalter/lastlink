using System.Net;
using System.Net.Http.Headers;
using System.Net.Http.Json;
using AdvanceRequests.Infrastructure.Persistence;
using AdvanceRequests.IntegrationTests.Infrastructure;
using FluentAssertions;
using Microsoft.Extensions.DependencyInjection;

namespace AdvanceRequests.IntegrationTests.Api;

[Collection("IntegrationTests")]
public sealed class AdvanceRequestsIntegrationTests
{
    private readonly HttpClient _client;
    private readonly IntegrationTestWebAppFactory _factory;

    public AdvanceRequestsIntegrationTests(IntegrationTestWebAppFactory factory)
    {
        _factory = factory;
        _client = factory.CreateClient();
    }

    [Fact]
    public async Task Simulate_Should_ReturnExpectedValues_WhenAmountIsValid()
    {
        var response = await _client.GetAsync("/api/v1/advance-requests/simulate?amount=1000");

        response.StatusCode.Should().Be(HttpStatusCode.OK);

        var body = await response.Content.ReadAsStringAsync();
        body.Should().Contain("1000");
        body.Should().Contain("50");
        body.Should().Contain("950");
    }

    [Fact]
    public async Task Simulate_Should_ReturnBadRequest_WhenAmountIsLessThanOrEqualToZero()
    {
        var response = await _client.GetAsync("/api/v1/advance-requests/simulate?amount=0");

        response.StatusCode.Should().Be(HttpStatusCode.BadRequest);

        var body = await response.Content.ReadAsStringAsync();
        body.Should().Contain("Falha na validação");
    }

    [Fact]
    public async Task Create_Should_ReturnUnauthorized_WhenTokenIsNotProvided()
    {
        await ResetDatabaseAsync();

        var payload = new
        {
            grossAmount = 1000m
        };

        var response = await _client.PostAsJsonAsync("/api/v1/advance-requests", payload);

        response.StatusCode.Should().Be(HttpStatusCode.Unauthorized);
    }

    [Fact]
    public async Task Create_Should_ReturnSuccess_WhenCreatorIsAuthenticated()
    {
        await ResetDatabaseAsync();

        var creatorId = Guid.NewGuid();
        var token = await LoginAsCreatorAsync(creatorId);

        var payload = new
        {
            grossAmount = 1000m
        };

        var request = new HttpRequestMessage(HttpMethod.Post, "/api/v1/advance-requests")
        {
            Content = JsonContent.Create(payload)
        };

        request.Headers.Authorization = new AuthenticationHeaderValue("Bearer", token);

        var response = await _client.SendAsync(request);

        response.StatusCode.Should().Be(HttpStatusCode.OK);

        var body = await response.Content.ReadAsStringAsync();
        body.Should().Contain("1000");
        body.Should().Contain("50");
        body.Should().Contain("950");
        body.Should().Contain(creatorId.ToString());
    }

    [Fact]
    public async Task Create_Should_ReturnBadRequest_WhenGrossAmountIsLessThanOrEqualToZero()
    {
        await ResetDatabaseAsync();

        var token = await LoginAsCreatorAsync(Guid.NewGuid());

        var payload = new
        {
            grossAmount = 0m
        };

        var request = new HttpRequestMessage(HttpMethod.Post, "/api/v1/advance-requests")
        {
            Content = JsonContent.Create(payload)
        };

        request.Headers.Authorization = new AuthenticationHeaderValue("Bearer", token);

        var response = await _client.SendAsync(request);

        response.StatusCode.Should().Be(HttpStatusCode.BadRequest);

        var body = await response.Content.ReadAsStringAsync();
        body.Should().Contain("Falha na validação");
    }

    [Fact]
    public async Task Create_Should_ReturnBadRequest_WhenCreatorAlreadyHasPendingRequest()
    {
        await ResetDatabaseAsync();

        var creatorId = Guid.NewGuid();
        var token = await LoginAsCreatorAsync(creatorId);

        var firstPayload = new
        {
            grossAmount = 1000m
        };

        var secondPayload = new
        {
            grossAmount = 500m
        };

        var firstRequest = new HttpRequestMessage(HttpMethod.Post, "/api/v1/advance-requests")
        {
            Content = JsonContent.Create(firstPayload)
        };
        firstRequest.Headers.Authorization = new AuthenticationHeaderValue("Bearer", token);

        var secondRequest = new HttpRequestMessage(HttpMethod.Post, "/api/v1/advance-requests")
        {
            Content = JsonContent.Create(secondPayload)
        };
        secondRequest.Headers.Authorization = new AuthenticationHeaderValue("Bearer", token);

        var firstResponse = await _client.SendAsync(firstRequest);
        var secondResponse = await _client.SendAsync(secondRequest);

        firstResponse.StatusCode.Should().Be(HttpStatusCode.OK);
        secondResponse.StatusCode.Should().Be(HttpStatusCode.BadRequest);

        var errorBody = await secondResponse.Content.ReadAsStringAsync();
        errorBody.Should().Contain("já possui uma solicitação de adiantamento pendente");
    }

    [Fact]
    public async Task List_Should_ReturnOnlyOwnRequests_WhenUserIsCreator()
    {
        await ResetDatabaseAsync();

        var creator1Id = Guid.NewGuid();
        var creator2Id = Guid.NewGuid();

        var creator1Token = await LoginAsCreatorAsync(creator1Id);
        var creator2Token = await LoginAsCreatorAsync(creator2Id);

        var createRequest1 = new HttpRequestMessage(HttpMethod.Post, "/api/v1/advance-requests")
        {
            Content = JsonContent.Create(new { grossAmount = 1000m })
        };
        createRequest1.Headers.Authorization = new AuthenticationHeaderValue("Bearer", creator1Token);

        var createRequest2 = new HttpRequestMessage(HttpMethod.Post, "/api/v1/advance-requests")
        {
            Content = JsonContent.Create(new { grossAmount = 1200m })
        };
        createRequest2.Headers.Authorization = new AuthenticationHeaderValue("Bearer", creator2Token);

        var createResponse1 = await _client.SendAsync(createRequest1);
        var createResponse2 = await _client.SendAsync(createRequest2);

        createResponse1.StatusCode.Should().Be(HttpStatusCode.OK);
        createResponse2.StatusCode.Should().Be(HttpStatusCode.OK);

        var listRequest = new HttpRequestMessage(HttpMethod.Get, "/api/v1/advance-requests");
        listRequest.Headers.Authorization = new AuthenticationHeaderValue("Bearer", creator1Token);

        var response = await _client.SendAsync(listRequest);

        response.StatusCode.Should().Be(HttpStatusCode.OK);

        var body = await response.Content.ReadAsStringAsync();
        body.Should().Contain(creator1Id.ToString());
        body.Should().NotContain(creator2Id.ToString());
    }

    [Fact]
    public async Task List_Should_ReturnAllRequests_WhenUserIsAdmin()
    {
        await ResetDatabaseAsync();

        var creator1Id = Guid.NewGuid();
        var creator2Id = Guid.NewGuid();

        var creator1Token = await LoginAsCreatorAsync(creator1Id);
        var creator2Token = await LoginAsCreatorAsync(creator2Id);
        var adminToken = await LoginAsAdminAsync();

        var createRequest1 = new HttpRequestMessage(HttpMethod.Post, "/api/v1/advance-requests")
        {
            Content = JsonContent.Create(new { grossAmount = 1000m })
        };
        createRequest1.Headers.Authorization = new AuthenticationHeaderValue("Bearer", creator1Token);

        var createRequest2 = new HttpRequestMessage(HttpMethod.Post, "/api/v1/advance-requests")
        {
            Content = JsonContent.Create(new { grossAmount = 1200m })
        };
        createRequest2.Headers.Authorization = new AuthenticationHeaderValue("Bearer", creator2Token);

        var createResponse1 = await _client.SendAsync(createRequest1);
        var createResponse2 = await _client.SendAsync(createRequest2);

        createResponse1.StatusCode.Should().Be(HttpStatusCode.OK);
        createResponse2.StatusCode.Should().Be(HttpStatusCode.OK);

        var listRequest = new HttpRequestMessage(HttpMethod.Get, "/api/v1/advance-requests");
        listRequest.Headers.Authorization = new AuthenticationHeaderValue("Bearer", adminToken);

        var response = await _client.SendAsync(listRequest);

        response.StatusCode.Should().Be(HttpStatusCode.OK);

        var body = await response.Content.ReadAsStringAsync();
        body.Should().Contain(creator1Id.ToString());
        body.Should().Contain(creator2Id.ToString());
    }

    [Fact]
    public async Task Approve_Should_ReturnForbidden_WhenUserIsCreator()
    {
        await ResetDatabaseAsync();

        var creatorId = Guid.NewGuid();
        var creatorToken = await LoginAsCreatorAsync(creatorId);

        var createRequest = new HttpRequestMessage(HttpMethod.Post, "/api/v1/advance-requests")
        {
            Content = JsonContent.Create(new { grossAmount = 1000m })
        };
        createRequest.Headers.Authorization = new AuthenticationHeaderValue("Bearer", creatorToken);

        var createResponse = await _client.SendAsync(createRequest);
        createResponse.StatusCode.Should().Be(HttpStatusCode.OK);

        var created = await createResponse.Content.ReadFromJsonAsync<AdvanceRequestCreatedTestResponse>();
        created.Should().NotBeNull();

        var approveRequest = new HttpRequestMessage(
            HttpMethod.Post,
            $"/api/v1/advance-requests/{created!.Id}/approve");

        approveRequest.Headers.Authorization = new AuthenticationHeaderValue("Bearer", creatorToken);

        var approveResponse = await _client.SendAsync(approveRequest);

        approveResponse.StatusCode.Should().Be(HttpStatusCode.Forbidden);
    }

    [Fact]
    public async Task Approve_Should_ReturnNoContent_WhenUserIsAdmin()
    {
        await ResetDatabaseAsync();

        var creatorToken = await LoginAsCreatorAsync(Guid.NewGuid());
        var adminToken = await LoginAsAdminAsync();

        var createRequest = new HttpRequestMessage(HttpMethod.Post, "/api/v1/advance-requests")
        {
            Content = JsonContent.Create(new { grossAmount = 1000m })
        };
        createRequest.Headers.Authorization = new AuthenticationHeaderValue("Bearer", creatorToken);

        var createResponse = await _client.SendAsync(createRequest);
        createResponse.StatusCode.Should().Be(HttpStatusCode.OK);

        var created = await createResponse.Content.ReadFromJsonAsync<AdvanceRequestCreatedTestResponse>();
        created.Should().NotBeNull();

        var approveRequest = new HttpRequestMessage(
            HttpMethod.Post,
            $"/api/v1/advance-requests/{created!.Id}/approve");

        approveRequest.Headers.Authorization = new AuthenticationHeaderValue("Bearer", adminToken);

        var approveResponse = await _client.SendAsync(approveRequest);

        approveResponse.StatusCode.Should().Be(HttpStatusCode.NoContent);
    }

    private async Task ResetDatabaseAsync()
    {
        using var scope = _factory.Services.CreateScope();
        var dbContext = scope.ServiceProvider.GetRequiredService<ApplicationDbContext>();

        dbContext.OutboxMessages.RemoveRange(dbContext.OutboxMessages);
        dbContext.AdvanceRequestStatusHistory.RemoveRange(dbContext.AdvanceRequestStatusHistory);
        dbContext.AdvanceRequests.RemoveRange(dbContext.AdvanceRequests);

        await dbContext.SaveChangesAsync();
    }

    private async Task<string> LoginAsAdminAsync()
    {
        var payload = new
        {
            role = "Admin"
        };

        var response = await _client.PostAsJsonAsync("/api/v1/auth/login", payload);
        response.EnsureSuccessStatusCode();

        var result = await response.Content.ReadFromJsonAsync<LoginResponseTestModel>();
        result.Should().NotBeNull();

        return result!.AccessToken;
    }

    private async Task<string> LoginAsCreatorAsync(Guid creatorId)
    {
        var payload = new
        {
            role = "Creator",
            creatorId
        };

        var response = await _client.PostAsJsonAsync("/api/v1/auth/login", payload);
        response.EnsureSuccessStatusCode();

        var result = await response.Content.ReadFromJsonAsync<LoginResponseTestModel>();
        result.Should().NotBeNull();

        return result!.AccessToken;
    }

    private sealed class LoginResponseTestModel
    {
        public string AccessToken { get; init; } = string.Empty;
        public DateTime ExpiresAtUtc { get; init; }
    }

    private sealed class AdvanceRequestCreatedTestResponse
    {
        public Guid Id { get; init; }
    }
}