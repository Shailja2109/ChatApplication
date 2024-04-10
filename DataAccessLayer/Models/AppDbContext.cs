using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Identity.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore;
using ChatApplication2.DataAccessLayer.Models;
using ChatApplication2.ParameterModels;

namespace ChatApplication2.DataAccessLayer.Data
{
    public class AppDbContext : IdentityDbContext<IdentityUser>
    {
        public AppDbContext(DbContextOptions<AppDbContext> options) : base(options)
        {

        }

        //protected override void OnModelCreating(ModelBuilder builder)
        //{
        //    base.OnModelCreating(builder);
        //    SeedRoles(builder);
        //}

            //private static void SeedRoles(ModelBuilder builder)
            //{
            //    builder.Entity<IdentityRole>().HasData
            //        (
            //        new IdentityRole() { Name = "Admin", ConcurrencyStamp = "1", NormalizedName = "Admin" },
            //        new IdentityRole() { Name = "User", ConcurrencyStamp = "2", NormalizedName = "User" }
            //        );
            //}



        public DbSet<Message> Messages { get; set; }
        public DbSet<ApiLog> ApiLogs { get; set; }
        public DbSet<UserReg> UserRegs { get; set; }

    }
}
