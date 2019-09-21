using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.EntityFrameworkCore;
using Perpustakaan.Models;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Hosting;
using Perpustakaan.Helpers;
using Perpustakaan.ViewModels;

namespace Perpustakaan.Controllers
{
    [Authorize(Roles = "Admin,Staff")]
    public class BooksController : Controller
    {
        private readonly PustakaDbContext _context;
        private IHostingEnvironment _environment;
       
        public BooksController(PustakaDbContext context, IHostingEnvironment environment)
        {
            _context = context;
            _environment = environment;
        }

        // GET: Books
        public async Task<IActionResult> Index()
        {
            var pustakaDbContext = _context.Books.Include(b => b.Category);
            return View(await pustakaDbContext.ToListAsync());
        }

        // GET: Books/Details/5
        public async Task<IActionResult> Details(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var book = await _context.Books
                .Include(b => b.Category)
                .FirstOrDefaultAsync(m => m.Id == id);
            if (book == null)
            {
                return NotFound();
            }

            return View(book);
        }

        // GET: Books/Create
        public IActionResult Create()
        {
            ViewData["CategoryId"] = new SelectList(_context.Categories, "Id", "NamaCategory");
            return View();
        }

        // POST: Books/Create
        // To protect from overposting attacks, please enable the specific properties you want to bind to, for 
        // more details see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        //public async Task<IActionResult> Create([Bind("Id,Judul,Penulis,Penerbit,Deskripsi,CategoryId,Status,Gambar")] Book book)
        public async Task<IActionResult> Create(Book book)
        {
            if (ModelState.IsValid)
            {
                var file = book.FileToUpload;
                if (file != null && file.Length > 0)
                {
                    var uploads = Path.Combine(_environment.WebRootPath, "images");
                    using (var fileStream = new FileStream(Path.Combine(uploads,
                        file.FileName), FileMode.Create))
                    {
                        await file.CopyToAsync(fileStream);
                    }
                    book.Gambar = file.FileName;
                }
                _context.Add(book);
                await _context.SaveChangesAsync();
                return RedirectToAction(nameof(Index));
            }
            ViewData["CategoryId"] = new SelectList(_context.Categories, "Id", "NamaCategory", book.CategoryId);
            return View(book);
        }

        // GET: Books/Edit/5
        public async Task<IActionResult> Edit(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var book = await _context.Books.FindAsync(id);
            if (book == null)
            {
                return NotFound();
            }
            ViewData["CategoryId"] = new SelectList(_context.Categories, "Id", "NamaCategory", book.CategoryId);
            return View(book);
        }

        // POST: Books/Edit/5
        // To protect from overposting attacks, please enable the specific properties you want to bind to, for 
        // more details see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Edit(int id, Book book)
        {
            if (id != book.Id)
            {
                return NotFound();
            }

            if (ModelState.IsValid)
            {
                try
                {
                    var file = book.FileToUpload;
                    if (file != null && file.Length > 0)
                    {
                        var uploads = Path.Combine(_environment.WebRootPath, "images");
                        using (var fileStream = new FileStream(Path.Combine(uploads,
                            file.FileName), FileMode.Create))
                        {
                            await file.CopyToAsync(fileStream);
                        }
                        book.Gambar = file.FileName;
                    }
                    _context.Update(book);
                    await _context.SaveChangesAsync();
                }
                catch (DbUpdateConcurrencyException)
                {
                    if (!BookExists(book.Id))
                    {
                        return NotFound();
                    }
                    else
                    {
                        throw;
                    }
                }
                return RedirectToAction(nameof(Index));
            }
            ViewData["CategoryId"] = new SelectList(_context.Categories, "Id", "NamaCategory", book.CategoryId);
            return View(book);
        }

        // GET: Books/Delete/5
        public async Task<IActionResult> Delete(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var book = await _context.Books
                .Include(b => b.Category)
                .FirstOrDefaultAsync(m => m.Id == id);
            if (book == null)
            {
                return NotFound();
            }

            return View(book);
        }

        // POST: Books/Delete/5
        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> DeleteConfirmed(int id)
        {
            var book = await _context.Books.FindAsync(id);
            _context.Books.Remove(book);
            await _context.SaveChangesAsync();
            return RedirectToAction(nameof(Index));
        }

        private bool BookExists(int id)
        {
            return _context.Books.Any(e => e.Id == id);
        }


        [HttpGet]
        public async Task<IActionResult> DaftarPeminjam()
        {
            try
            {
                var historyList = await (from o in _context.Orders
                    join u in _context.Users on o.Username equals u.Username
                    select new PeminjamanVM()
                    {
                        OrderId = o.OrderId,
                        Peminjam = u.Nama,
                        Status = o.Closed ? "Closed" : "Open",
                        OrderDate = o.OrderDate,
                        ReturnDate = o.ReturnDate

                    }).ToListAsync();

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
        public async Task<IActionResult> DetailPeminjaman(int id)
        {
            try
            {

                var details = await (from od in _context.OrderDetails
                    join o in _context.Orders on od.OrderId equals o.OrderId
                    join b in _context.Books on od.BookId equals b.Id
                    where o.OrderId.Equals(id)
                    select new PeminjamanDetailVM()
                    {
                        OrderId = o.OrderId,
                        BukuId = b.Id,
                        Judul = b.Judul,
                        Penulis = b.Penulis,
                        Gambar = b.Gambar,
                        Status = o.Closed
                    }).ToListAsync();

              
                HttpContext.Session.SetObjectAsJson("DetailPeminjaman", details);

                return View(details);

            }
            catch (Exception ex)
            {

                ModelState.AddModelError("", "Unable to process. " + ex.Message);
                ViewBag.Success = "Error";
            }


            return View();
        }


        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> DetailPeminjaman(string answer)
        {
            if (answer == "Save")
            {
                try
                {
                    var details = HttpContext.Session.
                        GetObjectFromJson<List<PeminjamanDetailVM>>("DetailPeminjaman");

                    var order = _context.Orders.Include(o => o.OrderDetails).
                        First(c => c.OrderId.Equals(details.First().OrderId));
                    order.Closed = true;
                    order.ReturnDate = DateTime.Now;
                    foreach (var od in order.OrderDetails)
                    {
                        var update = _context.Books.First(b => b.Id == od.BookId);
                        update.Status = true;
                    }
                    await _context.SaveChangesAsync();
                    HttpContext.Session.Remove("DetailPeminjaman");
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
                return RedirectToAction("DaftarPeminjam", "Books");
            }
            return View();
        }

    }
}
