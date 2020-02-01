using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Text;

namespace IdelMedical.Database.Tables
{
    [Table("popupnotice")]
    public class PopupNotice
    {
        [Key]
        public int Id { get; set; }

        [Required]
        public string Url { get; set; }

        public string Link { get; set; }

        public string LinkTarget { get; set; }

        [Required]
        public int OrderIdx { get; set; }

        [Required, DatabaseGenerated(DatabaseGeneratedOption.Identity)]
        public DateTime CreateTime { get; set; }
    }
}
