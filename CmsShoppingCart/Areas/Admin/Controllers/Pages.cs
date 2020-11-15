using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using CmsShoppingCart.Infrastructure;
using CmsShoppingCart.Models;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

namespace CmsShoppingCart.Areas.Admin.Controllers
{
    [Area("Admin")]
    public class Pages : Controller
    {
        private readonly CmsShoppingCartContext contex;
        public Pages(CmsShoppingCartContext contex)
        {
            this.contex = contex;
        }

        //GET /admin/pages
        public async Task<IActionResult> Index()
        {
            IQueryable<Page> pages = from p in contex.Pages orderby p.Sorting select p;

            List<Page> pageList = await pages.ToListAsync();

            return View(pageList);
         }

        //GET /admin/pages/details/5
        public async Task<IActionResult> Details(int id)
        {
            Page page = await contex.Pages.FirstOrDefaultAsync(x => x.Id == id);

            if (page == null)
            {
                return NotFound();
            }

            return View(page);
        }

        //GET /admin/pages/create
        public IActionResult Create() => View();

        //POST /admin/pages/create
        [HttpPost]
        [ValidateAntiForgeryToken] //protect against CSRF attacks 
        public async Task<IActionResult> Create(Page page)
        {
            if (ModelState.IsValid)
            {
                page.Slug = page.Title.ToLower().Replace(" ", "-");
                page.Sorting = 100;

                var slug = await contex.Pages.FirstOrDefaultAsync(x => x.Slug == page.Slug);
                if (slug != null)
                {
                    ModelState.AddModelError("", "The title already exists.");
                    return View(page);
                }
                contex.Add(page);
                await contex.SaveChangesAsync();

                TempData["Success"] = "The page has been added!";

                return RedirectToAction("Index");
            }
            
            return View(page);
        }


    }
}
