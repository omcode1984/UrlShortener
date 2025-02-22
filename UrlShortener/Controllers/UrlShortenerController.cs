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
            try
            {
                var shortenUrl = _urlShortenerService.ShortenUrl(request);

                return Ok(shortenUrl);
            }
            catch (Exception ex)
            {
                _logger.LogCritical(ex, "Unexpected error: {Message}", ex.Message);
                return StatusCode(500, new { Message = "An unexpected error occurred." });
            }
        }

        // GET: api/{shortId}
        [HttpGet("{shortId}")]
        public IActionResult ResolveUrl(string shortId)
        {
            try
            {
                var urlMapping = _urlShortenerService.GetUrlMapping(shortId);
                if (urlMapping == null)
                {
                    _logger.LogWarning($"URL not found for short ID: {shortId}");
                    return NotFound("URL not found.");
                }

                return Ok(urlMapping.OriginalUrl);
            }
            catch (KeyNotFoundException ex)
            {
                return NotFound(ex.Message);
            }
            catch (Exception ex)
            {
                _logger.LogCritical(ex, "Unexpected error: {Message}", ex.Message);
                return StatusCode(500, new { Message = "An unexpected error occurred." });
            }
        }
    }
}
