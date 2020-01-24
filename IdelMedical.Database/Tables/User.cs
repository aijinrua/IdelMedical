using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Text;

namespace IdelMedical.Database.Tables
{
    [Table("user")]
    public class User
    {
        [Key]
        public int Id { get; set; }

        public AccountTypes AccountType { get; set; }

        [Required, MaxLength(100)]
        public string UserKey { get; set; }

        [MaxLength(100)]
        public string UserId { get; set; }

        [MaxLength(100)]
        public string Passwd { get; set; }

        [Required]
        public string Name { get; set; }

        [Required]
        public string Birthday { get; set; }

        [Required]
        public string Phone { get; set; }

        [Required]
        public bool IsReceivePhone { get; set; }

        [Required]
        public string Email { get; set; }

        [Required]
        public bool IsReceiveEmail { get; set; }

        [Required]
        public GenderTypes Gender { get; set; }

        [Required]
        public DateTime CreateTime { get; set; }

        public static void Build(ModelBuilder modelBuilder)
        {
            var entity = modelBuilder.Entity<User>();

            entity.HasIndex(nameof(User.AccountType));
            entity.HasIndex(nameof(User.UserKey)).IsUnique();
            entity.HasIndex(nameof(User.UserId)).IsUnique();
            entity.HasIndex(nameof(User.Email));
            entity.HasIndex(nameof(User.IsReceiveEmail));
            entity.HasIndex(nameof(User.Gender));
        }
    }
}
