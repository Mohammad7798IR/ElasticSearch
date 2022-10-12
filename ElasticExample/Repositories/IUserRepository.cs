using DapperExample.Models.Entites;

namespace ElasticExample.Repositories
{
    public interface IUserRepository
    {
        List<User> GetAllAsync();
    }
}
