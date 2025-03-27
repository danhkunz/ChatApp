using ChatApp.Models;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Diagnostics;
using Serilog;

namespace ChatApp.DBContext
{
    public class ChatContext : DbContext
    {
        public ChatContext(DbContextOptions<ChatContext> options) : base(options) { }

        private readonly string _dbPath;

        public ChatContext(string dbPath)
        {
            _dbPath = dbPath;
        }

        protected override void OnConfiguring(DbContextOptionsBuilder optionsBuilder)
        {
            optionsBuilder
                          .LogTo(x => Log.Logger.Debug(x),
                                    events: new[]
                                    {
                                        RelationalEventId.CommandExecuted,
                                    })
                          .EnableDetailedErrors()
                          .EnableSensitiveDataLogging();

            if (!optionsBuilder.IsConfigured)
            {
                optionsBuilder.UseSqlite($"Data Source={_dbPath}");
            }
        }

        public DbSet<Message> Messages { get; set; }
        public DbSet<User> Users { get; set; }
    }
}