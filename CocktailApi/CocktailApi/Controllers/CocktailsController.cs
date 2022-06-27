using Amazon.DynamoDBv2;
using Amazon.DynamoDBv2.Model;
using CocktailApi.Clients;
using CocktailApi.Extensions;
using CocktailApi.Model;
using Microsoft.AspNetCore.Mvc;

namespace CocktailApi.Controllers
{

    [ApiController]
    [Route("[controller]")]
    public class CocktailsController : ControllerBase
    {
        private readonly ILogger<CocktailsController> _logger;
        private readonly CocktailsClient _cocktailClient;
        private readonly IDynamoDBClient _dynamoDBClient;

        public CocktailsController(ILogger<CocktailsController> logger, CocktailsClient cocktailClient, IDynamoDBClient dynamoDBClient) 
        {
            _logger = logger;
            _cocktailClient = cocktailClient;
            _dynamoDBClient = dynamoDBClient;
        }

        [HttpGet("SearchCocktailByName")]
        public async Task<Cocktail> SearchCocktailByName([FromQuery] DrinkParameter parametres)
        {
            var result = await _cocktailClient.SearchCocktailByName(parametres.drink); 
            return result;
        }

        [HttpGet("SearchIngredientByName")]
        public async Task<Ingredients> SearchIngredientByName([FromQuery] IngredientParameter parametres)
        {
            var result = await _cocktailClient.SearchIngredientByName(parametres.ingredient);
            return result;
        }

        [HttpGet("LookupARandomCocktail")]
        public async Task<Cocktail> LookupARandomCocktail()
        {
            var result = await _cocktailClient.LookupARandomCocktail();
            return result;
        }

        [HttpGet("SearchByIngredient")]
        public async Task<Cocktail2> SearchByIngredient([FromQuery] IngredientParameter parametres)
        {
            var result = await _cocktailClient.SearchByIngredient(parametres.ingredient);
            return result;
        }

        [HttpGet("GetFromDatabase")]
        [ProducesResponseType(StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status404NotFound)]
        public async Task<IActionResult> GetCocktail([FromQuery] string name)
        {
            var result = await _dynamoDBClient.GetCocktailByName(name);
            if (result == null) return NotFound();
            var cocktailresponse = new Drinks2
            {
                strDrink = result.cocktail,
                strDrinkThumb = result.strDrinkThumb,
            };
            return Ok(cocktailresponse);
        }

        [HttpPost("AddToFavorites")]
        public async Task<IActionResult> AddToFavorites([FromQuery] Drinks3 drink)
        {
            var data = new Database
            {
                cocktail = drink.name,
                strDrinkThumb = ""
            };
            var result = await _dynamoDBClient.PosttoDB(data);
            if (result == false)
            {
                return BadRequest("Cannot insert value to database");
            }
            return Ok("Succesfully added");
        }

        [HttpGet("GetAllFavorites")] 
        public async Task<IActionResult> GetAll()
        {
            var response = await _dynamoDBClient.GetAll();

            if (response == null) return NotFound("There are no records in database");

            var result = response
                .Select(x => new Drinks2()
                {
                    strDrink = x.cocktail,
                    strDrinkThumb = x.strDrinkThumb
                }).ToList();
            return Ok(result);
        }

        [HttpDelete("DeleteCocktail")]
        public async Task<IActionResult> DeleteCocktailByName([FromQuery] string name)
        {
            await _dynamoDBClient.DeleteCocktailByName(name);
            return Ok();
        }

        [HttpPut("UpdateCocktail")]
        public async Task<IActionResult> UpdateintoDB([FromQuery] string name, string updatedname)
        {
            var data = new Database
            {
                cocktail = updatedname,
                strDrinkThumb = ""
            };

            if (data.cocktail == name) await _dynamoDBClient.UpdateintoDB(name, updatedname, data);
            await _dynamoDBClient.PosttoDB(data);
            await _dynamoDBClient.DeleteCocktailByName(name);
            return Ok();
        }
    }
}
