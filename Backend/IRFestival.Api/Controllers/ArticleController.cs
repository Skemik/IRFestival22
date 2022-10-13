using System.Configuration;
using System.Net;
using IRFestival.Api.Domain;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Azure.Cosmos;
using Microsoft.Azure.Cosmos.Linq;

namespace IRFestival.Api.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class ArticleController : ControllerBase
    {
        private CosmosClient _cosmosClient;
        private Container _WebsiteArticlesContainer;
        private IConfiguration _configuration;

        public ArticleController(IConfiguration configuration)
        {
            _cosmosClient = new CosmosClient(configuration.GetConnectionString("CosmosConnection"));
            _WebsiteArticlesContainer = _cosmosClient.GetContainer("IRFestivalArticles", "WebsiteArticles");
            _configuration= configuration;
        }

        [HttpPost]
        public async Task<ActionResult> CreateItemAsync(Article dummy)
        {
            await _cosmosClient.CreateDatabaseIfNotExistsAsync("IRFestivalArticles");
            await _cosmosClient.GetDatabase("IRFestivalArticles").CreateContainerIfNotExistsAsync(
                new ContainerProperties() { Id = "WebsiteArticles", PartitionKeyPath = "/tag", 
                    IndexingPolicy = new IndexingPolicy() { Automatic = false, IndexingMode = IndexingMode.Lazy, } });
            if (dummy != null)
            {
                return Ok(await _WebsiteArticlesContainer.CreateItemAsync(dummy));
            }
            else return NoContent();
        }

        [HttpGet]
        [ProducesResponseType((int)HttpStatusCode.OK, Type = typeof(Article))]
        public async Task<IActionResult> GetAsync()
        {
            var result = new List<Article>();
            var queryDefinition = _WebsiteArticlesContainer.GetItemLinqQueryable<Article>()
                .Where(p => p.Status == nameof(Status.Published))
                .OrderBy(p => p.Date);
            var iterator = queryDefinition.ToFeedIterator();
            while (iterator.HasMoreResults)
            {
                var response = await iterator.ReadNextAsync();
                result = response.ToList();
            }

            return Ok(result);
        }

     
    }
}
