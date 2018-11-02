using System;
using System.Collections.Generic;
using System.Linq;
using movieGame.Models;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Newtonsoft.Json.Linq;
using movieGame.Controllers.Game.MixedControllers;

namespace movieGame.Controllers.PlayerControllers
{
    public class GetPlayerInfoController : Controller
    {
        private MovieContext _context;
        private User _player;

        private List<Movie> _playerMovies;
        private List<MovieUserJoin> _movieUserJoinList;
        private List<String> _playerMoviesPosters;
        private JObject _playerMoviesJObject;
        private GetMovieInfoController _getMovieInfoController;

        public GetPlayerInfoController (MovieContext context)
        {
            _context = context;
        }

        public List<Movie> playerMovies
        {
            get => _playerMovies;
            set => _playerMovies = value;
        }

        public List<MovieUserJoin> movieUserJoinList
        {
            get => _movieUserJoinList;
            set => _movieUserJoinList = value;
        }

        public List<string> playerMoviesPosters
        {
            get => _playerMoviesPosters;
            set => _playerMoviesPosters = value;
        }

        public JObject playerMoviesJObject
        {
            get => _playerMoviesJObject;
            set => _playerMoviesJObject = value;
        }

        public GetMovieInfoController GetMovie
        {
            get => _getMovieInfoController;
            set => _getMovieInfoController = value;
        }

        public User Player
        {
            get => _player;
            set => _player = value;
        }

        public User GetPlayer (int id)
        {
            Player = _context.Users
                .Include (m => m.MovieUserJoin)
                .ThenInclude (n => n.Movie)
                .SingleOrDefault (p => p.UserId == id);
            return Player;
        }

        public List<MovieUserJoin> UsersMovieList (int id)
        {
            Player = GetPlayer (id);
            var playerMovieList = Player.MovieUserJoin.ToList ();
            return playerMovieList;
        }

        public List<Movie> UsersMovies (int id)
        {
            Player = GetPlayer (id);
            movieUserJoinList = Player.MovieUserJoin.ToList ();
            playerMovies = new List<Movie> ();

            foreach (var movie in movieUserJoinList)
            {
                Movie _movie = new Movie
                {
                    MovieId = movie.MovieId,
                    Title = movie.Movie.Title,
                    Year = movie.Movie.Year,
                    Poster = movie.Movie.Poster
                };

                MoviePosters (_movie.Title, _movie.Year);

                int gamesWon = Player.GamesWon;
                if (movie.MovieId <= gamesWon)
                {
                    playerMovies.Add (_movie);
                }
            }
            return playerMovies;
        }

        public List<string> MoviePosters (string title, int year)
        {
            playerMoviesPosters = new List<string> ();
            GetMovie = new GetMovieInfoController (_context);
            playerMoviesJObject = GetMovie.GetMovieJSON (title, year);

            string moviePoster = playerMoviesJObject["Poster"].ToString ();
            playerMoviesPosters.Add (moviePoster);

            return playerMoviesPosters;
        }

        public IList<User> GetTopTenLeaders ()
        {
            // LEADERS ---> System.Collections.Generic.List`1[movieGame.Models.Player]
            var topTenLeaders = _context.Users.OrderByDescending (t => t.Points).Take (10).ToList ();
            return topTenLeaders;
        }

    }
}