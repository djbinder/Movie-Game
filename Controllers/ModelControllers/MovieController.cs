using movieGame.Models;
using Newtonsoft.Json.Linq;
using RestSharp;
using C = System.Console;

#pragma warning disable IDE0052
namespace movieGame.Controllers
{
    public class MovieController
    {
        private readonly MovieContext _context;

        public MovieController(MovieContext context)
        {
            _context = context;
        }





        #region FROM IMDB ------------------------------------------------------------


        // * Get all of a movie's JSON info based on movie title and release year
        public JObject GetMovieJSON (string movieName, int movieYear)
        {
            string apiQueryTitleYear = $"https://www.omdbapi.com/?t={movieName}&y={movieYear}&apikey=4514dc2d";

            RestClient client   = new RestClient  (apiQueryTitleYear);
            RestRequest request = new RestRequest (Method.GET);

            request.AddHeader ("Postman-Token", "688d818e-29a5-498f-9c1c-907c7cb77c7f")
                .AddHeader ("Cache-Control", "no-cache");

            IRestResponse response = client.Execute (request);
            string responseJSON    = response.Content;

            JObject movieJObject = JObject.Parse (responseJSON);

            return movieJObject;
        }




        #endregion FROM IMDB ------------------------------------------------------------






        #region PRINTING PRESS ------------------------------------------------------------





        #endregion PRINTING PRESS ------------------------------------------------------------



    }
}
