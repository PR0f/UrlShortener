using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.ModelBinding;
using Microsoft.EntityFrameworkCore;
using Serilog;
using System;
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
            var shortUrl = await ShortUrlWithoutDuplication();
            
            if(shortUrl == String.Empty)
            {
                return NotFound();
            }

            var shortener = new Shortener()
            {
                Url = url,
                ShortUrl = shortUrl,
                TimeToLive = timeToLive,
            };

            return await CreateShortUrl(shortener);
        }

        [HttpPost("ShortenUrlWithShortId")]
        public ActionResult<string> ShortenUrlWithShortId(string url, string shortId, DateTime? timeToLive = null)
        {
            //_logger.Information(GenShortUrl());

            List<string> gen = new List<string>();
            for (var i = 0; i < 1000000; i++)
            {
                gen.Add(GenShortUrl());
            }

            _logger.Information($"count list: {gen.Count()} vs {gen.Distinct().Count()}");

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
                var elementExists = await _context.Shortener.FirstOrDefaultAsync<Shortener>(x => x.ShortUrl == genUrl);

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
