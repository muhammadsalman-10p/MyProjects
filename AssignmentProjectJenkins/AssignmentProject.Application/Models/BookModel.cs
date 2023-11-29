using System;
using System.ComponentModel.DataAnnotations;

namespace AssignmentProject.Application.Models
{
    public class BookModel
    {
        public int Id { get; set; }
        [Required]
        [MaxLength(250)]
        public string Name { get; set; }
        [MaxLength(250)]
        public string Author { get; set; }
       // [Display(Name="Published Date")]
        public DateTime PublishedDate { get; set; }
    }
}
