using System.ComponentModel.DataAnnotations;

namespace movieGame.Models
{
    public class Hints
    {
        [Key]
        public int HintsId { get; set; }
        public string Genre { get; set; }
        public string ReleaseYear { get; set; }
        public string Director { get; set; }
    }
}