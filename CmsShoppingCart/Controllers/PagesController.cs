using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using CmsShoppingCart.Infrastructure;
using CmsShoppingCart.Models;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

namespace CmsShoppingCart.Controllers
{
    public class PagesController : Controller
    {
        private readonly CmsShoppingCartContext contex;
        public PagesController(CmsShoppingCartContext contex)
        {
            this.contex = contex;
        }

        //GET / or / slug
        public async Task<IActionResult> Page(string slug)
        {
            if (slug == null)
            {
                return View(await contex.Pages.Where(x => x.Slug == "home").FirstOrDefaultAsync());
            }

            Page page = await contex.Pages.Where(x => x.Slug == slug).FirstOrDefaultAsync();

            if (page == null)
            {
                return NotFound();
            }

            return View(page);
        }
    }
}
