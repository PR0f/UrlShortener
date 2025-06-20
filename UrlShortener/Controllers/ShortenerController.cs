using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Serilog;
using UrlShortener.Model;
using ILogger = Serilog.ILogger;

namespace UrlShortener.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class ShortenerController : ControllerBase
    {
        private readonly ILogger _logger;
        private readonly ApplicationDbContext _context;

        public ShortenerController(ILogger logger, ApplicationDbContext context)
        {
            _logger = logger;
            _context = context;
        }

        [HttpPost("ShortenUrl")]
        public async Task<ActionResult<string>> ShortenUrl(string url, DateTime? timeToLive = null)
        {
            var shortUrl = String.Empty;

            var shortener = new Shortener()
            {
                Url = url,
                ShortUrl = shortUrl,
                TimeToLive = timeToLive,
            };
            await _context.AddAsync<Shortener>(shortener);
            var result = await _context.SaveChangesAsync();

            if(result > 0)
            {
                _logger.Information($"Added new Url {url} as {shortUrl}, ttl to {timeToLive}");
                return Ok(shortUrl);
            }

            return NotFound();
        }

        [HttpPost("ShortenUrlWithShortId")]
        public ActionResult<string> ShortenUrlWithShortId(string url, string shortId, DateTime? timeToLive = null)
        {
            _logger.Information("Test");

            return "";
        }

        [HttpGet("GetUrlById/{shortId}")]
        public ActionResult<string> GetUrlById(string shortId)
        {
            _logger.Information(shortId);

            return "";
        }

        [HttpDelete("DeleteUrlById/{shortId}")]
        public ActionResult<string> DeleteUrlById(string shortId)
        {
            _logger.Information(shortId);

            return "";
        }

    }
}
