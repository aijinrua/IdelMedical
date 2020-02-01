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
            Counseling.Build(modelBuilder);
            Reservation.Build(modelBuilder);
            BeforeAfter.Build(modelBuilder);
            Manager.Build(modelBuilder);
            MainSlide.Build(modelBuilder);
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
                if (db.Database.EnsureCreated())
                {
                    db.Mains.Add(new Main
                    {
                        IsShowSnowEffect = false
                    });

                    db.Managers.Add(new Manager
                    {
                        CreateTime = DateTime.Now,
                        Passwd = "manager",
                        Type = ManagerTypes.Root,
                        UserId = "rootmanager"
                    });

                    db.SaveChanges();

                    db.Managers.Add(new Manager
                    {
                        CreateTime = DateTime.Now,
                        Passwd = "counseler",
                        Type = ManagerTypes.Counseler,
                        UserId = "counseler"
                    });

                    db.SaveChanges();

                    db.Managers.Add(new Manager
                    {
                        CreateTime = DateTime.Now,
                        Passwd = "marketer",
                        Type = ManagerTypes.Marketer,
                        UserId = "marketer"
                    });

                    db.SaveChanges();
                }
            }
        }

        public DbSet<Notice> Notices { get; set; }
        public DbSet<Broadcast> Broadcasts { get; set; }
        public DbSet<IdelStar> IdelStars { get; set; }
        public DbSet<PressCoverage> PressCoverages { get; set; }
        public DbSet<IdelTV> IdelTVs { get; set; }
        public DbSet<IdelEvent> IdelEvents { get; set; }
        public DbSet<User> Users { get; set; }
        public DbSet<Counseling> Counselings { get; set; }
        public DbSet<Reservation> Reservations { get; set; }
        public DbSet<BeforeAfter> BeforeAfters { get; set; }
        public DbSet<Manager> Managers { get; set; }
        public DbSet<Main> Mains { get; set; }
        public DbSet<MainSlide> MainSlides { get; set; }
        public DbSet<ModelGathering> ModelGatherings { get; set; }
        public DbSet<PopupNotice> PopupNotices { get; set; }
    }
}