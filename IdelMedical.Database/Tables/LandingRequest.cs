using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Text;

namespace IdelMedical.Database.Tables
{
    [Table(nameof(LandingRequest))]
    public class LandingRequest
    {
        [Key]
        public int Id { get; set; }

        [Required, StringLength(10, MinimumLength = 1)]
        public string UserName { get; set; }

        [Required, StringLength(maximumLength: 13, MinimumLength = 12)]
        public string Phone { get; set; }

        [Required]
        public string Question { get; set; }

        public string Memo { get; set; }

        [DatabaseGenerated(DatabaseGeneratedOption.Identity)]
        public DateTime CreateTime { get; set; }

        public static void Build(ModelBuilder modelBuilder)
        {
            var entity = modelBuilder.Entity<LandingRequest>();

            entity.HasIndex(nameof(LandingRequest.Phone))
                  .IsUnique(true);
        }
    }
}
