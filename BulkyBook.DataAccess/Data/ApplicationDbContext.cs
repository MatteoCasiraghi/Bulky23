using BulkyBook.Models;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Identity.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore;


namespace BulkyBook.DataAccess
{
    public class ApplicationDbContext : IdentityDbContext<IdentityUser>
    {
        //ApplicationDbContext riceverà nel costruttore le opzioni per potersi connettere al database
        public ApplicationDbContext(DbContextOptions<ApplicationDbContext> options) : base(options)
        {

        }
        public DbSet<Category> Categories { get; set; } = null!;
        public DbSet<CoverType> CoverTypes { get; set; } = null!;
        public DbSet<Product> Products { get; set; } = null!;

        public DbSet<ApplicationUser> ApplicationUsers { get; set; } = null!;

        public DbSet<Company> Companies { get; set; } = null!;
        public DbSet<ShoppingCart> ShoppingCarts { get; set; } = null!;




        protected override void OnModelCreating(ModelBuilder builder)
        {
            base.OnModelCreating(builder);
            // Customize the ASP.NET Identity model and override the defaults if needed.
            // For example, you can rename the ASP.NET Identity table names and more.
            // Add your customizations after calling base.OnModelCreating(builder);
        }
    }

}
