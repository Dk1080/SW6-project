using FitnessApi.Models;
using Microsoft.EntityFrameworkCore;
using MongoDB.EntityFrameworkCore.Extensions;

namespace FitnessApi.Services
{
    public class DatabaseContext : DbContext
    {
        public DbSet<User> Users { get; set; }



        public DatabaseContext(DbContextOptions options)
            : base(options)
        {
        }

        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            base.OnModelCreating(modelBuilder);

            modelBuilder.Entity<User>();
        }

    }
}
