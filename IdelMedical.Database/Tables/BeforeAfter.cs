using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Text;

namespace IdelMedical.Database.Tables
{
    [Table("beforeafter")]
    public class BeforeAfter
    {
        [Key]
        public int Id { get; set; }

        [Required]
        public string Category { get; set; }

        [Required]
        public string Title { get; set; }

        [Required]
        public string Comment { get; set; }

        [Required]
        public string BeforeFront { get; set; }

        [Required]
        public string BeforeHalf { get; set; }

        [Required]
        public string BeforeSide { get; set; }

        [Required]
        public string AfterFront { get; set; }

        [Required]
        public string AfterHalf { get; set; }

        [Required]
        public string AfterSide { get; set; }

        [Required, DatabaseGenerated(DatabaseGeneratedOption.Identity)]
        public DateTime CreateTime { get; set; }



        public static void Build(ModelBuilder modelBuilder)
        {
            var entity = modelBuilder.Entity<BeforeAfter>();
            entity.HasIndex(x => x.Category);
            entity.HasIndex(x => x.Title);
        }
    }
}
