﻿using EnterpriseWeb.Models;
using System.ComponentModel.DataAnnotations.Schema;

namespace Enterprise_Web.Models
{
    public class Idea : Common
    {
        public int Id { get; set; }
        public string? Title { get; set; }
        public string? Content { get; set; }
        public int Views { get; set; }
        public string? Image { get; set; }
        public bool IsAnonymos { get; set; }
        public int AcademicYearId { get; set; }
        public AcademicYear? AcademicYear { get; set; }
        public int CategoryId { get; set; }
        public Category? Category { get; set; }
        public int UserId { get; set; }
        public User? User { get; set; }
        public ICollection<Reaction>? Reactions { set; get; }
        public ICollection<Comment>? Comments { get; set; }
        [NotMapped]
        public IFormFile? File { get; set; }
    }
}
