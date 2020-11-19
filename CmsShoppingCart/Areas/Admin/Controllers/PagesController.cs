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
    public class PagesController : Controller
    {
        private readonly CmsShoppingCartContext contex;
        public PagesController(CmsShoppingCartContext contex)
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
                    ModelState.AddModelError("", "The page already exists.");
                    return View(page);
                }
                contex.Add(page);
                await contex.SaveChangesAsync();

                TempData["Success"] = "The page has been added!";

                return RedirectToAction("Index");
            }
            
            return View(page);
        }

        //GET /admin/pages/edit/5
        public async Task<IActionResult> Edit(int id)
        {
            Page page = await contex.Pages.FindAsync(id);

            if (page == null)
            {
                return NotFound();
            }

            return View(page);
        }

        //POST /admin/pages/edit
        [HttpPost]
        [ValidateAntiForgeryToken] //protect against CSRF attacks 
        public async Task<IActionResult> Edit(Page page)
        {
            if (ModelState.IsValid)
            {
                page.Slug = page.Id == 1 ? "home" : page.Title.ToLower().Replace(" ", "-");

                var slug = await contex.Pages.Where(x => x.Id != page.Id).FirstOrDefaultAsync(x => x.Slug == page.Slug);
                if (slug != null)
                {
                    ModelState.AddModelError("", "The page already exists.");
                    return View(page);
                }
                contex.Update(page);
                await contex.SaveChangesAsync();

                TempData["Success"] = "The page has been updated!";

                return RedirectToAction("Edit", new { id = page.Id });
            }

            return View(page);
        }

        //GET /admin/pages/delete/5
        public async Task<IActionResult> Delete(int id)
        {
            Page page = await contex.Pages.FindAsync(id);

            if (page == null)
            {
                TempData["Error"] = "The page does not exist!";
            }
            else
            {
                if (page.Id == 1)
                {
                    TempData["Error"] = "Unable to delete Home page!";
                }
                else
                {
                    contex.Pages.Remove(page);
                    await contex.SaveChangesAsync();

                    TempData["Success"] = "The page has been deleted!";
                }
            }

            return RedirectToAction("Index");
        }

        //POST /admin/pages/reorder
        [HttpPost]
        public async Task<IActionResult> Reorder(int[] id)
        {
            int count = 1;

            foreach (var pageId in id)
            {
                Page page = await contex.Pages.FindAsync(pageId);
                page.Sorting = count;
                contex.Update(page);
                await contex.SaveChangesAsync();
                count++;
            }

            return Ok();
        }
    }
}
