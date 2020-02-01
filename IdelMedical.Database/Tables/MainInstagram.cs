using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Text;

namespace IdelMedical.Database.Tables
{
    [Table("maininstagram")]
    public class MainInstagram
    {
        [Key]
        public int Id { get; set; }

        [Required]
        public int MainId { get; set; }

        [Required]
        public string Url { get; set; }

        [Required]
        public string Link { get; set; }

        [Required]
        public string Title { get; set; }

        public string MyProperty { get; set; }

        [Required, DatabaseGenerated(DatabaseGeneratedOption.Identity)]
        public DateTime CreateTime { get; set; }

        public Main Main { get; set; }

        public static void Build(ModelBuilder modelBuilder)
        {
            var entity = modelBuilder.Entity<MainInstagram>();

            entity
                .HasOne(x => x.Main)
                .WithMany(x => x.MainInstagrams)
                .HasForeignKey(x => x.MainId)
                .HasPrincipalKey(x => x.Id)
                .OnDelete(DeleteBehavior.Cascade);
        }
    }
}
