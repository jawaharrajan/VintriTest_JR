using Microsoft.AspNetCore.Mvc;
using VintriTest_JR.Services;
using VintriTest_JR.Models;
using VintriTest_JR.ActionFilters;

namespace VintriTest_JR.Controllers
{
    [ApiController]
    [Route("api/Beer")]
    public class BeerController : ControllerBase
    {
        private readonly IApiService _apiService;

        public BeerController()
        {
            
        }
        public BeerController(IApiService apiService)
        {
            _apiService = apiService;
        }

        //Additional endpoint to get beers to test as a health check also get some Ids for testing
        [HttpGet("GetBeers")]
        public async Task<IActionResult> GetBeers()
        {
            try
            {
                var result = await _apiService.GetBeerIdAndName();
                return Ok(result);
            }
            catch (Exception ex) 
            {
                return BadRequest("Error getting Beer information.");
            }
        }

        //Allows users to sumbit a rating for given beer - must know some Beer Id's
        [HttpPost("SubmitRating")]
        [ServiceFilter(typeof(UsernameValidationFilterAttribute))]
        public async Task<IActionResult> SubmitRating([FromBody] Ratings ratingModel,int Id)
        {
            if (!ModelState.IsValid)
                return BadRequest(ModelState);

            try
            {
                var isValidId = await _apiService.IsBeerIdValid(Id);
                var result = await _apiService.SubmitBeerRatingAsync(ratingModel, Id);
                return Ok(result);
            }
            catch
            {
                return BadRequest("Error submitting rating.");
            }
        }


        [HttpGet("GetBeersBySearch")]
        public async Task<IActionResult> GetBeersBySearch(string searchTerm)
        {
            try
            {
                var result = await _apiService.SearchBeerAsync(searchTerm);
                return Ok(result);
            }
            catch(Exception ex)
            {
                return BadRequest($"Error Searching for your Beer - Error: {ex.Message}");
            }
        }
    }
}
