using UrlShortener.Models;

namespace UrlShortener.Services.Interface
{
    public interface IUrlShortenerService
    {
        UrlResponse ShortenUrl(UrlRequest request);
        UrlMapping? GetUrlMapping(string shortId);        
    }
}
