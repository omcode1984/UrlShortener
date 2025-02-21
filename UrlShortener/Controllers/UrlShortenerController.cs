using Microsoft.AspNetCore.Mvc;
using UrlShortener.Models;
using UrlShortener.Services.Interface;

namespace URLShortener.Controllers
{
    [Route("api")]
    [ApiController]
    public class UrlShortenerController : ControllerBase
    {
        private readonly IUrlShortenerService _urlShortenerService;
        private readonly ILogger<UrlShortenerController> _logger;

        public UrlShortenerController(IUrlShortenerService urlShortenerService, ILogger<UrlShortenerController> logger)
        {
            _urlShortenerService = urlShortenerService ?? throw new ArgumentNullException(nameof(urlShortenerService));
            _logger = logger;
        }

        // POST: api/shorten
        [HttpPost("shorten")]
        public IActionResult ShortenUrl([FromBody] UrlRequest request)
        {
            if (request == null || !IsValidUrl(request.OriginalUrl))
            {
                _logger.LogWarning("Invalid URL received for shortening.");
                return BadRequest("Invalid URL.");
            }

            var shortenUrl = _urlShortenerService.ShortenUrl(request);

            return Ok(shortenUrl);
        }

        // GET: api/{shortId}
        [HttpGet("{shortId}")]
        public IActionResult ResolveUrl(string shortId)
        {
            var urlMapping = _urlShortenerService.GetUrlMapping(shortId);
            if (urlMapping == null)
            {
                _logger.LogWarning($"URL not found for short ID: {shortId}");
                return NotFound("URL not found.");
            }

            return Ok(urlMapping.OriginalUrl);
        }

        // Helper method to validate the URL
        private bool IsValidUrl(string url)
        {
            return Uri.IsWellFormedUriString(url, UriKind.Absolute);
        }
    }
}
