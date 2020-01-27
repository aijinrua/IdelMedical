using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Text;

namespace IdelMedical.Database.Tables
{
    [Table("reservation")]
    public class Reservation
    {
        [Key]
        public int Id { get; set; }

        [MaxLength(100)]
        public string Category { get; set; }

        public DateTime ReservationTime { get; set; }

        [MaxLength(100)]
        public string Name { get; set; }

        [MaxLength(100)]
        public string Phone { get; set; }

        public bool UseCall { get; set; }

        public bool IsFirst { get; set; }

        public string Channel { get; set; }

        public string Content { get; set; }

        public string Memo { get; set; }

        public DateTime CreateTime { get; set; }

        public static void Build(ModelBuilder modelBuilder)
        {
            var entity = modelBuilder.Entity<Reservation>();
            entity.HasIndex(x => x.Category);
            entity.HasIndex(x => x.Name);
            entity.HasIndex(x => x.Phone);
        }
    }
}
