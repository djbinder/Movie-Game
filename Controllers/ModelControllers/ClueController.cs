using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using CsvHelper;
using Microsoft.AspNetCore.Mvc;
using movieGame.Models;
using C = System.Console;

namespace movieGame.Controllers.ModelControllers
{
    [Route("clue")]
    [ApiController]
    public class ClueController : ControllerBase
    {
        private readonly MovieGameContext _context;

        public ClueController(MovieGameContext context)
        {
            _context = context;
        }


        private readonly string csvFileLocation = "CSVfiles/movieGame_clues.110218-1.csv";


        // https://127.0.0.1:5001/clue/seed

        // [HttpGet]
        [HttpGet("seed")]
        public ActionResult SeedCluesFromCsv()
        {
            C.WriteLine("Seed Clues From Csv");
            
            using(StreamReader reader = new StreamReader(csvFileLocation))
            using(CsvReader csv       = new CsvReader(reader))
            {
                csv.Configuration.RegisterClassMap<ClueMap>();

                IEnumerable<Clue> clueRecords = csv.GetRecords<Clue>();
                C.WriteLine($"clueRecords : {clueRecords.Count()}");

                foreach(Clue clue in clueRecords)
                {
                    clue.DateCreated = DateTime.Now;
                    clue.DateUpdated = DateTime.Now;
                }

                _context.AttachRange(clueRecords);
                _context.AddRange(clueRecords);
                _context.SaveChanges();
            }
            return Ok();
        }
    }
}