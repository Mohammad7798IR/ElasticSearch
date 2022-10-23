using ElasticExample.Models;
using ElasticExample.Models.AdventureWorksLT2019;
using ElasticExample.Repositories;
using Nest;

namespace ElasticExample.Services
{
    public class ElasticService
    {
        private readonly ElasticClient _elasticClient;

        private readonly ICustumerRepository _custumerRepository;

        private readonly string _indexName;

        public ElasticService
            (ElasticClient elasticClient, ICustumerRepository custumerrepository)
        {
            _indexName = "customer_company";

            _elasticClient = elasticClient;

            _custumerRepository = custumerrepository;

            elasticClient.Indices
                  .Create(_indexName, index => index
                  .Settings(a => a.NumberOfShards(1).NumberOfReplicas(1).Analysis(Analysis))
                  .Map<Customer>(MapCustomers));
        }

        public void AddUsers()
        {
            var settings = new ConnectionSettings(new Uri("http://elasticsearchengine:9200"));

            var pingResponse = _elasticClient.Ping(new PingRequest());

            var data = _custumerRepository.GetAllAsync();

            var result = _elasticClient.Bulk(a => a
             .Index(_indexName)
             .IndexMany(data));
        }

        public async Task DeleteIndex()
        {
            await _elasticClient.Indices.DeleteAsync(_indexName);
        }

        public List<ElasticSuggestViewModel> SuggestAsync(string query)
        {
            var result = _elasticClient.Search<Customer>(a =>
                  a.Index(_indexName)
                   .Source(sf =>
                           sf.Includes(f => f
                                 .Field(a => a.CompanyName)
                                 .Field(a => a.Title)))

                   .Suggest(su =>
                            su.Completion("custumers-suggestions", c =>
                                                         c.Prefix(query)
                                                         .Field(a => a.Suggest)
                                                         .SkipDuplicates())));

            return result.Suggest["custumers-suggestions"]
                     .FirstOrDefault()?
                     .Options
                     .Select(suggest => new ElasticSuggestViewModel
                     {
                         Content = (!string.IsNullOrWhiteSpace(suggest.Source.CompanyName)
                                   ? suggest.Source.CompanyName
                                   : string.Empty),

                         Key = suggest.Source.CustomerId
                     })
                     .ToList();
        }

        private static TypeMappingDescriptor<Customer> MapCustomers(TypeMappingDescriptor<Customer> map) => map
                .AutoMap()
                .Properties(ps => ps
                    .Text(t => t
                        .Name(p => p.CompanyName)
                        .Analyzer("custumers-analyzer")
                        .Fields(f => f
                            .Text(p => p
                                .Name("keyword")
                                .Analyzer("custumers-keyword")
                            )
                            .Keyword(p => p
                                .Name("raw")
                            )
                        )
                    )
                    .Completion(c => c
                        .Name(p => p.Suggest)));

        private static AnalysisDescriptor Analysis(AnalysisDescriptor analysis) => analysis
                .Tokenizers(tokenizers => tokenizers
                    .Pattern("custumers-tokenizer", p => p.Pattern(@"\W+"))
                )
                .TokenFilters(tokenFilters => tokenFilters
                    .WordDelimiter("custumers-words", w => w
                        .SplitOnCaseChange()
                        .PreserveOriginal()
                        .SplitOnNumerics()
                        .GenerateNumberParts(false)
                        .GenerateWordParts()
                    )
                )
                .Analyzers(analyzers => analyzers
                    .Custom("custumers-analyzer", c => c
                        .Tokenizer("custumers-tokenizer")
                        .Filters("custumers-words", "lowercase")
                    )
                    .Custom("custumers-keyword", c => c
                        .Tokenizer("keyword")
                        .Filters("lowercase")
                    ));
    }
}