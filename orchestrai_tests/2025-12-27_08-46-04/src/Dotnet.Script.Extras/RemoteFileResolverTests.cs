using System;
using System.Collections.Generic;
using System.Net;
using System.Net.Http;
using System.Threading;
using System.Threading.Tasks;
using Xunit;
using Moq;
using Dotnet.Script.Extras;

namespace Dotnet.Script.Extras.Tests
{
    public class RemoteFileResolverTests
    {
        private readonly Mock<HttpClient> _mockHttpClient;
        private readonly RemoteFileResolver _resolver;

        public RemoteFileResolverTests()
        {
            _mockHttpClient = new Mock<HttpClient>();
            _resolver = new RemoteFileResolver(_mockHttpClient.Object);
        }

        #region Constructor Tests

        [Fact]
        public void Constructor_WithValidHttpClient_InitializesSuccessfully()
        {
            // Arrange
            var httpClient = new HttpClient();

            // Act
            var resolver = new RemoteFileResolver(httpClient);

            // Assert
            Assert.NotNull(resolver);
        }

        [Fact]
        public void Constructor_WithNullHttpClient_ThrowsArgumentNullException()
        {
            // Arrange
            HttpClient httpClient = null;

            // Act & Assert
            Assert.Throws<ArgumentNullException>(() => new RemoteFileResolver(httpClient));
        }

        #endregion

        #region ResolveAsync Tests - Valid URLs

        [Fact]
        public async Task ResolveAsync_WithValidHttpUrl_ReturnsFileContent()
        {
            // Arrange
            var url = "http://example.com/script.csx";
            var expectedContent = "// C# script content";
            var response = new HttpResponseMessage(HttpStatusCode.OK)
            {
                Content = new StringContent(expectedContent)
            };

            _mockHttpClient
                .Setup(c => c.GetAsync(It.IsAny<string>(), It.IsAny<CancellationToken>()))
                .ReturnsAsync(response);

            // Act
            var result = await _resolver.ResolveAsync(url);

            // Assert
            Assert.Equal(expectedContent, result);
            _mockHttpClient.Verify(c => c.GetAsync(url, It.IsAny<CancellationToken>()), Times.Once);
        }

        [Fact]
        public async Task ResolveAsync_WithValidHttpsUrl_ReturnsFileContent()
        {
            // Arrange
            var url = "https://example.com/script.csx";
            var expectedContent = "// HTTPS C# script content";
            var response = new HttpResponseMessage(HttpStatusCode.OK)
            {
                Content = new StringContent(expectedContent)
            };

            _mockHttpClient
                .Setup(c => c.GetAsync(It.IsAny<string>(), It.IsAny<CancellationToken>()))
                .ReturnsAsync(response);

            // Act
            var result = await _resolver.ResolveAsync(url);

            // Assert
            Assert.Equal(expectedContent, result);
        }

        [Fact]
        public async Task ResolveAsync_WithGitHubRawUrl_ReturnsFileContent()
        {
            // Arrange
            var url = "https://raw.githubusercontent.com/user/repo/main/script.csx";
            var expectedContent = "// GitHub script content";
            var response = new HttpResponseMessage(HttpStatusCode.OK)
            {
                Content = new StringContent(expectedContent)
            };

            _mockHttpClient
                .Setup(c => c.GetAsync(It.IsAny<string>(), It.IsAny<CancellationToken>()))
                .ReturnsAsync(response);

            // Act
            var result = await _resolver.ResolveAsync(url);

            // Assert
            Assert.Equal(expectedContent, result);
        }

        [Fact]
        public async Task ResolveAsync_WithUrlContainingQueryParameters_ReturnsFileContent()
        {
            // Arrange
            var url = "http://example.com/script.csx?version=1.0&format=raw";
            var expectedContent = "// Script with parameters";
            var response = new HttpResponseMessage(HttpStatusCode.OK)
            {
                Content = new StringContent(expectedContent)
            };

            _mockHttpClient
                .Setup(c => c.GetAsync(It.IsAny<string>(), It.IsAny<CancellationToken>()))
                .ReturnsAsync(response);

            // Act
            var result = await _resolver.ResolveAsync(url);

            // Assert
            Assert.Equal(expectedContent, result);
        }

        #endregion

        #region ResolveAsync Tests - Empty/Null URLs

        [Fact]
        public async Task ResolveAsync_WithNullUrl_ThrowsArgumentNullException()
        {
            // Arrange
            string url = null;

            // Act & Assert
            await Assert.ThrowsAsync<ArgumentNullException>(() => _resolver.ResolveAsync(url));
        }

        [Fact]
        public async Task ResolveAsync_WithEmptyUrl_ThrowsArgumentException()
        {
            // Arrange
            var url = string.Empty;

            // Act & Assert
            await Assert.ThrowsAsync<ArgumentException>(() => _resolver.ResolveAsync(url));
        }

        [Fact]
        public async Task ResolveAsync_WithWhitespaceOnlyUrl_ThrowsArgumentException()
        {
            // Arrange
            var url = "   ";

            // Act & Assert
            await Assert.ThrowsAsync<ArgumentException>(() => _resolver.ResolveAsync(url));
        }

        #endregion

        #region ResolveAsync Tests - Invalid URLs

        [Fact]
        public async Task ResolveAsync_WithMalformedUrl_ThrowsUriFormatException()
        {
            // Arrange
            var url = "not a valid url";

            // Act & Assert
            await Assert.ThrowsAsync<UriFormatException>(() => _resolver.ResolveAsync(url));
        }

        [Fact]
        public async Task ResolveAsync_WithFileProtocolUrl_ThrowsInvalidOperationException()
        {
            // Arrange
            var url = "file:///c:/script.csx";

            // Act & Assert
            await Assert.ThrowsAsync<InvalidOperationException>(() => _resolver.ResolveAsync(url));
        }

        [Fact]
        public async Task ResolveAsync_WithFtpProtocolUrl_ThrowsInvalidOperationException()
        {
            // Arrange
            var url = "ftp://example.com/script.csx";

            // Act & Assert
            await Assert.ThrowsAsync<InvalidOperationException>(() => _resolver.ResolveAsync(url));
        }

        [Fact]
        public async Task ResolveAsync_WithCustomProtocolUrl_ThrowsInvalidOperationException()
        {
            // Arrange
            var url = "custom://example.com/script.csx";

            // Act & Assert
            await Assert.ThrowsAsync<InvalidOperationException>(() => _resolver.ResolveAsync(url));
        }

        #endregion

        #region ResolveAsync Tests - HTTP Status Codes

        [Fact]
        public async Task ResolveAsync_WithNotFoundStatus_ThrowsHttpRequestException()
        {
            // Arrange
            var url = "http://example.com/missing.csx";
            var response = new HttpResponseMessage(HttpStatusCode.NotFound);

            _mockHttpClient
                .Setup(c => c.GetAsync(It.IsAny<string>(), It.IsAny<CancellationToken>()))
                .ReturnsAsync(response);

            // Act & Assert
            await Assert.ThrowsAsync<HttpRequestException>(() => _resolver.ResolveAsync(url));
        }

        [Fact]
        public async Task ResolveAsync_WithUnauthorizedStatus_ThrowsHttpRequestException()
        {
            // Arrange
            var url = "http://example.com/secure.csx";
            var response = new HttpResponseMessage(HttpStatusCode.Unauthorized);

            _mockHttpClient
                .Setup(c => c.GetAsync(It.IsAny<string>(), It.IsAny<CancellationToken>()))
                .ReturnsAsync(response);

            // Act & Assert
            await Assert.ThrowsAsync<HttpRequestException>(() => _resolver.ResolveAsync(url));
        }

        [Fact]
        public async Task ResolveAsync_WithForbiddenStatus_ThrowsHttpRequestException()
        {
            // Arrange
            var url = "http://example.com/forbidden.csx";
            var response = new HttpResponseMessage(HttpStatusCode.Forbidden);

            _mockHttpClient
                .Setup(c => c.GetAsync(It.IsAny<string>(), It.IsAny<CancellationToken>()))
                .ReturnsAsync(response);

            // Act & Assert
            await Assert.ThrowsAsync<HttpRequestException>(() => _resolver.ResolveAsync(url));
        }

        [Fact]
        public async Task ResolveAsync_WithServerErrorStatus_ThrowsHttpRequestException()
        {
            // Arrange
            var url = "http://example.com/error.csx";
            var response = new HttpResponseMessage(HttpStatusCode.InternalServerError);

            _mockHttpClient
                .Setup(c => c.GetAsync(It.IsAny<string>(), It.IsAny<CancellationToken>()))
                .ReturnsAsync(response);

            // Act & Assert
            await Assert.ThrowsAsync<HttpRequestException>(() => _resolver.ResolveAsync(url));
        }

        [Fact]
        public async Task ResolveAsync_WithBadGatewayStatus_ThrowsHttpRequestException()
        {
            // Arrange
            var url = "http://example.com/gateway.csx";
            var response = new HttpResponseMessage(HttpStatusCode.BadGateway);

            _mockHttpClient
                .Setup(c => c.GetAsync(It.IsAny<string>(), It.IsAny<CancellationToken>()))
                .ReturnsAsync(response);

            // Act & Assert
            await Assert.ThrowsAsync<HttpRequestException>(() => _resolver.ResolveAsync(url));
        }

        #endregion

        #region ResolveAsync Tests - Network Errors

        [Fact]
        public async Task ResolveAsync_WhenHttpClientThrowsException_PropagatesException()
        {
            // Arrange
            var url = "http://example.com/script.csx";
            _mockHttpClient
                .Setup(c => c.GetAsync(It.IsAny<string>(), It.IsAny<CancellationToken>()))
                .ThrowsAsync(new HttpRequestException("Network error"));

            // Act & Assert
            await Assert.ThrowsAsync<HttpRequestException>(() => _resolver.ResolveAsync(url));
        }

        [Fact]
        public async Task ResolveAsync_WhenTimeoutOccurs_ThrowsTaskCanceledException()
        {
            // Arrange
            var url = "http://example.com/script.csx";
            _mockHttpClient
                .Setup(c => c.GetAsync(It.IsAny<string>(), It.IsAny<CancellationToken>()))
                .ThrowsAsync(new TaskCanceledException());

            // Act & Assert
            await Assert.ThrowsAsync<TaskCanceledException>(() => _resolver.ResolveAsync(url));
        }

        [Fact]
        public async Task ResolveAsync_WhenOperationCancelled_ThrowsOperationCanceledException()
        {
            // Arrange
            var url = "http://example.com/script.csx";
            var cts = new CancellationTokenSource();
            cts.Cancel();

            _mockHttpClient
                .Setup(c => c.GetAsync(It.IsAny<string>(), It.IsAny<CancellationToken>()))
                .ThrowsAsync(new OperationCanceledException());

            // Act & Assert
            await Assert.ThrowsAsync<OperationCanceledException>(() => _resolver.ResolveAsync(url, cts.Token));
        }

        #endregion

        #region ResolveAsync Tests - Content Types

        [Fact]
        public async Task ResolveAsync_WithPlainTextContent_ReturnsContent()
        {
            // Arrange
            var url = "http://example.com/script.csx";
            var expectedContent = "var x = 5;";
            var response = new HttpResponseMessage(HttpStatusCode.OK)
            {
                Content = new StringContent(expectedContent, System.Text.Encoding.UTF8, "text/plain")
            };

            _mockHttpClient
                .Setup(c => c.GetAsync(It.IsAny<string>(), It.IsAny<CancellationToken>()))
                .ReturnsAsync(response);

            // Act
            var result = await _resolver.ResolveAsync(url);

            // Assert
            Assert.Equal(expectedContent, result);
        }

        [Fact]
        public async Task ResolveAsync_WithApplicationOctetStreamContent_ReturnsContent()
        {
            // Arrange
            var url = "http://example.com/script";
            var expectedContent = "Console.WriteLine(\"test\");";
            var response = new HttpResponseMessage(HttpStatusCode.OK)
            {
                Content = new StringContent(expectedContent, System.Text.Encoding.UTF8, "application/octet-stream")
            };

            _mockHttpClient
                .Setup(c => c.GetAsync(It.IsAny<string>(), It.IsAny<CancellationToken>()))
                .ReturnsAsync(response);

            // Act
            var result = await _resolver.ResolveAsync(url);

            // Assert
            Assert.Equal(expectedContent, result);
        }

        #endregion

        #region ResolveAsync Tests - Edge Cases

        [Fact]
        public async Task ResolveAsync_WithVeryLongUrl_ReturnsContent()
        {
            // Arrange
            var url = "http://example.com/" + new string('a', 2000);
            var expectedContent = "// Long URL script";
            var response = new HttpResponseMessage(HttpStatusCode.OK)
            {
                Content = new StringContent(expectedContent)
            };

            _mockHttpClient
                .Setup(c => c.GetAsync(It.IsAny<string>(), It.IsAny<CancellationToken>()))
                .ReturnsAsync(response);

            // Act
            var result = await _resolver.ResolveAsync(url);

            // Assert
            Assert.Equal(expectedContent, result);
        }

        [Fact]
        public async Task ResolveAsync_WithVeryLargeContent_ReturnsAllContent()
        {
            // Arrange
            var url = "http://example.com/large.csx";
            var expectedContent = new string('x', 10_000_000); // 10MB
            var response = new HttpResponseMessage(HttpStatusCode.OK)
            {
                Content = new StringContent(expectedContent)
            };

            _mockHttpClient
                .Setup(c => c.GetAsync(It.IsAny<string>(), It.IsAny<CancellationToken>()))
                .ReturnsAsync(response);

            // Act
            var result = await _resolver.ResolveAsync(url);

            // Assert
            Assert.Equal(expectedContent, result);
            Assert.Equal(10_000_000, result.Length);
        }

        [Fact]
        public async Task ResolveAsync_WithEmptyContentResponse_ReturnsEmptyString()
        {
            // Arrange
            var url = "http://example.com/empty.csx";
            var response = new HttpResponseMessage(HttpStatusCode.OK)
            {
                Content = new StringContent(string.Empty)
            };

            _mockHttpClient
                .Setup(c => c.GetAsync(It.IsAny<string>(), It.IsAny<CancellationToken>()))
                .ReturnsAsync(response);

            // Act
            var result = await _resolver.ResolveAsync(url);

            // Assert
            Assert.Empty(result);
        }

        [Fact]
        public async Task ResolveAsync_WithSpecialCharactersInContent_ReturnsContent()
        {
            // Arrange
            var url = "http://example.com/special.csx";
            var expectedContent = "// Special chars: !@#$%^&*(){}[]|\\:;\"'<>,.?/";
            var response = new HttpResponseMessage(HttpStatusCode.OK)
            {
                Content = new StringContent(expectedContent)
            };

            _mockHttpClient
                .Setup(c => c.GetAsync(It.IsAny<string>(), It.IsAny<CancellationToken>()))
                .ReturnsAsync(response);

            // Act
            var result = await _resolver.ResolveAsync(url);

            // Assert
            Assert.Equal(expectedContent, result);
        }

        [Fact]
        public async Task ResolveAsync_WithUnicodeContent_ReturnsContent()
        {
            // Arrange
            var url = "http://example.com/unicode.csx";
            var expectedContent = "// Unicode: 你好世界 مرحبا بالعالم Здравствуй мир";
            var response = new HttpResponseMessage(HttpStatusCode.OK)
            {
                Content = new StringContent(expectedContent, System.Text.Encoding.UTF8)
            };

            _mockHttpClient
                .Setup(c => c.GetAsync(It.IsAny<string>(), It.IsAny<CancellationToken>()))
                .ReturnsAsync(response);

            // Act
            var result = await _resolver.ResolveAsync(url);

            // Assert
            Assert.Equal(expectedContent, result);
        }

        [Fact]
        public async Task ResolveAsync_WithUrlHavingFragment_ReturnsContent()
        {
            // Arrange
            var url = "http://example.com/script.csx#section";
            var expectedContent = "// Script with fragment";
            var response = new HttpResponseMessage(HttpStatusCode.OK)
            {
                Content = new StringContent(expectedContent)
            };

            _mockHttpClient
                .Setup(c => c.GetAsync(It.IsAny<string>(), It.IsAny<CancellationToken>()))
                .ReturnsAsync(response);

            // Act
            var result = await _resolver.ResolveAsync(url);

            // Assert
            Assert.Equal(expectedContent, result);
        }

        #endregion

        #region ResolveAsync Tests - Cancellation Token

        [Fact]
        public async Task ResolveAsync_WithValidCancellationToken_CompletesSuccessfully()
        {
            // Arrange
            var url = "http://example.com/script.csx";
            var expectedContent = "// Script with token";
            var response = new HttpResponseMessage(HttpStatusCode.OK)
            {
                Content = new StringContent(expectedContent)
            };
            var cts = new CancellationTokenSource();

            _mockHttpClient
                .Setup(c => c.GetAsync(It.IsAny<string>(), It.IsAny<CancellationToken>()))
                .ReturnsAsync(response);

            // Act
            var result = await _resolver.ResolveAsync(url, cts.Token);

            // Assert
            Assert.Equal(expectedContent, result);
        }

        [Fact]
        public async Task ResolveAsync_WithDefaultCancellationToken_CompletesSuccessfully()
        {
            // Arrange
            var url = "http://example.com/script.csx";
            var expectedContent = "// Script with default token";
            var response = new HttpResponseMessage(HttpStatusCode.OK)
            {
                Content = new StringContent(expectedContent)
            };

            _mockHttpClient
                .Setup(c => c.GetAsync(It.IsAny<string>(), It.IsAny<CancellationToken>()))
                .ReturnsAsync(response);

            // Act
            var result = await _resolver.ResolveAsync(url, CancellationToken.None);

            // Assert
            Assert.Equal(expectedContent, result);
        }

        #endregion

        #region Multiple Consecutive Calls Tests

        [Fact]
        public async Task ResolveAsync_WithMultipleConsecutiveCalls_AllCompletesSuccessfully()
        {
            // Arrange
            var urls = new[] 
            { 
                "http://example.com/script1.csx",
                "http://example.com/script2.csx",
                "http://example.com/script3.csx"
            };
            var contents = new[] 
            { 
                "// Script 1",
                "// Script 2",
                "// Script 3"
            };

            var queue = new Queue<string>(contents);
            _mockHttpClient
                .Setup(c => c.GetAsync(It.IsAny<string>(), It.IsAny<CancellationToken>()))
                .Returns(() => 
                {
                    var content = queue.Dequeue();
                    var response = new HttpResponseMessage(HttpStatusCode.OK)
                    {
                        Content = new StringContent(content)
                    };
                    return Task.FromResult(response);
                });

            // Act
            var results = new List<string>();
            foreach (var url in urls)
            {
                results.Add(await _resolver.ResolveAsync(url));
            }

            // Assert
            Assert.Equal(3, results.Count);
            for (int i = 0; i < results.Count; i++)
            {
                Assert.Equal(contents[i], results[i]);
            }
        }

        [Fact]
        public async Task ResolveAsync_WithParallelCalls_AllCompletesSuccessfully()
        {
            // Arrange
            var urls = new[] 
            { 
                "http://example.com/script1.csx",
                "http://example.com/script2.csx",
                "http://example.com/script3.csx"
            };
            var contents = new[] 
            { 
                "// Script 1",
                "// Script 2",
                "// Script 3"
            };

            var urlContentMap = new Dictionary<string, string>();
            for (int i = 0; i < urls.Length; i++)
            {
                urlContentMap[urls[i]] = contents[i];
            }

            _mockHttpClient
                .Setup(c => c.GetAsync(It.IsAny<string>(), It.IsAny<CancellationToken>()))
                .Returns((string url, CancellationToken ct) => 
                {
                    var response = new HttpResponseMessage(HttpStatusCode.OK)
                    {
                        Content = new StringContent(urlContentMap[url])
                    };
                    return Task.FromResult(response);
                });

            // Act
            var tasks = new List<Task<string>>();
            foreach (var url in urls)
            {
                tasks.Add(_resolver.ResolveAsync(url));
            }
            var results = await Task.WhenAll(tasks);

            // Assert
            Assert.Equal(3, results.Length);
            for (int i = 0; i < results.Length; i++)
            {
                Assert.Equal(contents[i], results[i]);
            }
        }

        #endregion

        #region Dispose Tests

        [Fact]
        public void Dispose_WhenImplemented_ReleasesResources()
        {
            // Arrange
            var resolver = _resolver as IDisposable;

            // Act & Assert
            if (resolver != null)
            {
                resolver.Dispose(); // Should not throw
            }
        }

        [Fact]
        public void Dispose_CalledMultipleTimes_DoesNotThrow()
        {
            // Arrange
            var resolver = _resolver as IDisposable;

            // Act & Assert
            if (resolver != null)
            {
                resolver.Dispose();
                resolver.Dispose(); // Should not throw on second call
            }
        }

        #endregion

        #region TryResolveAsync Tests (if method exists)

        [Fact]
        public async Task TryResolveAsync_WithValidUrl_ReturnsTrue()
        {
            // Arrange
            var url = "http://example.com/script.csx";
            var expectedContent = "// Try resolve content";
            var response = new HttpResponseMessage(HttpStatusCode.OK)
            {
                Content = new StringContent(expectedContent)
            };

            _mockHttpClient
                .Setup(c => c.GetAsync(It.IsAny<string>(), It.IsAny<CancellationToken>()))
                .ReturnsAsync(response);

            // Act
            var result = await _resolver.TryResolveAsync(url, out var content);

            // Assert
            Assert.True(result);
            Assert.Equal(expectedContent, content);
        }

        [Fact]
        public async Task TryResolveAsync_WithInvalidUrl_ReturnsFalse()
        {
            // Arrange
            var url = "not a valid url";

            // Act
            var result = await _resolver.TryResolveAsync(url, out var content);

            // Assert
            Assert.False(result);
            Assert.Null(content);
        }

        [Fact]
        public async Task TryResolveAsync_With404Status_ReturnsFalse()
        {
            // Arrange
            var url = "http://example.com/missing.csx";
            var response = new HttpResponseMessage(HttpStatusCode.NotFound);

            _mockHttpClient
                .Setup(c => c.GetAsync(It.IsAny<string>(), It.IsAny<CancellationToken>()))
                .ReturnsAsync(response);

            // Act
            var result = await _resolver.TryResolveAsync(url, out var content);

            // Assert
            Assert.False(result);
        }

        [Fact]
        public async Task TryResolveAsync_WithNullUrl_ReturnsFalse()
        {
            // Arrange
            string url = null;

            // Act
            var result = await _resolver.TryResolveAsync(url, out var content);

            // Assert
            Assert.False(result);
        }

        #endregion

        #region Helper Methods Tests

        [Fact]
        public void IsValidUrl_WithValidHttpUrl_ReturnsTrue()
        {
            // Act
            var result = RemoteFileResolver.IsValidUrl("http://example.com/script.csx");

            // Assert
            Assert.True(result);
        }

        [Fact]
        public void IsValidUrl_WithValidHttpsUrl_ReturnsTrue()
        {
            // Act
            var result = RemoteFileResolver.IsValidUrl("https://example.com/script.csx");

            // Assert
            Assert.True(result);
        }

        [Fact]
        public void IsValidUrl_WithInvalidUrl_ReturnsFalse()
        {
            // Act
            var result = RemoteFileResolver.IsValidUrl("not a url");

            // Assert
            Assert.False(result);
        }

        [Fact]
        public void IsValidUrl_WithNullUrl_ReturnsFalse()
        {
            // Act
            var result = RemoteFileResolver.IsValidUrl(null);

            // Assert
            Assert.False(result);
        }

        [Fact]
        public void IsValidUrl_WithEmptyUrl_ReturnsFalse()
        {
            // Act
            var result = RemoteFileResolver.IsValidUrl(string.Empty);

            // Assert
            Assert.False(result);
        }

        [Fact]
        public void IsValidUrl_WithFileProtocol_ReturnsFalse()
        {
            // Act
            var result = RemoteFileResolver.IsValidUrl("file:///c:/script.csx");

            // Assert
            Assert.False(result);
        }

        #endregion

        #region Error Recovery Tests

        [Fact]
        public async Task ResolveAsync_AfterFailure_CanSucceedOnNextAttempt()
        {
            // Arrange
            var url = "http://example.com/script.csx";
            var expectedContent = "// Recovered content";
            var callCount = 0;

            _mockHttpClient
                .Setup(c => c.GetAsync(It.IsAny<string>(), It.IsAny<CancellationToken>()))
                .Returns(() =>
                {
                    callCount++;
                    if (callCount == 1)
                    {
                        return Task.FromResult(new HttpResponseMessage(HttpStatusCode.InternalServerError));
                    }
                    return Task.FromResult(new HttpResponseMessage(HttpStatusCode.OK)
                    {
                        Content = new StringContent(expectedContent)
                    });
                });

            // Act - First attempt should fail
            await Assert.ThrowsAsync<HttpRequestException>(() => _resolver.ResolveAsync(url));

            // Reset the mock for the second attempt with proper setup
            _mockHttpClient.Reset();
            _mockHttpClient
                .Setup(c => c.GetAsync(It.IsAny<string>(), It.IsAny<CancellationToken>()))
                .ReturnsAsync(new HttpResponseMessage(HttpStatusCode.OK)
                {
                    Content = new StringContent(expectedContent)
                });

            // Act - Second attempt should succeed
            var result = await _resolver.ResolveAsync(url);

            // Assert
            Assert.Equal(expectedContent, result);
        }

        #endregion
    }
}