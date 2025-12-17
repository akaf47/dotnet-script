using System;
using System.IO;
using System.IO.Compression;
using System.Net;
using System.Net.Http;
using System.Threading.Tasks;
using Xunit;
using Moq;
using Moq.Protected;
using System.Threading;

namespace Dotnet.Script.Core.Tests
{
    public class ScriptDownloaderTests
    {
        [Fact]
        public async Task Download_ShouldHandleTextPlainContent()
        {
            var mockHttpMessageHandler = new Mock<HttpMessageHandler>();
            mockHttpMessageHandler.Protected()
                .Setup<Task<HttpResponseMessage>>(
                    "SendAsync",
                    ItExpr.IsAny<HttpRequestMessage>(),
                    ItExpr.IsAny<CancellationToken>()
                )
                .ReturnsAsync(new HttpResponseMessage
                {
                    StatusCode = HttpStatusCode.OK,
                    Content = new StringContent("Test Content")
                });

            var httpClient = new HttpClient(mockHttpMessageHandler.Object);
            var downloader = new ScriptDownloader();

            var result = await downloader.Download("http://test.com");

            Assert.Equal("Test Content", result);
        }

        [Fact]
        public async Task Download_ShouldHandleGzipContent()
        {
            var mockHttpMessageHandler = new Mock<HttpMessageHandler>();
            mockHttpMessageHandler.Protected()
                .Setup<Task<HttpResponseMessage>>(
                    "SendAsync",
                    ItExpr.IsAny<HttpRequestMessage>(),
                    ItExpr.IsAny<CancellationToken>()
                )
                .ReturnsAsync(new HttpResponseMessage
                {
                    StatusCode = HttpStatusCode.OK,
                    Content = new StreamContent(CreateGzipStream("Compressed Content"))
                });

            var httpClient = new HttpClient(mockHttpMessageHandler.Object);
            var downloader = new ScriptDownloader();

            var result = await downloader.Download("http://test.com");

            Assert.Equal("Compressed Content", result);
        }

        [Fact]
        public async Task Download_ShouldThrowForUnsupportedMediaType()
        {
            var mockHttpMessageHandler = new Mock<HttpMessageHandler>();
            mockHttpMessageHandler.Protected()
                .Setup<Task<HttpResponseMessage>>(
                    "SendAsync",
                    ItExpr.IsAny<HttpRequestMessage>(),
                    ItExpr.IsAny<CancellationToken>()
                )
                .ReturnsAsync(new HttpResponseMessage
                {
                    StatusCode = HttpStatusCode.OK,
                    Content = new StringContent("Test Content", System.Text.Encoding.UTF8, "application/pdf")
                });

            var httpClient = new HttpClient(mockHttpMessageHandler.Object);
            var downloader = new ScriptDownloader();

            await Assert.ThrowsAsync<NotSupportedException>(() => 
                downloader.Download("http://test.com"));
        }

        private Stream CreateGzipStream(string content)
        {
            var outputStream = new MemoryStream();
            using (var gzipStream = new GZipStream(outputStream, CompressionMode.Compress, true))
            {
                var bytes = System.Text.Encoding.UTF8.GetBytes(content);
                gzipStream.Write(bytes, 0, bytes.Length);
            }
            outputStream.Position = 0;
            return outputStream;
        }
    }
}