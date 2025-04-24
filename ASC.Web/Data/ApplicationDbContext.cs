using ASC.Model.Models;
using Microsoft.AspNetCore.Identity.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore;
using SixLabors.ImageSharp.PixelFormats;

namespace ASC.Web.Data
{
    public class ApplicationDbContext : IdentityDbContext
    {
        //public ApplicationDbContext(DbContextOptions<ApplicationDbContext> options)
        //    : base(options)
        //{
        //}
        public virtual DbSet<MasterDataKey> MasterDatakeys { get; set; }
        public virtual DbSet<MasterDataValue> MasterDataValues { get; set; }
        public virtual DbSet<ServiceRequest> ServiceRequests { get; set; }
        public virtual DbSet<Book> Books { get; set; }
        public virtual DbSet<Product> Products { get; set; }
        public virtual DbSet<Test> Test { get; set; }
        public ApplicationDbContext(DbContextOptions<ApplicationDbContext> options) : base(options)
        {
            Database.Migrate();
            //Database.EnsureCreated();
        }
        protected override void OnModelCreating(ModelBuilder builder)
        {
            builder.Entity<MasterDataKey>()
                .HasKey(c => new { c.PatititonKey, c.RowKey });
            builder.Entity<MasterDataValue>()
                .HasKey(c => new { c.PatititonKey, c.RowKey });
            builder.Entity<ServiceRequest>()
                .HasKey(c => new { c.PatititonKey, c.RowKey });
            base.OnModelCreating(builder);
        }
    }
}
