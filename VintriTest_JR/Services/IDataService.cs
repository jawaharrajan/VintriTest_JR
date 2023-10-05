using VintriTest_JR.Models;

namespace VintriTest_JR.Services
{
    public interface IDataService
    {
        Task<bool> SaveRatingData(Ratings rating);
        Task<string> GetRatingData(int Id);
    }
}
