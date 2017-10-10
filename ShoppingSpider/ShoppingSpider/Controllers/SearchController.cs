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
            string str = Common.HttpHelper.GetHttpContent("https://search.jd.com/Search?keyword=%E7%A1%AC%E7%9B%98&enc=utf-8&wq=%E7%A1%AC%E7%9B%98&pvid=ce46cbe580424cde9595644d9f8d715d");
            ViewData["str"] = str;
            return View();
        }
    }
}