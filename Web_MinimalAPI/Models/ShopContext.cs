using Microsoft.EntityFrameworkCore;

namespace Web_MinimalAPI.Models
{
    public class ShopContext : DbContext
    {
        //ShopContext of DbcontextOptions
        public ShopContext(DbContextOptions<ShopContext> options): base(options) { }

        //fill sample data from the ModelBuilderExtension file
        //call when model creating event takes place
        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            //defining relationship between entities
            //products belong to category, and catgeory can have n no.s of product
            modelBuilder.Entity<Category>().HasMany(c => c.Products).WithOne(a=>a.Category).HasForeignKey(a=>a.CategoryId);

            modelBuilder.Seed();
        }


        public DbSet<Product> Products { get; set; }
        public DbSet<Category> Categories { get; set; }
    }
}
