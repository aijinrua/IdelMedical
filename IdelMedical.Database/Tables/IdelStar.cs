using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Text;

namespace IdelMedical.Database.Tables
{
    [Table("idelstar")]
    public class IdelStar
    {
        [Key]
        public int Id { get; set; }

        [Required, MaxLength(150)]
        public string Subject { get; set; }
        [Required]
        public string ThumbnailPC { get; set; }
        [Required]
        public string ThumbnailMobile { get; set; }
        public string ContentPC { get; set; }
        public string ContentMobile { get; set; }
        public string Link { get; set; }
        public string LinkTarget { get; set; }
        public string VideoLink { get; set; }
        [Required]
        public DateTime CreateTime { get; set; }

        public static void Build(ModelBuilder modelBuilder)
        {
            var entity = modelBuilder.Entity<IdelStar>();
            entity.HasIndex(nameof(IdelStar.Subject));
        }
    }
}
