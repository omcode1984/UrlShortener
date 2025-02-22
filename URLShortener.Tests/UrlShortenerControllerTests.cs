using Moq;
using System;
using URLShortener.Controllers;
using Xunit;
using Microsoft.AspNetCore.Mvc;
using UrlShortener.Models;
using UrlShortener.Repositories.Interfaces;
using System.Xml.Serialization;
using Microsoft.AspNetCore.Http.HttpResults;
using UrlShortener.Services.Interface;
using MyUrlShortenerApp.Services;
using Castle.Core.Logging;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Caching.Memory;

namespace URLShortener.Tests
{
    public class UrlShortenerControllerTests
    {
        private readonly Mock<IUrlRepository> _mockUrlRepository;
        private readonly Mock<ILogger<UrlShortenerController>> _mockLogger;
        private readonly UrlShortenerController _controller;
        private readonly IUrlShortenerService _urlShortenerService;
        private readonly Mock<InMemoryCacheService> _mockCacheService;

        public UrlShortenerControllerTests()
        {
            _mockUrlRepository = new Mock<IUrlRepository>();
            _mockLogger = new Mock<ILogger<UrlShortenerController>>();
            var memoryCache = new MemoryCache(new MemoryCacheOptions());
            _mockCacheService = new Mock<InMemoryCacheService>(memoryCache);

            _urlShortenerService = new UrlShortenerService(_mockUrlRepository.Object, _mockCacheService.Object);
            _controller = new UrlShortenerController(_urlShortenerService, _mockLogger.Object);
        }

        // Test for valid URL shortening
        [Fact]
        public void ShortenUrl_WhenGivenValidUrl_ReturnsShortenedUrl()
        {
            // Arrange
            var originalUrl = "https://www.example.com";
            var urlRequest = CreateValidUrlRequest(originalUrl);
                       
            // Act
            var result = _controller.ShortenUrl(urlRequest) as OkObjectResult;

            // Assert
            UrlResponse urlDetails = result.Value as UrlResponse;
            Assert.NotNull(urlDetails);
            Assert.Equal(result.StatusCode, 200);
            Assert.NotEmpty(urlDetails.ShortenedUrl);
            _mockUrlRepository.Verify(repo => repo.AddUrlMapping(It.IsAny<string>(), It.IsAny<UrlMapping>()), Times.Once);
           
        }

        [Fact]
        public void ResolveUrl_ShouldReturnUrl_FromCache()
        {
            // Arrange
            string shortId = "abc123";
            string originalUrl = "https://example.com";
            _mockCacheService.Setup(c => c.Get(shortId)).Returns(originalUrl);
            _mockCacheService.Setup(c => c.Exists(shortId)).Returns(true);

            // Act
            var result = _urlShortenerService.GetUrlMapping(shortId);

            // Assert
            Assert.Equal(originalUrl, result.OriginalUrl);
            _mockUrlRepository.Verify(r => r.GetUrlMapping(It.IsAny<string>()), Times.Never);
        }
        // Test for invalid URL shortening
        [Fact]
        public void ShortenUrl_WhenGivenInvalidUrl_ReturnsBadRequest()
        {
            // Arrange
            var invalidUrlRequest = new UrlRequest { OriginalUrl = "invalid_url" };

            // Act
            var result = _controller.ShortenUrl(invalidUrlRequest) as BadRequestObjectResult;

            // Assert
            AssertBadRequest(result);
        }

        // Test for resolving an existing short URL
        [Fact]
        public void ResolveUrl_WhenShortIdExists_ReturnsRedirectToOriginalUrl()
        {
            // Arrange
            var shortId = "abc123";
            var originalUrl = "https://www.example.com";
            SetupUrlRepositoryToReturnOriginalUrl(shortId, originalUrl);

            // Act
            var result = _controller.ResolveUrl(shortId) as OkObjectResult;

            // Assert
            AssertRedirectResult(result, originalUrl);
        }

        // Test for resolving a non-existing short URL
        [Fact]
        public void ResolveUrl_WhenShortIdDoesNotExist_ReturnsNotFound()
        {
            // Arrange
            var nonExistingShortId = "nonexistent123";
            SetupUrlRepositoryToReturnNullForShortId(nonExistingShortId);

            // Act
            var result = _controller.ResolveUrl(nonExistingShortId) as NotFoundObjectResult;

            // Assert
            AssertNotFound(result);
        }

        // Helper method to create a valid URL request
        private UrlRequest CreateValidUrlRequest(string url)
        {
            return new UrlRequest { OriginalUrl = url };
        }

        // Helper method to set up the repository to return the original URL for a short ID
        private void SetupUrlRepositoryToReturnOriginalUrl(string shortId, string originalUrl)
        {
            var urlMapping = new UrlMapping { OriginalUrl = originalUrl, ShortId = shortId };
            _mockUrlRepository.Setup(repo => repo.GetUrlMapping(shortId)).Returns(urlMapping);
        }

        // Helper method to set up the repository to return null for a non-existent short ID
        private void SetupUrlRepositoryToReturnNullForShortId(string shortId)
        {
            _mockUrlRepository.Setup(repo => repo.GetUrlMapping(shortId)).Returns((UrlMapping)null);
        }

        // Helper method to assert the BadRequest response
        private void AssertBadRequest(BadRequestObjectResult result)
        {
            Assert.NotNull(result);
            Assert.Equal("Invalid URL.", result.Value);
        }

        // Helper method to assert the RedirectResult
        private void AssertRedirectResult(OkObjectResult result, string expectedUrl)
        {
            Assert.NotNull(result);
            Assert.Equal(expectedUrl, result.Value);
        }

        // Helper method to assert the NotFoundObjectResult
        private void AssertNotFound(NotFoundObjectResult result)
        {
            Assert.NotNull(result);
            Assert.Equal("URL not found.", result.Value);
        }
    }
}
