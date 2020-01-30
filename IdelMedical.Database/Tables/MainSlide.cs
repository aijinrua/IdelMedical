using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Text;

namespace IdelMedical.Database.Tables
{
    [Table("mainslide")]
    public class MainSlide
    {
        [Key]
        public int Id { get; set; }

        [Required]
        public int MainId { get; set; }

        [Required]
        public bool IsMobile { get; set; }

        [Required]
        public string Url { get; set; }

        public string Link { get; set; }

        public string LinkTarget { get; set; }

        [Required]
        public int OrderIdx { get; set; }

        [Required, DatabaseGenerated(DatabaseGeneratedOption.Identity)]
        public DateTime CreateTime { get; set; }

        public Main Main { get; set; }

        public static void Build(ModelBuilder modelBuilder)
        {
            var entity = modelBuilder.Entity<MainSlide>();

            entity.HasIndex(x => x.IsMobile);

            entity
                .HasOne(x => x.Main)
                .WithMany(x => x.MainSlides)
                .HasForeignKey(x => x.MainId)
                .HasPrincipalKey(x => x.Id)
                .OnDelete(DeleteBehavior.Cascade);
        }
    }
}
