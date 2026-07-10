using Microsoft.EntityFrameworkCore;
using SunmedMedikal.Models;

namespace SunmedMedikal.Data
{
    public class ApplicationDbContext : DbContext
    {
        public ApplicationDbContext(DbContextOptions<ApplicationDbContext> options) : base(options) { }

        public DbSet<Product> Products { get; set; }
        public DbSet<Brand> Brands { get; set; }
        public DbSet<User> Users { get; set; }
        public DbSet<Order> Orders { get; set; }
        public DbSet<CartItem> CartItems { get; set; }
        
        public DbSet<MainCategory> MainCategories { get; set; }
        public DbSet<SubCategory> SubCategories { get; set; }
        public DbSet<SubSubCategory> SubSubCategories { get; set; }
        public DbSet<SubSubSubCategory> SubSubSubCategories { get; set; } 
        public DbSet<SubSubSubSubCategory> SubSubSubSubCategories { get; set; } // YENİ: 5. SEVİYE TABLO

        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            base.OnModelCreating(modelBuilder);

            modelBuilder.Entity<User>().HasData(new User { Id = 1, FullName = "Mehtap Ş. Önder", Username = "admin", Password = "sunmed123", Role = "Admin", Email = "admin@sunmedikal.com", PhoneNumber = "+905554443322", CustomerType = "Kurumsal" });

            modelBuilder.Entity<MainCategory>().HasData(
                new MainCategory { Id = 1, Name = "Kimyasallar" }, new MainCategory { Id = 2, Name = "Sarf Malzemeler" }, new MainCategory { Id = 3, Name = "Cam Malzemeler" }, new MainCategory { Id = 4, Name = "Laboratuvar Cihazları" }, new MainCategory { Id = 5, Name = "Referans Standartları" }
            );

            modelBuilder.Entity<SubCategory>().HasData(
                new SubCategory { Id = 1, MainCategory = "Kimyasallar", Name = "Analitik Kimyasallar" }, new SubCategory { Id = 2, MainCategory = "Kimyasallar", Name = "Teknik Kimyasallar" }, new SubCategory { Id = 3, MainCategory = "Cam Malzemeler", Name = "Beherler" }
            );

            modelBuilder.Entity<SubSubCategory>().HasData(
                new SubSubCategory { Id = 1, MainCategory = "Cam Malzemeler", SubCategory = "Beherler", Name = "Cam Beherler" }, new SubSubCategory { Id = 2, MainCategory = "Cam Malzemeler", SubCategory = "Beherler", Name = "Plastik Beherler" }
            );

            modelBuilder.Entity<SubSubSubCategory>().HasData(
                new SubSubSubCategory { Id = 1, MainCategory = "Cam Malzemeler", SubCategory = "Beherler", SubSubCategory = "Cam Beherler", Name = "Uzun Form Cam Beherler" }
            );
            
            // Örnek 5. Seviye Kategori
            modelBuilder.Entity<SubSubSubSubCategory>().HasData(
                new SubSubSubSubCategory { Id = 1, MainCategory = "Cam Malzemeler", SubCategory = "Beherler", SubSubCategory = "Cam Beherler", SubSubSubCategory = "Uzun Form Cam Beherler", Name = "250ml Uzun Form Cam Beherler" }
            );

            modelBuilder.Entity<Brand>().HasData(
                new Brand { Id = 1, Name = "MERCK", ImageUrl = "/images/brands/MERCKLOGO.png" }, new Brand { Id = 2, Name = "ISOLAB", ImageUrl = "/images/brands/isolablogo.png" }
            );
        }
    }
}