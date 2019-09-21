using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Perpustakaan.Helpers;
using Perpustakaan.Models;
using Perpustakaan.ViewModels;

namespace Perpustakaan.Controllers
{
    [Authorize]
    public class HomeController : Controller
    {
        private readonly PustakaDbContext _context;
        public HomeController(PustakaDbContext context)
        {
            _context = context;
        }
        public IActionResult Index()
        {
            var model = _context.Books.OrderBy(r => Guid.NewGuid()).Take(6).ToList();
            return View(model);
        }

        [HttpGet]
        public IActionResult DaftarBuku()
        {
            var model = _context.Books.
                Where(c => c.Category.NamaCategory.ToLower().Equals("teknologi")).ToList();
            return View(model);
        }

        [HttpGet]
        public IActionResult DaftarBukuByCategory(string category)
        {
            var model = _context.Books.
                Where(c => c.Category.NamaCategory.ToLower().Equals(category)).ToList();

            return PartialView("~/Views/Shared/_GetBuku.cshtml", model);

        }

        [HttpPost]
        public IActionResult AddShoppingCart(Book book)
        {

            var bookCount = 0;
            var books = HttpContext.Session.GetObjectFromJson<List<Book>>("ShoppingCart");
            string messageUser;
            try
            {
                if (book != null)
                {
                    if (books != null)
                    {
                        var isDuplicate = books.Any(c => c.Id.Equals(book.Id));
                        if (isDuplicate)
                        {
                            messageUser = @"Buku telah ada didalam keranjang anda !";
                        }
                        else
                        {
                            books.Add(book);
                            messageUser = @"Buku telah sukses dimasukkan kedalam keranjang anda !";
                        }
                    }
                    else
                    {
                        books = new List<Book> { book };
                        messageUser = "Buku telah sukses dimasukkan kedalam keranjang anda !";
                    }
                    bookCount = books.Count;
                }
                else
                {
                    messageUser = "Coba kembali, terjadi kesalahan pada sistem  !";
                }

            }
            catch (Exception ex)
            {
                messageUser = ex.Message;
            }
            HttpContext.Session.SetObjectAsJson("ShoppingCart", books);



            return Json(new
            {
                message = messageUser,
                jumlah = bookCount
            });


        }


        [HttpGet]
        public IActionResult DisplayMyOrder()
        {
            var books = HttpContext.Session.GetObjectFromJson<List<Book>>("ShoppingCart");
            return View(books);
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> DisplayMyOrder(string answer)
        {

            if (answer == "Save")
            {
                try
                {
                    var books = HttpContext.Session.GetObjectFromJson<List<Book>>("ShoppingCart");
                    var details = books.Select(b => new OrderDetail()
                    {
                        BookId = b.Id
                    }).ToList();

                    var order = new Order()
                    {
                        OrderDate = DateTime.Now,
                        Username = User.Identity.Name,
                        OrderDetails = details
                    };

                    _context.Orders.Add(order);
                    foreach (var book in order.OrderDetails)
                    {
                        var obj = _context.Books.First(b => b.Id == book.BookId);
                        obj.Status = false;
                        _context.Update(obj);
                    }
                    await _context.SaveChangesAsync();
                    HttpContext.Session.Remove("ShoppingCart");
                    ViewBag.Success = "Success";
                }

                catch (Exception ex)
                {
                    ModelState.AddModelError("", "Unable to process. " + ex.Message);
                    ViewBag.Success = "Error";
                }

            }
            else
            {
                HttpContext.Session.Remove("ShoppingCart");
                return RedirectToAction("Index");
            }
            return View();
        }

        [HttpGet]
        public async Task<IActionResult> DisplayHistory()
        {
            try
            {
                var historyList = (from od in _context.OrderDetails
                                   join o in _context.Orders on
                                       od.OrderId equals o.OrderId
                                   join b in _context.Books on
                                       od.BookId equals b.Id
                                   where o.Username.ToLower()
                                       .Equals(User.Identity.Name.ToLower())
                                   select new HistoryVM()
                                   {
                                       BukuId = od.BookId,
                                       Judul = b.Judul,
                                       Penulis = b.Penulis,
                                       Gambar = b.Gambar,
                                       Status = o.Closed ?
                                           "Sudah Kembali" :
                                           "Masih Dipinjam",
                                       OrderDate = o.OrderDate,
                                       ReturnDate = o.ReturnDate

                                   }).ToList();
                return View(historyList);



            }
            catch (Exception ex)
            {

                ModelState.AddModelError("", "Unable to process. " + ex.Message);
                ViewBag.Success = "Error";
            }


            return View();
        }

        [HttpGet]
        public async Task<IActionResult> DetailBook(int id)
        {
            try
            {
                var book = await _context.Books.Include(b => b.Category).
                    FirstOrDefaultAsync(c => c.Id == id);
                if (book != null) return View(book);
                ModelState.AddModelError("", "Buku tidak dapat ditampilkan.");
                return View();
            }
            catch (Exception ex)
            {

                ModelState.AddModelError("", "Unable to process. " + ex.Message);
            }
            return View();
        }


    }
}