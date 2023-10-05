using System.Text.Json;
using VintriTest_JR.Models;

namespace VintriTest_JR.Helpers
{
    public class JsonDataHelper
    {
        public static string SaveRatingData(Ratings rating, string folder, string filename)
        {
            //trying to save to the downloads folder of the current user.
            try
            {
                //Determine the path to the Downloads folder
                string userProfile = Environment.GetFolderPath(Environment.SpecialFolder.UserProfile);
                string downloadsFolder = Path.Combine(userProfile, folder);

                // Combine the Downloads folder path with the desired filename
                string fullPath = Path.Combine(downloadsFolder, filename);

                List<Ratings> data;

                if (File.Exists(fullPath))
                {
                    var existingData = File.ReadAllText(fullPath);

                    // Try to deserialize the existing data into a list.
                    // If the file is empty or not a valid list, initialize an empty list.
                    try
                    {
                        data = JsonSerializer.Deserialize<List<Ratings>>(existingData) ?? new List<Ratings>();

                        // if one or more of the same id and same user already exists then remove them.             
                        data.RemoveAll(x => x.Id == rating.Id && x.Username == rating.Username);                           
                    }
                    catch
                    {
                        data = new List<Ratings>();
                    }
                }
                else
                {
                    data = new List<Ratings>();
                }

                data.Add(rating);
                var jsonString = JsonSerializer.Serialize(data, new JsonSerializerOptions { WriteIndented = true });
                File.WriteAllText(fullPath, jsonString);                     
                return "Rating Data saved";
            }
            catch (Exception ex)
            {
                return $"Unable to save Rating data - Error: {ex.Message}";
            }
        }

        public static string UpdateSearchDataWithRatings(List<BeerResult> beerSearchResult, string folder, string filename)
        {
            //Determine the path to the Downloads folder
            string userProfile = Environment.GetFolderPath(Environment.SpecialFolder.UserProfile);
            string downloadsFolder = Path.Combine(userProfile, folder);

            // Combine the Downloads folder path with the desired filename
            string fullPath = Path.Combine(downloadsFolder, filename);

            List<Ratings> data;

            if (File.Exists(fullPath))
            {
                var existingData = File.ReadAllText(fullPath);
                try
                {
                    data = JsonSerializer.Deserialize<List<Ratings>>(existingData) ?? new List<Ratings>();

                    //update the search results with user rating data.
                    foreach(var beer in beerSearchResult)
                    {
                        var currentUserRatings = data.Where(x => x.Id == beer.id)
                            .Select(s => new UserRatings
                            {
                                username = s.Username,
                                rating = s.Rating,
                                comments = s.Comments
                            })
                            .ToList();
                        if (currentUserRatings.Any())
                        {                           
                            beer.userRatings = currentUserRatings.ToArray();
                        }
                    }
                    var jsonString = JsonSerializer.Serialize(beerSearchResult, new JsonSerializerOptions { WriteIndented = true });
                    return jsonString;
                }
                catch (Exception ex)
                {
                    return $"Unable to add user Rating data - Error: {ex.Message}";
                }
            }
            else
            {
                return beerSearchResult.ToString();
            }
        }

        public static string GetBeerIdAndName(List<BeerIdAndName> beers)
        {
            List<BeerIdAndName> data;
            try
            {
                var beerIdAndName = beers.Select(x => new BeerIdAndName
                {
                    id = x.id, name = x.name
                }).ToList();
                var jsonString = JsonSerializer.Serialize(beerIdAndName, new JsonSerializerOptions { WriteIndented = true });
                return jsonString;
            }
            catch(Exception ex)
            {
                return $"Unable to get beer data - Error: {ex.Message}";
            }
        }
    }
}
