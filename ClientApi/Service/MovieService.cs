using ClientApi.Models;
using IdentityModel.Client;
using Newtonsoft.Json;

namespace ClientApi.Service
{
    public class MovieService : IMovie
    {
        public Task<Movie> CreateMovie(Movie movie)
        {
            throw new NotImplementedException();
        }

        public Task DeleteMovie(int id)
        {
            throw new NotImplementedException();
        }

        public Task<Movie> GetMovie(string id)
        {
            throw new NotImplementedException();
        }

        public async Task<IEnumerable<Movie>> GetMovies()
        {



            //// 1. "retrieve" our api credentials. This must be registered on Identity Server!
           
            var apiClientCredentials = new ClientCredentialsTokenRequest
            {
                Address = "https://localhost:7026/connect/token",

                ClientId = "movieClient",
                ClientSecret = "secret",

                // This is the scope our Protected API requires. 
                Scope = "movieAPI"
            };

            // creates a new HttpClient to talk to our IdentityServer (localhost:5005)
            var client = new HttpClient();

            //// just checks if we can reach the Discovery document. Not 100% needed but..

            var disco = await client.GetDiscoveryDocumentAsync("https://localhost:7026");
            if (disco.IsError)
            {
                return null; // throw 500 error
            }

            //// 2. Authenticates and get an access token from Identity Server
            
            var tokenResponse = await client.RequestClientCredentialsTokenAsync(apiClientCredentials);
            if (tokenResponse.IsError)
            {
                return null;
            }

            // Another HttpClient for talking now with our Protected API
            var apiClient = new HttpClient();

            //// 3. Set the access_token in the request Authorization: Bearer <token>
            client.SetBearerToken(tokenResponse.AccessToken);

            //// 4. Send a request to our Protected API
            var response = await client.GetAsync("https://localhost:7045/api/Movies");
            response.EnsureSuccessStatusCode();

            var content = await response.Content.ReadAsStringAsync();

            var movieList = JsonConvert.DeserializeObject<List<Movie>>(content);
            return movieList;



            //for controller testing purpose
            //var data = new List<Movie>();

            //data.Add(new Movie
            //{
            //    Id = 1,
            //    Title = "hello",
            //    Genre = "hello",
            //    ImageUrl = "hello",
            //    Owner = "je",
            //    ReleaseDate = DateTime.Now,
            //    Rating = "1"
            //});

            //return Task.FromResult<IEnumerable<Movie>>(data);
        }

        public Task<Movie> UpdateMovie(Movie movie)
        {
            throw new NotImplementedException();
        }
    }
}
