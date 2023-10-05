using System.ComponentModel.DataAnnotations;

namespace VintriTest_JR.Models
{
    public class BeerResult
    {
        public  int id { get; set; }
        public string name { get; set; }
        public string description { get; set; }
        public UserRatings[] userRatings { get; set; }
    }

    public class UserRatings
    {
        public string username { get; set; }

        public int rating { get; set; }

        public string comments { get; set; }
    }
}
