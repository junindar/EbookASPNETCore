using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc.Rendering;

namespace Introduction.Models
{
    public class Anggota
    {
        [Display(Name = "Nama Lengkap")]
        [Required(ErrorMessage = "Field Nama Lengkap harus diisi")]
        [StringLength(150, MinimumLength = 3)]
        public string Nama { get; set; }
        [RegularExpression(@"[A-Za-z0-9._%+-]+@[A-Za-z0-9.-]+\.[A-Zaz]{2,4}")]
        public string Email { get; set; }

        [Compare("Email")]
        public string EmailConfirm { get; set; }

        public string Alamat { get; set; }
        [DataType(DataType.Date)]
        public DateTime? TanggalLahir { get; set; }

        [Range(17, 65)]
        public int Umur { get; set; }

        public List<SelectListItem> ListAgama { get; set; }
            = new List<SelectListItem>
            {
                new SelectListItem("Islam", "Islam"),
                new SelectListItem("Protestan", "Protestan"),
                new SelectListItem("Katolik", "Katolik"),
                new SelectListItem("Hindu", "Hindu"),
                new SelectListItem("Buddha", "Buddha"),
                new SelectListItem("Kong Hu Cu", "Kong Hu Cu")
            }; 
        public string Agama { get; set; }

        [DataType(DataType.Password)]
        public string Password { get; set; }

        public string JenisKelamin { get; set; }
        public bool Status { get; set; }

        [DisplayFormat(ApplyFormatInEditMode = true, DataFormatString= "{0:c}")]
        public decimal Penghasilan { get; set; }

    }
}
