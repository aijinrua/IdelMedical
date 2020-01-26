using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Text;

namespace IdelMedical.Database.Tables
{
    [Table("counseling")]
    public class Counseling
    {
        [Key]
        public int Id { get; set; }
        public int? UserId { get; set; }
        [Required]
        public string Category { get; set; }
        [Required]
        public string Subject { get; set; }
        [Required]
        public string Substance { get; set; }
        public string Answer { get; set; }
        [Required]
        public bool IsSecret { get; set; }
        [Required]
        public string UserName { get; set; }
        [Required, DatabaseGenerated(DatabaseGeneratedOption.Identity)]
        public DateTime CreateTime { get; set; }

        public User User { get; set; }

        public static void Build(ModelBuilder modelBuilder)
        {
            var entity = modelBuilder.Entity<Counseling>();

            entity
                .HasOne(x => x.User)
                .WithMany()
                .HasPrincipalKey(x => x.Id)
                .HasForeignKey(x => x.UserId)
                .OnDelete(DeleteBehavior.SetNull);
        }
    }
}
