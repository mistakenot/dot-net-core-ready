﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;

namespace DotNetCoreReady.Controllers
{
    public class HomeController : Controller
    {
        public ActionResult Index(string id = null)
        {
            ViewBag.DefaultId = id;
            return View();
        }
    }
}