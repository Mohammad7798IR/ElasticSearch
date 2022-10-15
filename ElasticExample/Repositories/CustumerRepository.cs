using DapperExample.Models.Entites;
using ElasticExample.Context;
using ElasticExample.Models.AdventureWorksLT2019;
using Microsoft.EntityFrameworkCore;

namespace ElasticExample.Repositories
{
        public class CustumerRepository : ICustumerRepository
        {
                private readonly ElasticDbContext _context;

                public CustumerRepository(ElasticDbContext context)
                {
                        _context = context;
                }

                public List<Customer> GetAllAsync()
                {
                        //  return _context.Users.ToList();
                        return _context.Customers.ToList();
                }
        }
}
