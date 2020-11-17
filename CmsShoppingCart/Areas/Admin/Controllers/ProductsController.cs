using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using CmsShoppingCart.Infrastructure;
using CmsShoppingCart.Models;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.EntityFrameworkCore;

namespace CmsShoppingCart.Areas.Admin.Controllers
{
    [Area("Admin")]
    public class ProductsController : Controller
    {
        private readonly CmsShoppingCartContext contex;
        public ProductsController(CmsShoppingCartContext contex)
        {
            this.contex = contex;
        }

        //GET /admin/products
        public async Task<IActionResult> Index()
        {
            return View(await contex.Products.OrderByDescending(x => x.Id).Include(x => x.Category).ToListAsync());
        }

        //GET /admin/categories/create
        public IActionResult Create()
        {
            ViewBag.CategoryId = new SelectList(contex.Categories.OrderBy(x => x.Sorting), "Id", "Name");
            return View();
        }
           

        //POST /admin/categories/create
        [HttpPost]
        [ValidateAntiForgeryToken] //protect against CSRF attacks 
        public async Task<IActionResult> Create(Product product)
        {
            if (ModelState.IsValid)
            {
                product.Slug = product.Name.ToLower().Replace(" ", "-");

                var slug = await contex.Categories.FirstOrDefaultAsync(x => x.Slug == product.Slug);
                if (slug != null)
                {
                    ModelState.AddModelError("", "The product already exists.");
                    return View(product);
                }
                contex.Add(product);
                await contex.SaveChangesAsync();

                TempData["Success"] = "The product has been added!";

                return RedirectToAction("Index");
            }

            return View(product);
        }
    }
}
