using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Text;

namespace AssignmentProject.Core.Entities
{
    public class Book
    {
        [Key]
        public int Id { get; set; }
        [Required]
        [Column(TypeName = "varchar(250)")]
        public string Name { get; set; }
        [Column(TypeName = "varchar(250)")]
        public string Author { get; set; }
        public DateTime PublishedDate { get; set; }
    }
}
