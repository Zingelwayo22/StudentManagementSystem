using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Web;

namespace StudentManagementSystem.Models
{
    public class Module
    {
        [Key]
        public int ModuleId { get; set; }

        [Required]
        public string ModuleCode { get; set; }

        [Required]
        public string ModuleName { get; set; }

        public int Credits { get; set; }

        // Lecturer assigned to module
        public int? LecturerId { get; set; }

        public virtual Lecturer Lecturer { get; set; }
    }
}