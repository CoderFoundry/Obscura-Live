using Obscura_Live.Client.Models;
using Obscura_Live.Client.Services;
using System.Text.Json;


namespace Obscura_Live.Services
{
    public class TMDBService : ITMDBService
    {
        private readonly HttpClient _http;
        private static readonly Random _random = new();

        private readonly JsonSerializerOptions _jsonOptions = new()
        {
            PropertyNamingPolicy = JsonNamingPolicy.SnakeCaseLower
        };

        public TMDBService(HttpClient http, IConfiguration config)
        {
            _http = http;

            const string configKey = "TmdbApiKey";

            string? devTmdbKey = config[configKey];
            string? prodTmdbKey = Environment.GetEnvironmentVariable(configKey);


            var tmdbKey = string.IsNullOrEmpty(devTmdbKey) ? prodTmdbKey : devTmdbKey;

            if (!string.IsNullOrEmpty(tmdbKey))
            {
                _http.BaseAddress = new Uri("https://api.themoviedb.org/3/");
                _http.DefaultRequestHeaders.Authorization = new("Bearer", tmdbKey);
            }
            else
            {
                Console.WriteLine($"No TMDB API key found in {configKey} or environment variables.");
            }
        }


        /// <summary>
        /// Get a random movie from TMDB
        /// </summary>
        /// <param name="yearStart"></param>
        /// <param name="yearEnd"></param>
        /// <param name="genres"></param>
        /// <returns></returns>
        /// <exception cref="NotImplementedException"></exception>
        public async Task<Movie> GetRandomMovieAsync(int? yearStart, int? yearEnd, string? genres)
        {
            //get a random movie
            string releaseDateGte = "";
            string releaseDateLte = "";

            string baseUrl = "discover/movie?include_adult=false&include_video=false&region=US&with_origin_country=US&with_original_language=en&sort_by=vote_count.desc&vote_count.gte=200";
            Movie randomMovie = new();

            if(yearStart is not null && yearEnd is not null && yearStart < yearEnd)
            {
                releaseDateGte = $"{yearStart}-01-01";
                releaseDateLte = $"{yearEnd}-12-31";

                baseUrl += $"&release_date.gte={releaseDateGte}&release_date.lte={releaseDateLte}";
            }  
            
            if(!string.IsNullOrEmpty(genres))
            {
                baseUrl += $"&with_genres={genres}";
            }   


            //limit the search to the first 20 pages
            int randomPage = _random.Next(1, 21);
            string discoverURL = $"{baseUrl}&page={randomPage}";

            //get a random movie 
            var intialResponse = await _http.GetFromJsonAsync<MovieResponse>(discoverURL, _jsonOptions)
                ?? throw new HttpIOException(HttpRequestError.InvalidResponse, "Could not find a random movie");

            //how many pages are there?
            int totalPages = Math.Min(intialResponse.TotalPages, 500);

            //we need to filter any movies that do not have a poster
            var filteredMovies = intialResponse.Results.Where(m => !string.IsNullOrEmpty(m.PosterPath)).ToList();

            if(filteredMovies.Count > 0)
            {
                int randomIndex = _random.Next(filteredMovies.Count);
                randomMovie = filteredMovies[randomIndex];
                randomMovie.PosterPath = $"https://image.tmdb.org/t/p/w500{randomMovie.PosterPath}";
                return randomMovie;
            }

            ///no movie found 
            int retryPage = totalPages;
            if (totalPages > 1) 
            { 
                retryPage = _random.Next(1, totalPages + 1);
            }

            string retryUrl = $"{baseUrl}&page={retryPage}";

            var retryResponse = await _http.GetFromJsonAsync<MovieResponse>(retryUrl, _jsonOptions)
                ?? throw new HttpIOException(HttpRequestError.InvalidResponse, "Could not find a random movie");

            var filteredRetryMovies = retryResponse.Results.Where(m => !string.IsNullOrEmpty(m.PosterPath)).ToList();

            if (filteredRetryMovies.Count > 0)
            {
                int randomIndex = _random.Next(filteredRetryMovies.Count);
                randomMovie = filteredRetryMovies[randomIndex];
                randomMovie.PosterPath = $"https://image.tmdb.org/t/p/w500{randomMovie.PosterPath}";
                return randomMovie;
            }
           
           throw new HttpIOException(HttpRequestError.InvalidResponse, "Could not find a random movie");
           
        }

        public async Task<List<Movie>> SearchMoviesAsync(string query, int page = 1)
        {
            var url = $"search/movie?query={query}&region=US&include_adult=false&language=en-US";

            MovieResponse response = await _http.GetFromJsonAsync<MovieResponse>(url, _jsonOptions)
                ?? throw new HttpIOException(HttpRequestError.InvalidResponse, "Search results could not be loaded");

            return response.Results.Where(r => r.VoteCount > 150).ToList();
        }

        public async Task<MovieDetails> GetMovieDetailsAsync(int movieId)
        {
            var url = $"movie/{movieId}?append_to_response=videos,credits&language=en-US";
           
            MovieDetails? response = await _http.GetFromJsonAsync<MovieDetails>(url, _jsonOptions)
                ?? throw new HttpIOException(HttpRequestError.InvalidResponse, "Movie details could not be loaded");
           
            return response;
        }

        public async Task<List<Genre>> GetGenresAsync()
        {
            var url = "genre/movie/list?language=en-Us";
            GenreResponse? response = await _http.GetFromJsonAsync<GenreResponse>(url, _jsonOptions)
                ?? throw new HttpIOException(HttpRequestError.InvalidResponse, "Genres could not be loaded");
            return response?.Genres?? new List<Genre>();
        }

        public async  Task<Video?> GetMovieTrailerAsync(int movieId)
        {
            var url = $"movie/{movieId}/videos?language=en-US";

            var videos = await _http.GetFromJsonAsync<MovieVideoResponse>(url, _jsonOptions)
                ?? throw new HttpIOException(HttpRequestError.InvalidResponse, "Videos could not be loaded");

            return videos.Results.FirstOrDefault(v => v.Site!.Contains("YouTube", StringComparison.Ordinal) && 
                                  v.Type!.Contains("Trailer", StringComparison.OrdinalIgnoreCase));
        }
    }
}
