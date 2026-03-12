using System.Net;
using System.Net.Http.Json;
using FluentAssertions;
using AdvanceRequests.IntegrationTests.Infrastructure;

namespace AdvanceRequests.IntegrationTests.Api;

[Collection("IntegrationTests")]
public sealed class AdvanceRequestsIntegrationTests
{
    private readonly HttpClient _client;

    public AdvanceRequestsIntegrationTests(IntegrationTestWebAppFactory factory)
    {
        _client = factory.CreateClient();
    }

    [Fact]
    public async Task Create_Should_ReturnSuccess_WhenPayloadIsValid()
    {
        // Arrange
        var payload = new
        {
            creatorId = Guid.NewGuid(),
            grossAmount = 1000m
        };

        // Act
        var response = await _client.PostAsJsonAsync("/api/v1/advance-requests", payload);

        // Assert
        response.StatusCode.Should().Be(HttpStatusCode.OK);

        var body = await response.Content.ReadAsStringAsync();
        body.Should().Contain("1000");
        body.Should().Contain("50");
        body.Should().Contain("950");
    }

    [Fact]
    public async Task Create_Should_ReturnBadRequest_WhenCreatorAlreadyHasPendingRequest()
    {
        // Arrange
        var creatorId = Guid.NewGuid();

        var firstPayload = new
        {
            creatorId,
            grossAmount = 1000m
        };

        var secondPayload = new
        {
            creatorId,
            grossAmount = 500m
        };

        // Act
        var firstResponse = await _client.PostAsJsonAsync("/api/v1/advance-requests", firstPayload);
        var secondResponse = await _client.PostAsJsonAsync("/api/v1/advance-requests", secondPayload);

        // Assert
        firstResponse.StatusCode.Should().Be(HttpStatusCode.OK);
        secondResponse.StatusCode.Should().Be(HttpStatusCode.BadRequest);

        var errorBody = await secondResponse.Content.ReadAsStringAsync();
        errorBody.Should().Contain("already has a pending advance request");
    }

    [Fact]
    public async Task Simulate_Should_ReturnExpectedValues_WhenAmountIsValid()
    {
        // Act
        var response = await _client.GetAsync("/api/v1/advance-requests/simulate?amount=1000");

        // Assert
        response.StatusCode.Should().Be(HttpStatusCode.OK);

        var body = await response.Content.ReadAsStringAsync();
        body.Should().Contain("1000");
        body.Should().Contain("50");
        body.Should().Contain("950");
    }

    [Fact]
    public async Task Create_Should_ReturnBadRequest_WhenCreatorIdIsEmpty()
    {
        var payload = new
        {
            creatorId = Guid.Empty,
            grossAmount = 1000m
        };

        var response = await _client.PostAsJsonAsync("/api/v1/advance-requests", payload);

        response.StatusCode.Should().Be(HttpStatusCode.BadRequest);
    }

    [Fact]
    public async Task Create_Should_ReturnBadRequest_WhenGrossAmountIsLessThanOrEqualToZero()
    {
        var payload = new
        {
            creatorId = Guid.NewGuid(),
            grossAmount = 0m
        };

        var response = await _client.PostAsJsonAsync("/api/v1/advance-requests", payload);

        response.StatusCode.Should().Be(HttpStatusCode.BadRequest);
    }

    [Fact]
    public async Task Simulate_Should_ReturnBadRequest_WhenAmountIsLessThanOrEqualToZero()
    {
        var response = await _client.GetAsync("/api/v1/advance-requests/simulate?amount=0");

        response.StatusCode.Should().Be(HttpStatusCode.BadRequest);
    }
}