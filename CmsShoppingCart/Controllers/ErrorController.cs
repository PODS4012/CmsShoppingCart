using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;

namespace CmsShoppingCart.Controllers
{
    public class ErrorController : Controller
    {
        public IActionResult HttpStatusCodeHandler(int statusCode)
        {
            switch (statusCode)
            {
                case 404:
                    ViewBag.ErrorMessage = "That page does not exist.";
                    break;
                default:
                    ViewBag.ErrorMessage = "Something went wronng.";
                    break;
            }

            return View("Not Found");
        }
    }
}
