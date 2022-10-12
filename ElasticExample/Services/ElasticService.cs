using DapperExample.Models.Entites;
using ElasticExample.Models;
using ElasticExample.Repositories;
using Nest;


namespace ElasticExample.Services
{
    public class ElasticService
    {
        private readonly ElasticClient _elasticClient;

        private readonly IUserRepository _userRepository;

        private readonly string _indexName;

        public ElasticService
            (ElasticClient elasticClient, IUserRepository userRepository)
        {
            _indexName = "example_users4";

            _elasticClient = elasticClient;

            _userRepository = userRepository;

            elasticClient.Indices
                  .Create(_indexName, index => index
                  .Settings(a => a.NumberOfShards(1).NumberOfReplicas(1).Analysis(Analysis))
                  .Map<User>(MapUsers));
        }


        public void AddUsers()
        {

            var data = _userRepository.GetAllAsync();

            _elasticClient.Bulk(b => b
               .Index(_indexName)
               .IndexMany(data));

        }

        public List<ElasticSuggestViewModel> SuggestAsync(string query)
        {
            var result = _elasticClient.Search<User>(a =>
                  a.Index(_indexName)
                   .Source(sf =>
                           sf.Includes(f => f
                                 .Field(a => a.UserName)
                                 .Field(a => a.Id)))

                   .Suggest(su =>
                            su.Completion("users-suggestions", c =>
                                                         c.Prefix(query)
                                                         .Field(a => a.Suggest)
                                                         .SkipDuplicates())));



            return result.Suggest["users-suggestions"]
         .FirstOrDefault()?
         .Options
         .Select(suggest => new ElasticSuggestViewModel
         {
             Content = (!string.IsNullOrWhiteSpace(suggest.Source.UserName)
                       ? suggest.Source.UserName
                       : string.Empty),

             Key = suggest.Source.Id


         })
         .ToList();
        }


        private static TypeMappingDescriptor<User> MapUsers(TypeMappingDescriptor<User> map) => map
                .AutoMap()
                .Properties(ps => ps
                    .Text(t => t
                        .Name(p => p.UserName)
                        .Analyzer("users-analyzer")
                        .Fields(f => f
                            .Text(p => p
                                .Name("keyword")
                                .Analyzer("users-keyword")
                            )
                            .Keyword(p => p
                                .Name("raw")
                            )
                        )
                    )
                    .Completion(c => c
                        .Name(p => p.Suggest)
                    ));

        private static AnalysisDescriptor Analysis(AnalysisDescriptor analysis) => analysis
                .Tokenizers(tokenizers => tokenizers
                    .Pattern("users-tokenizer", p => p.Pattern(@"\W+"))
                )
                .TokenFilters(tokenFilters => tokenFilters
                    .WordDelimiter("users-words", w => w
                        .SplitOnCaseChange()
                        .PreserveOriginal()
                        .SplitOnNumerics()
                        .GenerateNumberParts(false)
                        .GenerateWordParts()
                    )
                )
                .Analyzers(analyzers => analyzers
                    .Custom("users-analyzer", c => c
                        .Tokenizer("users-tokenizer")
                        .Filters("users-words", "lowercase")
                    )
                    .Custom("users-keyword", c => c
                        .Tokenizer("keyword")
                        .Filters("lowercase")
                    ));

    }
}
