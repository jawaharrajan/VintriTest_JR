using Microsoft.Extensions.Options;
using System.Text.Json;
using VintriTest_JR.Helpers;
using VintriTest_JR.Models;

namespace VintriTest_JR.Services
{
    public class ApiService : IApiService
    {
        private readonly IHttpClientFactory _clientFactory;
        private readonly Config _configSettings ;

        public ApiService(IHttpClientFactory clientFactory, IOptions<Config> settings)
        {
            _clientFactory = clientFactory;
            _configSettings = settings.Value;
        }

        public async Task<string> GetBeerIdAndName()
        {
            try
            {
                var client = _clientFactory.CreateClient();
                var response = await client.GetAsync($"{_configSettings.PunkBeerApi}");
                response.EnsureSuccessStatusCode();
                var data = await response.Content.ReadAsStringAsync();

                var resultData = JsonSerializer.Deserialize<List<BeerIdAndName>>(data);
                var toReturndata = JsonDataHelper.GetBeerIdAndName(resultData);
                return toReturndata;
            }
            catch (Exception ex )
            {
                return $"Error occured - Error: {ex.Message}";
            }

        }

        public async Task<bool> IsBeerIdValid(int i)
        {
            try
            {
                var client = _clientFactory.CreateClient();
                var response = await client.GetAsync($"{_configSettings.PunkBeerApi}?ids={i}");
                response.EnsureSuccessStatusCode();
                var data = await response.Content.ReadAsStringAsync();
                return true;
            }
            catch (Exception)
            {
                return false; // $"Error occured - Error: {ex.Message}";
            }
        }

        public async Task<string> SearchBeerAsync(string searchTerm )
        {
            try
            {
                var client = _clientFactory.CreateClient();
                var response = await client.GetAsync($"{_configSettings.PunkBeerApi}?beer_name={searchTerm}");
                response.EnsureSuccessStatusCode();
                var data = await response.Content.ReadAsStringAsync();

                var resultData = JsonSerializer.Deserialize<List<BeerResult>>(data);
                var toReturndata = JsonDataHelper.UpdateSearchDataWithRatings(resultData, _configSettings.DbFolder, _configSettings.DbFileName);
                return toReturndata;
            }
            catch (Exception ex)
            {
                return $"Error occured - Error: {ex.Message}";
            }
        }

        public async Task<string> SubmitBeerRatingAsync(Ratings rating, int i)
        {
            try
            {
                var client = _clientFactory.CreateClient();
                var response = await client.GetAsync($"{_configSettings.PunkBeerApi}?ids={i}");
                response.EnsureSuccessStatusCode();

                //save rating as Json
                var result = JsonDataHelper.SaveRatingData(rating, _configSettings.DbFolder, _configSettings.DbFileName);

                return result;
            }
            catch (Exception ex)
            {
                return ex.ToString();
            }           
        }

    }
}
