using BulkyBook.DataAccess.Repository.BulkyBook.DataAccess.Repository;
using BulkyBook.DataAccess.Repository.IRepository;
using BulkyBook.Models;

namespace BulkyBook.DataAccess.Repository
{
    public class ApplicationUserRepository : Repository<ApplicationUser>, IApplicationUserRepository
    {

        public ApplicationUserRepository(ApplicationDbContext db) : base(db)
        {

        }
    }
}
