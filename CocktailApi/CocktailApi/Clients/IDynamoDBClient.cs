using CocktailApi.Model;

namespace CocktailApi.Clients
{
    public interface IDynamoDBClient
    {
        public Task<Database> GetCocktailByName(string name);
        public Task<bool> DeleteCocktailByName(string name);
        public Task<bool> PosttoDB(Database data);
        public Task<bool> UpdateintoDB(string name, string updatedname, Database data);
        public Task<List<Database>> GetAll();
    }
}
