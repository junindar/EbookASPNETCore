using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Http;

namespace Perpustakaan.Models
{
  
        public class Book
        {
            public int Id { get; set; }
            [Required]
            [Display(Name = "Judul Buku")]
            public string Judul { get; set; }

            [Required]
            [Display(Name = "Nama Penulis")]
            public string Penulis { get; set; }

            [Required]
            [Display(Name = "Nama Penerbit")]
            public string Penerbit { get; set; }

            [DataType(DataType.MultilineText)]
            public string Deskripsi { get; set; }

            [Display(Name = "Kategori")]
            public int CategoryId { get; set; }


            [Display(Name = "Status Buku")]
            public bool Status { get; set; }

            [DisplayName("Upload Buku")]
            public string Gambar { get; set; }

            [NotMapped]
            public IFormFile FileToUpload { get; set; }

            [Display(Name = "Category")]
            public virtual Category Category { get; set; }
        }
    
}
