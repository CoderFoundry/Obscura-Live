namespace Obscura_Live.Client.Models
{
    public class Video
    {
        public string? Iso6391 { get; set; }
        public string? Iso31661 { get; set; }
        public string? Name { get; set; }
        public string? Key { get; set; }
        public string? Site { get; set; }
        public int Size { get; set; }
        public string? Type { get; set; }
        public bool Official { get; set; }
        public DateTime PublishedAt { get; set; }
        public string? Id { get; set; }
    }

    public class MovieVideoResponse
    {
        public int Id { get; set; }
        public List<Video> Results { get; set; } = [];
    }
}
