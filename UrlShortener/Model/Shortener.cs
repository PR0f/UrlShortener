namespace UrlShortener.Model
{
    public class Shortener
    {
        public Guid Id { get; set; }
        public string Url { get; set; } = string.Empty;
        public string ShortUrl { get; set; } = string.Empty;
        public DateTime? TimeToLive { get; set; }
    }
}
