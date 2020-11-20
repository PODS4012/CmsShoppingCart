using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Threading.Tasks;
using CmsShoppingCart.Infrastructure;
using CmsShoppingCart.Models;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.EntityFrameworkCore;

namespace CmsShoppingCart.Areas.Admin.Controllers
{
    [Area("Admin")]
    public class ProductsController : Controller
    {
        private readonly CmsShoppingCartContext contex;
        private readonly IWebHostEnvironment webHostEnvironment;
        public ProductsController(CmsShoppingCartContext contex, IWebHostEnvironment webHostEnvironment)
        {
            this.contex = contex;
            this.webHostEnvironment = webHostEnvironment;
        }

        //GET /admin/products
        public async Task<IActionResult> Index(int p = 1)
        {

            int pageSize = 4;

            var products = contex.Products.OrderByDescending(x => x.Id)
                                            .Include(x => x.Category)
                                            .Skip((p - 1) * pageSize)
                                            .Take(pageSize);

            ViewBag.PageNumber = p;
            ViewBag.PageRange = pageSize;
            ViewBag.TotalPages = (int)Math.Ceiling((decimal)contex.Products.Count() / pageSize);

            return View(await products.ToListAsync());

            // return list of all products without pagination
            //return View(await contex.Products.OrderByDescending(x => x.Id).Include(x => x.Category).ToListAsync());
        }

        //GET /admin/products/details/5
        public async Task<IActionResult> Details(int id)
        {
            Product product = await contex.Products.Include(x => x.Category).FirstOrDefaultAsync(x => x.Id == id);

            if (product == null)
            {
                return NotFound();
            }

            return View(product);
        }

        //GET /admin/categories/create
        public IActionResult Create()
        {
            ViewBag.CategoryId = new SelectList(contex.Categories.OrderBy(x => x.Sorting), "Id", "Name");
            return View();
        }

        //POST /admin/products/create
        [HttpPost]
        [ValidateAntiForgeryToken] //protect against CSRF attacks 
        public async Task<IActionResult> Create(Product product)
        {
            ViewBag.CategoryId = new SelectList(contex.Categories.OrderBy(x => x.Sorting), "Id", "Name");

            if (ModelState.IsValid)
            {
                product.Slug = product.Name.ToLower().Replace(" ", "-");

                var slug = await contex.Products.FirstOrDefaultAsync(x => x.Slug == product.Slug);
                if (slug != null)
                {
                    ModelState.AddModelError("", "The product already exists.");
                    return View(product);
                }

                string imageName = "noimage.png";
                if(product.ImageUpload != null)
                {
                    string uploadsDir = Path.Combine(webHostEnvironment.WebRootPath, "media/products");
                    imageName = Guid.NewGuid().ToString() + "_" + product.ImageUpload.FileName;
                    string filePath = Path.Combine(uploadsDir, imageName);
                    FileStream fs = new FileStream(filePath, FileMode.Create);
                    await product.ImageUpload.CopyToAsync(fs);
                    fs.Close();
                }
                product.Image = imageName;

                contex.Add(product);
                await contex.SaveChangesAsync();

                TempData["Success"] = "The product has been added!";

                return RedirectToAction("Index");
            }

            return View(product);
        }

        //GET /admin/products/edit/5
        public async Task<IActionResult> Edit(int id)
        {
            Product product = await contex.Products.FindAsync(id);
            ViewBag.CategoryId = new SelectList(contex.Categories.OrderBy(x => x.Sorting), "Id", "Name", product.CategoryId);

            if (product == null)
            {
                return NotFound();
            }

            return View(product);
        }

        //POST /admin/products/edit/5
        [HttpPost]
        [ValidateAntiForgeryToken] //protect against CSRF attacks 
        public async Task<IActionResult> Edit(int id, Product product)
        {
            ViewBag.CategoryId = new SelectList(contex.Categories.OrderBy(x => x.Sorting), "Id", "Name", product.CategoryId);

            if (ModelState.IsValid)
            {
                product.Slug = product.Name.ToLower().Replace(" ", "-");

                var slug = await contex.Products.Where(x => x.Id != id).FirstOrDefaultAsync(x => x.Slug == product.Slug);
                if (slug != null)
                {
                    ModelState.AddModelError("", "The product already exists.");
                    return View(product);
                }

                if (product.ImageUpload != null)
                {
                    string uploadsDir = Path.Combine(webHostEnvironment.WebRootPath, "media/products");

                    if (!string.Equals(product.Image, "noimage.png"))
                    {
                        string oldImagePath = Path.Combine(uploadsDir, product.Image);
                        if(System.IO.File.Exists(oldImagePath))
                        {
                            System.IO.File.Delete(oldImagePath);
                        }
                    }

                    string imageName = Guid.NewGuid().ToString() + "_" + product.ImageUpload.FileName;
                    string filePath = Path.Combine(uploadsDir, imageName);
                    FileStream fs = new FileStream(filePath, FileMode.Create);
                    await product.ImageUpload.CopyToAsync(fs);
                    fs.Close();
                    product.Image = imageName;
                }
                

                contex.Update(product);
                await contex.SaveChangesAsync();

                TempData["Success"] = "The product has been added!";

                return RedirectToAction("Index");
            }

            return View(product);
        }

        //GET /admin/products/delete/5
        public async Task<IActionResult> Delete(int id)
        {
            Product product = await contex.Products.FindAsync(id);



            if (product == null)
            {
                TempData["Error"] = "The product does not exist!";
            }
            else
            {
                if (!string.Equals(product.Image, "noimage.png"))
                {
                    string uploadsDir = Path.Combine(webHostEnvironment.WebRootPath, "media/products");
                    string oldImagePath = Path.Combine(uploadsDir, product.Image);
                    if (System.IO.File.Exists(oldImagePath))
                    {
                        System.IO.File.Delete(oldImagePath);
                    }
                }
                contex.Products.Remove(product);
                await contex.SaveChangesAsync();

                TempData["Success"] = "The product has been deleted!";
            }

            return RedirectToAction("Index");
        }
    }
}
