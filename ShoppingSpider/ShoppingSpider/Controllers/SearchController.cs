using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;

namespace ShoppingSpider.Controllers
{
    public class SearchController : Controller
    {
        // GET: Search
        public ActionResult Index()
        {
            string str = Common.HttpHelper.GetHttpContent("http://jd.com");
            ViewData["str"] = str;
            return View();
        }
    }
}