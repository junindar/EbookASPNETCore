using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace Perpustakaan.ViewModels
{
    public class HistoryVM
    {
        public int BukuId { get; set; }
        public string Judul { get; set; }
        public string Penulis { get; set; }
        public string Gambar { get; set; }
        public DateTime OrderDate { get; set; }
        public DateTime? ReturnDate { get; set; }
        public string Status { get; set; }
    }
}
