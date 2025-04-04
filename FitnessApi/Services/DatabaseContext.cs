using FitnessApi.Models;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.AI;
using MongoDB.Driver;
using MongoDB.EntityFrameworkCore.Extensions;
using System.Text.Json;

namespace FitnessApi.Services
{
    public class DatabaseContext : DbContext
    {
        public DbSet<User> Users { get; set; }
        public DbSet<ChatHistory> ChatHistories { get; set; }
        public DbSet<ChartData> ChartData { get; set; }


        public DatabaseContext(DbContextOptions options)
            : base(options)
        {
        }

        public static DatabaseContext Create(IMongoDatabase database) =>
        new(new DbContextOptionsBuilder<DatabaseContext>()
                                        .UseMongoDB(database.Client, database.DatabaseNamespace.DatabaseName)
                                        .Options);


        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            base.OnModelCreating(modelBuilder);

            modelBuilder.Entity<User>()
                .ToCollection("Users");

            modelBuilder.Entity<ChatHistory>()
                .ToCollection("ChatHistories");

            modelBuilder.Entity<ChartData>()
                .ToCollection("ChartData"); 




        }

    }
}
