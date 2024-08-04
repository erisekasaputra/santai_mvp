using Microsoft.AspNetCore.Mvc.Testing;
using Catalog.API.DTOs.ItemStock;
using Microsoft.Extensions.Hosting;
using Microsoft.AspNetCore.Hosting; 
using System.Net.Http.Json;
using Microsoft.AspNetCore.StaticAssets;

namespace CatalogDataIntegrityTests
{
    [TestFixture]
    public class Tests
    {
        private WebApplicationFactory<Program> _factory;
        private HttpClient _client;

        [SetUp]
        public void SetUp()
        {
            _factory = new CustomWebApplicationFactory();
            _client = _factory.CreateClient();
        }

        [Test]
        public async Task DeductStock_RaceConditionTest()
        {
            var tasks = Enumerable.Range(0, 100).Select(_ => DeductStockRequest()).ToArray();
            var results = await Task.WhenAll(tasks);

            var successCount = results.Count(r => r.IsSuccessStatusCode);
            var failureCount = results.Length - successCount; 
        }

        private async Task<HttpResponseMessage> DeductStockRequest()
        {
            throw new Exception();
            //var request = "/api/v1/catalog/items/stock/deduct";
            //var payload = new
            //{
            //    itemStocks = new[]
            //    {
            //        new ItemStockDto("01J3NKF5GKP74FJHNBHHV33E0H", 1)
            //    }
            //};

            //return await _client.PostAsJsonAsync(request, payload);
        }

        [TearDown]
        public void TearDown()
        {
            _client.Dispose();
            _factory.Dispose();
        } 

        public class CustomWebApplicationFactory : WebApplicationFactory<Program>
        {
            protected override IHost CreateHost(IHostBuilder builder)
            {
                builder.ConfigureWebHost(webHostBuilder =>
                {
                    webHostBuilder.UseEnvironment("Testing");
                });

                return base.CreateHost(builder);
            }
        }
    }
}
