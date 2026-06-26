using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Web;

namespace StudentManagementSystem.Models
{
    public class Mark
    {
        [Key]
        public int MarkId { get; set; }

        [Required]
        public int StudentId { get; set; }

        public virtual Student Student { get; set; }

        [Required]
        public int ModuleId { get; set; }

        public virtual Module Module { get; set; }

        [Required]
        [Range(0, 100)]
        public int Score { get; set; }
    }
}