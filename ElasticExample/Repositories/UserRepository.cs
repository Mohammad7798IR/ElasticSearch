using DapperExample.Models.Entites;
using ElasticExample.Context;
using Microsoft.EntityFrameworkCore;

namespace ElasticExample.Repositories
{
    public class UserRepository : IUserRepository
    {
        private readonly ElasticDbContext _context;

        public UserRepository(ElasticDbContext context)
        {
            _context = context;
        }

        public List<User> GetAllAsync()
        {
            return _context.Users.ToList();
        }
    }
}
