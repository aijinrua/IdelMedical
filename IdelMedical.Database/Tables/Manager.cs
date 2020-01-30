using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Text;

namespace IdelMedical.Database.Tables
{
    /// <summary>
    /// 관리자 계정 정보
    /// </summary>
    [Table("manager")]
    public class Manager
    {
        [Key]
        public int Id { get; set; }

        [Required]
        public ManagerTypes Type { get; set; }

        [Required]
        public string UserId { get; set; }

        [Required]
        public string Passwd { get; set; }

        [Required, DatabaseGenerated(DatabaseGeneratedOption.Identity)]
        public DateTime CreateTime { get; set; }

        public static void Build(ModelBuilder modelBuilder)
        {
            var entity = modelBuilder.Entity<Manager>();
            entity.HasIndex(x => x.Type);
            entity.HasIndex(x => new { x.UserId, x.Passwd });
        }
    }
}
