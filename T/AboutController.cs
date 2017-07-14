using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace T
{
    using System.Web.Mvc;

    public class AboutController : Controller
    {
        public ActionResult Index()
        {
            return View();
        }
    }
}