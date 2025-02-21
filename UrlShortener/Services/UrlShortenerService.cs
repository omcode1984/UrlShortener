using System;
using UrlShortener.Models;
using UrlShortener.Repositories.Interfaces;
using UrlShortener.Services.Interface;

namespace MyUrlShortenerApp.Services
{
    public class UrlShortenerService : IUrlShortenerService
    {
        private readonly IUrlRepository _urlRepository;
        private const string BaseUrl = "http://localhost:5000/";

        public UrlShortenerService(IUrlRepository urlRepository)
        {
            _urlRepository = urlRepository;
        }

        public UrlResponse ShortenUrl(UrlRequest request)
        {
            var shortId = _urlRepository.GenerateShortId();
            var urlMapping = CreateUrlMapping(request.OriginalUrl, shortId);

            _urlRepository.AddUrlMapping(shortId, urlMapping);

            return CreateUrlResponse(shortId);
        }

        public UrlMapping? GetUrlMapping(string shortId)
        {
           return _urlRepository.GetUrlMapping(shortId);
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
