using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using CmsShoppingCart.Infrastructure;
using CmsShoppingCart.Models;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

namespace CmsShoppingCart.Controllers
{
    [Authorize]
    public class ProductsController : Controller
    {
        private readonly CmsShoppingCartContext contex;
        public ProductsController(CmsShoppingCartContext contex)
        {
            this.contex = contex;
        }
        //GET /products
        public async Task<IActionResult> Index(int p = 1)
        {

            int pageSize = 4;

            var products = contex.Products.OrderByDescending(x => x.Id)
                                            .Skip((p - 1) * pageSize)
                                            .Take(pageSize);

            ViewBag.PageNumber = p;
            ViewBag.PageRange = pageSize;
            ViewBag.TotalPages = (int)Math.Ceiling((decimal)contex.Products.Count() / pageSize);

            return View(await products.ToListAsync());

            // return list of all products without pagination
            //return View(await contex.Products.OrderByDescending(x => x.Id).Include(x => x.Category).ToListAsync());
        }

        //GET /products/category
        public async Task<IActionResult> ProductsByCategory(string categorySlug, int p = 1)
        {
            Category category = await contex.Categories.Where(x => x.Slug == categorySlug).FirstOrDefaultAsync();

            if (category == null) return RedirectToAction("Index");

            int pageSize = 3;

            var products = contex.Products.OrderByDescending(x => x.Id)
                                            .Where(x => x.CategoryId == category.Id)
                                            .Skip((p - 1) * pageSize)
                                            .Take(pageSize);

            ViewBag.PageNumber = p;
            ViewBag.PageRange = pageSize;
            ViewBag.TotalPages = (int)Math.Ceiling((decimal)contex.Products.Where(x => x.CategoryId == category.Id).Count() / pageSize);

            ViewBag.CategoryName = category.Name;
            ViewBag.CategorySlug = categorySlug;
            return View(await products.ToListAsync());

            // return list of all products without pagination
            //return View(await contex.Products.OrderByDescending(x => x.Id).Include(x => x.Category).ToListAsync());
        }
    }
}
