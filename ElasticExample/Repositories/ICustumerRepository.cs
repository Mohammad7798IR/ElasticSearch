using DapperExample.Models.Entites;
using ElasticExample.Models.AdventureWorksLT2019;

namespace ElasticExample.Repositories
{
    public interface ICustumerRepository
    {
        List<Customer> GetAllAsync();
    }
}
