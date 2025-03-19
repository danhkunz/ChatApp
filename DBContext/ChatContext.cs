

using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Diagnostics;
using Serilog;

namespace ChatApp.DBContext
{
    public class ChatContext : DbContext{
        public ChatContext(DbContextOptions<ChatContext> options):  base(options){}

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
        }
    }
}