using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Threading.Tasks;

namespace Introduction.Models
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
        public string Penerbit { get; set; }
        [DataType(DataType.MultilineText)]
        public string Deskripsi { get; set; }
        [Display(Name = "Status Buku")]
        public bool Status { get; set; }
        public bool IsAvailable { get; set; }
        public string Gambar { get; set; }

    }

}
