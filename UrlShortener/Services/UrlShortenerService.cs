using System;
using UrlShortener.Models;
using UrlShortener.Repositories.Interfaces;
using UrlShortener.Services.Interface;

namespace MyUrlShortenerApp.Services
{
    public class UrlShortenerService : IUrlShortenerService
    {
        private readonly IUrlRepository _urlRepository;
        private readonly ICacheService _cacheService;
        private const string BaseUrl = "http://localhost:5000/";

        public UrlShortenerService(IUrlRepository urlRepository, ICacheService cacheService)
        {
            _urlRepository = urlRepository;
            _cacheService = cacheService;
        }

        public UrlResponse ShortenUrl(UrlRequest request)
        {
            var shortId = GenerateShortId();
            var urlMapping = CreateUrlMapping(request.OriginalUrl, shortId);

            _urlRepository.AddUrlMapping(shortId, urlMapping);

            var shortUrlDetails = CreateUrlResponse(shortId);

            _cacheService.Set(shortId, request.OriginalUrl);

            return shortUrlDetails;
        }

        public UrlMapping? GetUrlMapping(string shortId)
        {
            if (_cacheService.Exists(shortId))
            {
                var origianlUrl= _cacheService.Get(shortId);
                return new UrlMapping { OriginalUrl = origianlUrl, ShortId = shortId };
            }

            var urlMapping = _urlRepository.GetUrlMapping(shortId);
            if (urlMapping != null)
            {
                // Cache the resolved URL for future use
                _cacheService.Set(shortId, urlMapping.OriginalUrl);
                return new UrlMapping { OriginalUrl = urlMapping.OriginalUrl, ShortId = shortId };
            }

            throw new KeyNotFoundException("URL not found.");
        }
        private string GenerateShortId()
        {
            var random = new Random();
            const string chars = "abcdefghijklmnopqrstuvwxyzABCDEFGHIJKLMNOPQRSTUVWXYZ0123456789";
            return new string(Enumerable.Range(0, 6).Select(_ => chars[random.Next(chars.Length)]).ToArray());
        }

        private UrlResponse CreateUrlResponse(string shortId)
        {
            return new UrlResponse
            {
                ShortenedUrl = $"{BaseUrl}{shortId}",
                ShortId = shortId
            };
        }
        private UrlMapping CreateUrlMapping(string originalUrl, string shortId)
        {
            return new UrlMapping
            {
                OriginalUrl = originalUrl,
                ShortId = shortId
            };
        }
    }
}
