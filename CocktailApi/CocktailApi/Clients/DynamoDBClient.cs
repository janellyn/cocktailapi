using Amazon.DynamoDBv2;
using Amazon.DynamoDBv2.Model;
using CocktailApi.Constant;
using CocktailApi.Extensions;
using CocktailApi.Model;
using System.Net;

namespace CocktailApi.Clients
{
    public class DynamoDBClient : IDynamoDBClient, IDisposable
    {
        public string _tableName;
        private readonly IAmazonDynamoDB _dynamoDB;
        public DynamoDBClient(IAmazonDynamoDB dynamoDB) 
        {
            _dynamoDB = dynamoDB;
            _tableName = Constants.tableName;
        }
        public async Task<bool> DeleteCocktailByName(string name)
        {
            var item = new DeleteItemRequest
            {
                TableName = _tableName,
                Key = new Dictionary<string, AttributeValue>
                {
                    {"cocktail", new AttributeValue{S = name} }
                }
            };
            try
            {
                var response = await _dynamoDB.DeleteItemAsync(item);
                return response.HttpStatusCode == HttpStatusCode.OK;
            }
            catch (Exception ex)
            {
                Console.WriteLine("Here is your error \n" + ex);
                return false;
            }
        }

        public async Task<Database> GetCocktailByName(string name)
        {
            var item = new GetItemRequest
            {
                TableName = _tableName,
                Key = new Dictionary<string, AttributeValue>
                {
                    {"cocktail", new AttributeValue{S = name} }
                }
            };
            var response = await _dynamoDB.GetItemAsync(item);
            
            if (response.Item == null || !response.IsItemSet)
                return null;
            var result = response.Item.ToClass<Database>();
            return result;
        }

        public async Task<bool> PosttoDB(Database data)
        {
            var request = new PutItemRequest
            {
                TableName = _tableName,
                Item = new Dictionary<string, AttributeValue>
                {
                    {"cocktail", new AttributeValue { S = data.cocktail} },
                    {"strDrinkThumb", new AttributeValue { S = data.strDrinkThumb} }
                }
            }; 
            try
            {
                var response = await _dynamoDB.PutItemAsync(request);
                return response.HttpStatusCode == HttpStatusCode.OK;
            }
            catch (Exception ex)
            {
                Console.WriteLine("Here is your error \n" + ex);
                return false;
            }
        }

        public async Task<bool> UpdateintoDB(string name, string updatedname, Database data)
        {
            PosttoDB(data);
            DeleteCocktailByName(name);  
            return true;
        }

        public async Task<List<Database>> GetAll()
        {
            var result = new List<Database>();

            var request = new ScanRequest
            {
                TableName = _tableName,
            };
            var response = await _dynamoDB.ScanAsync(request);

            if (response.Items == null || response.Items.Count == 0) return null;

            foreach (Dictionary<string, AttributeValue> item in response.Items)
            {
                result.Add(item.ToClass<Database>());
            }
            return result;
        }

        public void Dispose()
        {
            _dynamoDB.Dispose();
        }
    }
}
