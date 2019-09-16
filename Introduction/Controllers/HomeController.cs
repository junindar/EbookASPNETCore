using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Introduction.IService;
using Microsoft.AspNetCore.Mvc;

// For more information on enabling MVC for empty projects, visit https://go.microsoft.com/fwlink/?LinkID=397860

namespace Introduction.Controllers
{
    public class HomeController : Controller
    {
        private readonly IBookRepository _bookRepository;

        public HomeController(IBookRepository bookRepository)
        {
            _bookRepository = bookRepository;
        }

        // GET: /<controller>/
        public IActionResult Index()
        {
            ViewBag.Title = "Daftar Buku";
            var books = _bookRepository.GetAllBooks().OrderBy(b => b.Judul);
            return View(books);
        }

        public  IActionResult DetailBook(int id)
        {
            var book =  _bookRepository.GetBookById(id);
            if (book == null)
            {
                
                return NotFound();
            }

            return View(book);
        }

    }
}
