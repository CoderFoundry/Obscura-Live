namespace Obscura_Live.Client.Models
{

    public class MovieResponse
    {
        public int page { get; set; }
        public List<Movie> Results { get; set; } = [];
        public int TotalPages { get; set; }
        public int TotalResults { get; set; }
    }

 

}
