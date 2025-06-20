using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Serilog;
using ILogger = Serilog.ILogger;

namespace UrlShortener.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class ShortenerController : ControllerBase
    {
        private readonly ILogger _logger;
        public ShortenerController(ILogger logger)
        {
            _logger = logger;
        }

        [HttpPost("ShortenUrl")]
        public ActionResult<string> ShortenUrl(string url, DateTime? timeToLive = null)
        {
            _logger.Information("Test");

            return "";
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
