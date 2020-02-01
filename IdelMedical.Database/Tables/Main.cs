using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Text;

namespace IdelMedical.Database.Tables
{
    [Table("main")]
    public class Main
    {
        [Key]
        public int Id { get; set; }

        public bool IsShowSnowEffect { get; set; }

        public IEnumerable<MainSlide> MainSlides { get; set; }

        public IEnumerable<MainInstagram> MainInstagrams { get; set; }
    }
}
