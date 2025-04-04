using Obscura_Live.Client.Models;

namespace Obscura_Live.Client.Services
{
    public interface ITMDBService
    {
        Task<Movie> GetRandomMovieAsync(int? yearStart, int? yearEnd, string? genres);
    }
}
