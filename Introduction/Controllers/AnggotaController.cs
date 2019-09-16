using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Introduction.Models;
using Microsoft.AspNetCore.Mvc;

namespace Introduction.Controllers
{
    public class AnggotaController : Controller
    {
        public IActionResult Index()
        {
            return View();
        }

        public IActionResult Create()
        {
            var model = new Anggota();
            return View(model);
        }

        [HttpPost]
        public IActionResult Create(Anggota model)
        {
            if (ModelState.IsValid)
            {
                return RedirectToAction("Index");
            }

            return View(model);
        }

    }
}