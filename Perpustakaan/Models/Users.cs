using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Threading.Tasks;

namespace Perpustakaan.Models
{
    public class Users
    {
        [Key]
        [Required]
        [Display(Name = "User name")]
        public string Username { get; set; }

        [Required]
        [Display(Name = "Nama Pengguna")]
        public string Nama { get; set; }

        [Required]
        [DataType(DataType.Password)]
        [Display(Name = "Password")]
        public string Password { get; set; }

        [Required]
        [Display(Name = "User Role")]
        public string Role { get; set; }

        public bool Status { get; set; }

        public List<Order> Orders { get; set; }

    }
}
