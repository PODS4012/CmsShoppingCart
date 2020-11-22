using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using CmsShoppingCart.Infrastructure;
using CmsShoppingCart.Models;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

namespace CmsShoppingCart.Areas.Admin.Controllers
{
    [Authorize(Roles = "admin")]
    [Area("Admin")]
    public class CategoriesController : Controller
    {
        private readonly CmsShoppingCartContext contex;
        public CategoriesController(CmsShoppingCartContext contex)
        {
            this.contex = contex;
        }

        //GET /admin/categories
        public async Task<IActionResult> Index()
        {
            return View(await contex.Categories.OrderBy(x => x.Sorting).ToListAsync());
        }

        //GET /admin/categories/create
        public IActionResult Create() => View();

        //POST /admin/categories/create
        [HttpPost]
        [ValidateAntiForgeryToken] //protect against CSRF attacks 
        public async Task<IActionResult> Create(Category category)
        {
            if (ModelState.IsValid)
            {
                category.Slug = category.Name.ToLower().Replace(" ", "-");
                category.Sorting = 100;

                var slug = await contex.Categories.FirstOrDefaultAsync(x => x.Slug == category.Slug);
                if (slug != null)
                {
                    ModelState.AddModelError("", "The category already exists.");
                    return View(category);
                }
                contex.Add(category);
                await contex.SaveChangesAsync();

                TempData["Success"] = "The Category has been added!";

                return RedirectToAction("Index");
            }

            return View(category);
        }

        //GET /admin/category/edit/5
        public async Task<IActionResult> Edit(int id)
        {
            Category category = await contex.Categories.FindAsync(id);

            if (category == null)
            {
                return NotFound();
            }

            return View(category);
        }

        //POST /admin/category/edit
        [HttpPost]
        [ValidateAntiForgeryToken] //protect against CSRF attacks 
        public async Task<IActionResult> Edit(int id, Category category)
        {
            if (ModelState.IsValid)
            {
                category.Slug = category.Name.ToLower().Replace(" ", "-");

                var slug = await contex.Categories.Where(x => x.Id != id).FirstOrDefaultAsync(x => x.Slug == category.Slug);
                if (slug != null)
                {
                    ModelState.AddModelError("", "The category already exists.");
                    return View(category);
                }
                contex.Update(category);
                await contex.SaveChangesAsync();

                TempData["Success"] = "The category has been updated!";

                return RedirectToAction("Edit", new { id });
            }

            return View(category);
        }

        //GET /admin/category/delete/5
        public async Task<IActionResult> Delete(int id)
        {
            Category category = await contex.Categories.FindAsync(id);

            if (category == null)
            {
                TempData["Error"] = "The category does not exist!";
            }
            else
            {
                contex.Categories.Remove(category);
                await contex.SaveChangesAsync();

                TempData["Success"] = "The category has been deleted!";
            }

            return RedirectToAction("Index");
        }

        //POST /admin/categories/reorder
        [HttpPost]
        public async Task<IActionResult> Reorder(int[] id)
        {
            int count = 1;

            foreach (var categoryId in id)
            {
                Category category = await contex.Categories.FindAsync(categoryId);
                category.Sorting = count;
                contex.Update(category);
                await contex.SaveChangesAsync();
                count++;
            }

            return Ok();
        }
    }
}
