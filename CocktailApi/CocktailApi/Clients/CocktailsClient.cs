using CocktailApi.Constant;
using CocktailApi.Model;
using Microsoft.Extensions.Options;
using Newtonsoft.Json;

namespace CocktailApi.Clients
{
    public class CocktailsClient
    {
        private HttpClient _client;
        private static string _address;
        private static string _apikey;
        public CocktailsClient()
        {
            _address = Constants.address;
            _apikey = Constants.apikey;

            _client = new HttpClient();
            _client.BaseAddress = new Uri(_address);

        }
        public async Task<Cocktail> SearchCocktailByName(string Drink)
        {
            var response = await _client.GetAsync($"{_apikey}/search.php?s={Drink}");
            response.EnsureSuccessStatusCode();
            
            var content = response.Content.ReadAsStringAsync().Result;
            var result = JsonConvert.DeserializeObject<Cocktail>(content);
            return result;
        }
        public async Task<Ingredients> SearchIngredientByName(string Ingredient)
        {
            var response = await _client.GetAsync($"{_apikey}/search.php?i={Ingredient}");
            response.EnsureSuccessStatusCode();

            var content = response.Content.ReadAsStringAsync().Result;
            var result = JsonConvert.DeserializeObject<Ingredients>(content);
            return result;
        }
        public async Task<Cocktail> LookupARandomCocktail()
        {
            var response = await _client.GetAsync($"{_apikey}/random.php");
            response.EnsureSuccessStatusCode();

            var content = response.Content.ReadAsStringAsync().Result;
            var result = JsonConvert.DeserializeObject<Cocktail>(content);
            return result;
        }
        public async Task<Cocktail2> SearchByIngredient(string Ingredient)
        {
            var response = await _client.GetAsync($"{_apikey}/filter.php?i={Ingredient}");
            response.EnsureSuccessStatusCode();

            var content = response.Content.ReadAsStringAsync().Result;
            var result = JsonConvert.DeserializeObject<Cocktail2>(content);
            return result;
        }
    }
}
