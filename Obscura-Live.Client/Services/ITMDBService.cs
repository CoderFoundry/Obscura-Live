using Obscura_Live.Client.Models;

namespace Obscura_Live.Client.Services
{
    public interface ITMDBService
    {
        Task<Movie> GetRandomMovieAsync(int? yearStart, int? yearEnd, string? genres);

        Task<List<Movie>> SearchMoviesAsync(string query, int page = 1);

        Task<MovieDetails> GetMovieDetailsAsync(int movieId);

        Task<List<Genre>> GetGenresAsync();

        Task<Video?> GetMovieTrailerAsync(int movieId);
    }
}
