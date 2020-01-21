using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Text;

namespace IdelMedical.Database.Tables
{
    [Table("ideltv")]
    public class IdelTV
    {
        [Key]
        public int Id { get; set; }

        [Required, MaxLength(150)]
        public string Subject { get; set; }
        [Required, MaxLength(150)]
        public string SubjectComment { get; set; }
        [Required]
        public string Thumbnail { get; set; }
        [Required]
        public string VideoLink { get; set; }
        [Required]
        public DateTime CreateTime { get; set; }

        public static void Build(ModelBuilder modelBuilder)
        {
            var entity = modelBuilder.Entity<IdelTV>();
            entity.HasIndex(nameof(IdelTV.Subject));
            entity.HasIndex(nameof(IdelTV.SubjectComment));
        }
    }
}
