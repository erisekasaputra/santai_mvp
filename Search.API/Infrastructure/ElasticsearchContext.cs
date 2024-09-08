using Core.Configurations;
using Elastic.Clients.Elasticsearch;
using Elastic.Transport;
using Microsoft.Extensions.Options; 

namespace Search.API.Infrastructure; 

public class ElasticsearchContext
{
    // connecting to elastic search documentation https://www.elastic.co/guide/en/elasticsearch/client/net-api/current/connecting.html
    /*
         var settings = new ElasticsearchClientSettings(new Uri("https://localhost:9200"))
            .CertificateFingerprint("<FINGERPRINT>")
            .Authentication(new BasicAuthentication("<USERNAME>", "<PASSWORD>"));

         var client = new ElasticsearchClient(settings);
     */
    private readonly ElasticsearchClient _client;

    public ElasticsearchClient Client => _client;

    public ElasticsearchContext(IOptions<ElasticsearchConfiguration> options)
    {
        ArgumentNullException.ThrowIfNull(options.Value);

        var addresses = options.Value.Addresses.Select(address => new Uri(address)).ToList();

        var pool = new StaticNodePool(addresses);

        var setting = new ElasticsearchClientSettings(pool).RequestTimeout(TimeSpan.FromSeconds(10));

        _client = new ElasticsearchClient(setting);
    }
}
