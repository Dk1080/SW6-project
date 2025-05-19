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
        public DbSet<HealthInfo> HealthInfo { get; set; }   
        public DbSet<UserPreferences> UserPreferences { get; set; }


        public DatabaseContext(DbContextOptions options)
            : base(options)
        {
        }

        protected override void OnConfiguring(DbContextOptionsBuilder optionsBuilder)
        {
            // Ensure MongoDB configuration is already set (e.g., via AddDbContext)
            base.OnConfiguring(optionsBuilder);

            // Disable transactions
            Database.AutoTransactionBehavior = AutoTransactionBehavior.Never;
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


            modelBuilder.Entity<HealthInfo>()
                .ToCollection("HealthInfo");

            modelBuilder.Entity<UserPreferences>()
                .ToCollection("UserPreferences");

        }

    }
}
