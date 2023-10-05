using VintriTest_JR.Models;

namespace VintriTest_JR.Services
{
    public interface IApiService
    {
        Task<string> GetBeerIdAndName();
        Task<bool> IsBeerIdValid(int id);
        Task<string> SubmitBeerRatingAsync(Ratings rating, int Id);
        Task<string> SearchBeerAsync(string searchTerm);
    }
}
