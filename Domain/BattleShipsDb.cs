using System;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging;

namespace Domain
{
    public class BattleShipsDb : DbContext
    {
        public DbSet<Game> Games { get; set; } = null!;
        public DbSet<GameState> GameStates { get; set; } = null!;


        private static readonly ILoggerFactory _loggerFactory = LoggerFactory.Create(
            builder =>
            {
                builder.AddFilter("Microsoft", LogLevel.Debug).AddConsole();
            });
        protected override void OnConfiguring(DbContextOptionsBuilder optionsBuilder)
        {
            base.OnConfiguring(optionsBuilder);
            optionsBuilder
                //.UseLoggerFactory(_loggerFactory)
            .UseSqlServer(@"
                Server=barrel.itcollege.ee,1533;
                User Id=student;
                Password=xxx;
                Database=mmetsa_battleship;
                MultipleActiveResultSets=true;");
        }
    }
}