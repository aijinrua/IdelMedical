using IdelMedical.Database.Tables;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Infrastructure;
using Microsoft.Extensions.Configuration;
using Pomelo.EntityFrameworkCore.MySql.Storage;
using System;
using System.Collections.Generic;

namespace IdelMedical.Database
{
    public class DatabaseContext : DbContext
    {
        public IConfiguration Configuration { get; }

        public DbSet<Tables.LandingRequest> LandingRequests { get; set; }

        public DatabaseContext(IConfiguration configuration)
        {
            this.Configuration = configuration;
        }

        protected override void OnConfiguring(DbContextOptionsBuilder optionsBuilder)
        {
            optionsBuilder.UseMySql(Configuration.GetConnectionString("MySql"), MySqlConfiguringOptionBuild);
            base.OnConfiguring(optionsBuilder);
        }

        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            LandingRequest.Build(modelBuilder);
            Notice.Build(modelBuilder);
            Broadcast.Build(modelBuilder);
            IdelStar.Build(modelBuilder);
            PressCoverage.Build(modelBuilder);
            IdelTV.Build(modelBuilder);
            IdelEvent.Build(modelBuilder);
            User.Build(modelBuilder);
            base.OnModelCreating(modelBuilder);
        }

        private void MySqlConfiguringOptionBuild(MySqlDbContextOptionsBuilder builder)
        {
            builder.CharSet(CharSet.Utf8Mb4);
        }

        public static void Init(IConfiguration configuration)
        {
            using (var db = new DatabaseContext(configuration))
            {
                db.Database.EnsureCreated();
            }
        }

        public DbSet<Notice> Notices { get; set; }
        public DbSet<Broadcast> Broadcasts { get; set; }
        public DbSet<IdelStar> IdelStars { get; set; }
        public DbSet<PressCoverage> PressCoverages { get; set; }
        public DbSet<IdelTV> IdelTVs { get; set; }
        public DbSet<IdelEvent> IdelEvents { get; set; }
        public DbSet<User> Users { get; set; }
    }
}
