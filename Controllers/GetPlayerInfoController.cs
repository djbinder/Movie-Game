using System;
using System.Collections.Generic;
using System.Linq;
using movieGame.Models;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Newtonsoft.Json.Linq;

namespace movieGame.Controllers {
    public class GetPlayerInfoController : Controller {
        private MovieContext _context;
        private Player _player;

        private List<Movie> _playerMovies;
        private List<MoviePlayerJoin> _moviePlayerJoinList;
        private List<String> _playerMoviesPosters;
        private JObject _playerMoviesJObject;
        private GetMovieInfoController _getMovieInfoController;

        public GetPlayerInfoController (MovieContext context) {
            _context = context;
        }

        public List<Movie> PlayerMovies {
            get => _playerMovies;
            set => _playerMovies = value;
        }

        public List<MoviePlayerJoin> MoviePlayerJoinList {
            get => _moviePlayerJoinList;
            set => _moviePlayerJoinList = value;
        }

        public List<string> PlayerMoviesPosters {
            get => _playerMoviesPosters;
            set => _playerMoviesPosters = value;
        }

        public JObject PlayerMoviesJObject {
            get => _playerMoviesJObject;
            set => _playerMoviesJObject = value;
        }

        public GetMovieInfoController GetMovie {
            get => _getMovieInfoController;
            set => _getMovieInfoController = value;
        }

        public Player Player {
            get => _player;
            set => _player = value;
        }

        public Player GetPlayer (int id) {

            Player = _context.Players
                .Include (m => m.MoviePlayerJoin)
                .ThenInclude (n => n.Movie)
                .SingleOrDefault (p => p.PlayerId == id);

            return Player;
        }

        public List<MoviePlayerJoin> PlayersMovieList (int id) {

            Player = GetPlayer (id);
            var PlayerMovieList = Player.MoviePlayerJoin.ToList ();
            // Complete.ThisMethod();
            return PlayerMovieList;
        }

        public List<Movie> PlayersMovies (int id) {
            Player = GetPlayer (id);

            MoviePlayerJoinList = Player.MoviePlayerJoin.ToList ();
            PlayerMovies = new List<Movie> ();

            foreach (var movie in MoviePlayerJoinList) {
                Movie _movie = new Movie {
                    MovieId = movie.MovieId,
                    Title = movie.Movie.Title,
                    Year = movie.Movie.Year,
                    Poster = movie.Movie.Poster
                };

                MoviePosters (_movie.Title, _movie.Year);

                int GamesWon = Player.GamesWon;
                if (movie.MovieId <= GamesWon) {
                    PlayerMovies.Add (_movie);
                }
            }

            // Complete.ThisMethod();
            return PlayerMovies;
        }

        public List<string> MoviePosters (string title, int year) {
            PlayerMoviesPosters = new List<string> ();
            GetMovie = new GetMovieInfoController (_context);
            PlayerMoviesJObject = GetMovie.GetMovieJSON (title, year);

            string MoviePoster = PlayerMoviesJObject["Poster"].ToString ();
            PlayerMoviesPosters.Add (MoviePoster);

            return PlayerMoviesPosters;
        }

        public IList<Player> GetTopTenLeaders () {
            // LEADERS ---> System.Collections.Generic.List`1[movieGame.Models.Player]
            var TopTenLeaders = _context.Players.OrderByDescending (t => t.Points).Take (10).ToList ();

            return TopTenLeaders;
        }

    }
}