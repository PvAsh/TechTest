using Moq;
using Moq.Protected;
using System.Net;
using TestTechApp.Services;

namespace TestTechApp.Tests;

public class DummyLoginServiceTests
{
    [Fact]
    public async Task GetRandomUserAsync_Returns_ValidUser()
    {
        var usersJson = @"{
            ""users"": [
                { ""username"": ""user1"", ""password"": ""pass1"" },
                { ""username"": ""user2"", ""password"": ""pass2"" }
            ]
        }";

        var handlerMock = new Mock<HttpMessageHandler>();
        handlerMock.Protected()
            .Setup<Task<HttpResponseMessage>>(
                "SendAsync",
                ItExpr.IsAny<HttpRequestMessage>(),
                ItExpr.IsAny<CancellationToken>()
            )
            .ReturnsAsync(new HttpResponseMessage
            {
                StatusCode = HttpStatusCode.OK,
                Content = new StringContent(usersJson)
            });

        var httpClient = new HttpClient(handlerMock.Object);
        var service = new DummyLoginService(httpClient);

        // Act
        var (username, password) = await service.GetRandomUserAsync();

        // Assert
        Assert.Contains(username, new[] { "user1", "user2" });
        Assert.Contains(password, new[] { "pass1", "pass2" });
    }
}