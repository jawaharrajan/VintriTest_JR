using System.ComponentModel.DataAnnotations;

namespace VintriTest_JR.Models
{
    public class Ratings
    {
        [Required]
        public int Id { get; set; }

        [Required(AllowEmptyStrings = false,ErrorMessage ="Must provide a UserName")]
        public string Username { get; set; }

        [Required()]
        [Range(1, 5,
        ErrorMessage = "Value for rating must be between {1} and {2}.")]
        public int Rating { get; set; }

        public string Comments { get; set; }    
    }
}
