using System.ComponentModel.DataAnnotations;

namespace movieGame.Models
{
    public class Hint
    {
        [Key]
        public int HintId         { get; set; }
        public string Genre       { get; set; }
        public string ReleaseYear { get; set; }
        public string Director    { get; set; }
        public int MovieId        { get; set; }
    }
}