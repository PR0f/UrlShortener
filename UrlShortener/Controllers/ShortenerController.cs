using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.ModelBinding;
using Microsoft.EntityFrameworkCore;
using Serilog;
using System;
using System.ComponentModel.DataAnnotations;
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

        [HttpPost("CreateUrl")]
        public async Task<ActionResult<string>> CreateUrl([Required] string url, DateTime? timeToLive = null)
        {
            var shortUrl = await ShortUrlWithoutDuplication();
            
            if(shortUrl == String.Empty)
            {
                return NotFound();
            }

            var shortener = new Shortener()
            {
                Url = url,
                ShortUrl = shortUrl,
                TimeToLive = timeToLive?.ToUniversalTime(),
            };

            return await CreateShortUrl(shortener);
        }

        [HttpPost("CreateUrlById")]
        public async Task<ActionResult<string>> CreateUrlById([Required]string url, [Required] string shortId, DateTime? timeToLive = null)
        {
            var elementExists = await _context.Shortener.FirstOrDefaultAsync<Shortener>(x => x.ShortUrl == shortId);

            if(elementExists != null)
            {
                _logger.Warning($"shortId is already used: {shortId}");
                return NotFound();
            }

            var shortener = new Shortener()
            {
                Url = url,
                ShortUrl = shortId,
                TimeToLive = timeToLive?.ToUniversalTime(),
            };

            return await CreateShortUrl(shortener);
        }

        [HttpGet("GetUrlById/{shortId}")]
        public async Task<ActionResult<string>> GetUrlById(string shortId)
        {
            _logger.Information(shortId);
            var shortener = await _context.Shortener.FirstOrDefaultAsync<Shortener>(_ => _.ShortUrl == shortId);
            if (shortener == null)
            {
                return NotFound();
            }

            if (shortener.TimeToLive == null)
            {
                return Ok(shortener.Url);
            }

            DateTime ttl = shortener.TimeToLive.Value;
            int diff = DateTime.Compare(ttl, DateTime.UtcNow);

            if (diff < 0)
            {
                return NotFound();
            }

            return Ok(shortener.Url);
        }

        [HttpDelete("DeleteUrlById/{shortId}")]
        public async Task<ActionResult<string>> DeleteUrlById(string shortId)
        {
            var shortener = await _context.Shortener.FirstOrDefaultAsync<Shortener>(_ => _.ShortUrl == shortId);
            if (shortener != null)
            {
                _context.Shortener.Remove(shortener);
                var result = await _context.SaveChangesAsync();
                _logger.Information($"Deleted Url: {shortener.Url}");
                return Ok();
            }

            _logger.Information($"Tried to deleted shortId: {shortId} but element doesnt exist");
            return NotFound();
        }

        #region Helpers

        private string GenShortUrl(int minLength = 5)
        {
            string result = String.Empty;

            char[] arrayOfAlphanumericals = [
                '0', '1', '2', '3', '4', '5', '6', '7', '8', '9',
                'a', 'b', 'c', 'd', 'e', 'f', 'g', 'h', 'i', 'j', 'k', 'l','m', 'n',
                'o', 'p', 'q','r', 's', 't', 'u', 'v', 'w', 'x', 'y', 'z',
                'A', 'B', 'C', 'D', 'E', 'F', 'G', 'H', 'I', 'J', 'K', 'L', 'M', 'N',
                'O', 'P', 'Q', 'R', 'S', 'T', 'U', 'V', 'W', 'X', 'Y', 'Z'
                ];

            var random = new Random();

            for (int i = 0; i < minLength; i ++)
            {
                int randomNumber = random.Next(0, arrayOfAlphanumericals.Length);
                result += arrayOfAlphanumericals[randomNumber];
            }
            
            return result;
        }

        private async Task<string> ShortUrlWithoutDuplication()
        {
            int nextGenAttempts = 10;

            var result = String.Empty;
            for (int i = 0; i < nextGenAttempts; i++)
            {
                var genUrl = GenShortUrl();
                var elementExists = await _context.Shortener.FirstOrDefaultAsync<Shortener>(_ => _.ShortUrl == genUrl);

                if (elementExists == null)
                {
                    result = genUrl;
                    break;
                }
            }

            return result;
        }

        private async Task<ActionResult<string>> CreateShortUrl(Shortener shortener)
        {
            await _context.AddAsync<Shortener>(shortener);
            var result = await _context.SaveChangesAsync();

            if (result > 0)
            {
                _logger.Information($"Added new Url {shortener.Url} as {shortener.ShortUrl}, ttl to {shortener.TimeToLive}");
                return Ok(shortener.ShortUrl);
            }

            return NotFound();
        }

        #endregion

    }
}
