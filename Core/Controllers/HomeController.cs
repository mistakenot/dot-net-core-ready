using System;
using System.Collections.Generic;
using System.Linq;
using Microsoft.AspNetCore.Mvc;

namespace DotNetCoreReady.Controllers
{
    public class HomeController : Controller
    {
        public IActionResult Index(string id = null)
        {
            ViewBag.DefaultId = id;
            return View();
        }
    }
}