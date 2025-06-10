// DataLayer/Models/Currency.cs
using System.ComponentModel.DataAnnotations;

namespace DataLayer.Models
{
    public class Currency
    {
        [Key]
        public int Id { get; set; } // <-- וודא שזה int
        public string Country { get; set; }
        public string Name { get; set; }
        public string Abbreviation { get; set; }
    }
}