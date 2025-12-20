using System;
using System.IO;
using System.IO.Compression;
using System.Net;
using System.Net.Http;
using System.Threading.Tasks;
using Moq;
using Moq.Protected;
using NUnit.Framework;

namespace Dotnet.Script.Core.Tests
{
    [TestFixture]
    public class ScriptDownloaderTests
    {
        private ScriptDownloader _scriptDownloader;
        private Mock<HttpMessageHandler> _mockHttpMessageHandler;

        [SetUp]
        public void Setup()
        {
            _mockHttpMessageHandler = new Mock<HttpMessageHandler>();
            _scriptDownloader = new ScriptDownloader();
        }

        [Test]
        public async Task Download_WithPlainTextContent_ShouldReturnContent()
        {
            // Arrange
            var uri = "https://example.com/script.txt";
            var expectedContent = "Hello, World!";
            
            SetupMockHttpHandler(uri, expectedContent, "text/plain");

            // Act
            var result = await _scriptDownloader.Download(uri);

            // Assert
            Assert.AreEqual(expectedContent, result);
        }

        [Test]
        public async Task Download_WithGzipContent_ShouldDecompressContent()
        {
            // Arrange
            var uri = "https://example.com/script.gz";
            var originalContent = "Compressed Hello, World!";
            
            SetupMockHttpHandler(uri, CompressContent(originalContent), "application/gzip");

            // Act
            var result = await _scriptDownloader.Download(uri);

            // Assert
            Assert.AreEqual(originalContent, result);
        }

        [Test]
        public void Download_WithUnsupportedMediaType_ShouldThrowNotSupportedException()
        {
            // Arrange
            var uri = "https://example.com/script.bin";
            
            SetupMockHttpHandler(uri, "Binary content", "application/octet-stream");

            // Act & Assert
            Assert.ThrowsAsync<NotSupportedException>(async () => 
                await _scriptDownloader.Download(uri));
        }

        [Test]
        public async Task Download_WithNullMediaType_ShouldReturnContent()
        {
            // Arrange
            var uri = "https://example.com/script.txt";
            var expectedContent = "Hello, World!";
            
            SetupMockHttpHandler(uri, expectedContent, null);

            // Act
            var result = await _scriptDownloader.Download(uri);

            // Assert
            Assert.AreEqual(expectedContent, result);
        }

        private void SetupMockHttpHandler(string uri, string content, string mediaType)
        {
            var response = new HttpResponseMessage(HttpStatusCode.OK)
            {
                Content = new StringContent(content)
            };

            if (mediaType != null)
            {
                response.Content.Headers.ContentType = new System.Net.Http.Headers.MediaTypeHeaderValue(mediaType);
            }

            _mockHttpMessageHandler
                .Protected()
                .Setup<Task<HttpResponseMessage>>(
                    "SendAsync", 
                    ItExpr.Is<HttpRequestMessage>(req => req.RequestUri.ToString() == uri),
                    ItExpr.IsAny<System.Threading.CancellationToken>()
                )
                .ReturnsAsync(response);

            // Create HttpClient with mocked handler for testing
            var httpClient = new HttpClient(_mockHttpMessageHandler.Object);
            var field = typeof(ScriptDownloader).GetField("client", System.Reflection.BindingFlags.NonPublic | System.Reflection.BindingFlags.Instance);
            field?.SetValue(_scriptDownloader, httpClient);
        }

        private byte[] CompressContent(string content)
        {
            using (var outputStream = new MemoryStream())
            {
                using (var gzipStream = new GZipStream(outputStream, CompressionMode.Compress))
                using (var writer = new StreamWriter(gzipStream))
                {
                    writer.Write(content);
                }
                return outputStream.ToArray();
            }
        }
    }
}