using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using movieGame.Models;
using Newtonsoft.Json.Linq;

namespace movieGame.Controllers
{
    public class GetPlayerInfoController : Controller
    {
        private MovieContext _context;
        private Player _player;
        private string _start = "STARTED";
        private string _complete = "COMPLETED";
        private List<Movie> _playerMovies;
        private List<MoviePlayerJoin> _moviePlayerJoinList;
        private List<String> _playerMoviesPosters;
        private JObject _playerMoviesJObject;
        private GetMovieInfoController _getMovieInfoController;



        public GetPlayerInfoController (MovieContext context) {
            _context = context;
        }


        public string Start {
            get => _start;
            set => _start = value;
        }

        public string Complete {
            get => _complete;
            set => _complete = value;

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



        // examples of setting temp data; how can I use this?
        public IActionResult SetTempData()
        {
            TempData["Variable"] = "Hello World";
            return RedirectToAction("OtherMethod");
        }
        public void GetTempData()
        {
            Console.WriteLine(TempData["Variable"]);
            // writes "Hello World" if redirected to from Method()
        }


        public Player GetPlayer (int id)
        {
            // Start.ThisMethod();

            Player = _context.Players
                .Include(m => m.MoviePlayerJoin)
                .ThenInclude(n => n.Movie)
                .SingleOrDefault(p => p.PlayerId == id);

            return Player;
        }



        public List<MoviePlayerJoin> PlayersMovieList (int id)
        {
            // Start.ThisMethod();

            Player = GetPlayer(id);
            var PlayerMovieList = Player.MoviePlayerJoin.ToList();
            // Complete.ThisMethod();
            return PlayerMovieList;
        }



        public List<Movie> PlayersMovies (int id)
        {
            // Start.ThisMethod();
            Player = GetPlayer(id);

            MoviePlayerJoinList = Player.MoviePlayerJoin.ToList();
            PlayerMovies = new List<Movie>();

            foreach(var movie in MoviePlayerJoinList)
            {
                Movie _movie = new Movie
                {
                    MovieId = movie.MovieId,
                    Title = movie.Movie.Title,
                    Year = movie.Movie.Year,
                    Poster = movie.Movie.Poster
                };

                MoviePosters(_movie.Title, _movie.Year);

                int GamesWon = Player.GamesWon;
                if(movie.MovieId <= GamesWon)
                {
                    PlayerMovies.Add(_movie);
                }
            }

            // Complete.ThisMethod();
            return  PlayerMovies;
        }


        public List<string> MoviePosters (string title, int year)
        {
            PlayerMoviesPosters = new List<string>();
            GetMovie = new GetMovieInfoController(_context);
            PlayerMoviesJObject = GetMovie.GetMovieJSON(title, year);

            string MoviePoster = PlayerMoviesJObject["Poster"].ToString();
            PlayerMoviesPosters.Add(MoviePoster);

            return PlayerMoviesPosters;
        }


        public IList<Player> GetTopTenLeaders ()
        {
            // Start.ThisMethod();
            // LEADERS ---> System.Collections.Generic.List`1[movieGame.Models.Player]
            var TopTenLeaders = _context.Players.OrderByDescending(t => t.Points).Take(10).ToList();

            return TopTenLeaders;
        }


    }
}