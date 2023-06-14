using ClientApi.Models;

namespace ClientApi.Service
{
    public interface IMovie
    {
        Task<IEnumerable<Movie>> GetMovies();
        Task<Movie> GetMovie(string id);
        Task<Movie> CreateMovie(Movie movie);
        Task<Movie> UpdateMovie(Movie movie);
        Task DeleteMovie(int id);
       // Task<UserInfoViewModel> GetUserInfo();
    }
}
