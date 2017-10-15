using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using NSoup.Nodes;
using NSoup.Select;
using ShoppingSpider.Models;
using System.Text;

namespace ShoppingSpider.Controllers
{
    public class SearchController : Controller
    {
        // GET: Search
        public ActionResult Index(string keyword,string[] cks,string sort)
        {
            List<Products> list = new List<Products>();
            if (string.IsNullOrEmpty(keyword)) return View(list);
            
            for (int i = 0; i < cks.Length; i++)
            {
                if (cks[i] == "1")
                {
                   list = GetJdList(keyword);
                    //list.Concat(list1);
                }
            }
            
            if (!string.IsNullOrEmpty(sort))
            {
                
                if (sort == "1")
                {
                    var query = from item in list
                                orderby item.Price ascending
                                select item;
                    list = query.ToList<Products>();
                }
                //else if (sort == "2")
                //{
                //    var query = from item in list
                //                orderby item.Price descending
                //                select item;
                //    list = query.ToList<Products>();
                //}
               
            }
            
            ViewData["keyword"] = keyword;
            return View(list);
        }


        public ActionResult Test(string keyword, string[] cks, string sort)
        {
            keyword = "硬盘";
            keyword = Url.Encode(keyword);
            string url = "https://s.taobao.com/search?q=" + keyword + "&imgfile=&commend=all&ssid=s5-e&search_type=item&sourceId=tb.index&spm=a21bo.50862.201856-taobao-item.1&ie=utf8&initiative_id=tbindexz_20171014";
            string str = Common.HttpHelper.GetHttpContent(url);
            Document htmlDoc = NSoup.NSoupClient.Parse(str);
            //获得item的li
            Element lis = htmlDoc.GetElementById("mainsrp-itemlist");
            
            StringBuilder sb = new StringBuilder();
            sb.Append(htmlDoc.Html());
            ViewData["keyword"] = keyword;
            ViewData["str"] = sb.ToString() ;
            return View();
        }
        public List<Products> GetJdList(string keyword)
        {
            keyword = Url.Encode(keyword);
            string url = "https://search.jd.com/Search?keyword=" + keyword + "&enc=utf-8&wq=" + keyword  ;
            string str = Common.HttpHelper.HttpGet(url);
            Document htmlDoc = NSoup.NSoupClient.Parse(str);
            //获得item的li
            Elements lis = htmlDoc.GetElementsByClass("gl-warp").Select(".gl-item");
            List<Products> list = new List<Products>();
            Products pro = null;
            foreach (var item in lis)
            {
                //过滤广告的item
                if (item.Attr("data-type") == "activity") continue;
                //li内的warp
                var liWarp = item.Select(".gl-i-wrap");
                pro = new Products();
                pro.ImgUrl = "http:"+liWarp.Select(".p-img img").Attr("src");
                pro.Name = liWarp.Select(".p-name a em").Text;
                pro.Price = Convert.ToDouble(liWarp.Select(".p-price strong i").Text);
                pro.ShopName = liWarp.Select(".p-shop span a").Text;
                pro.SourceUrl = liWarp.Select(".p-name a").Attr("href");
                pro.SourceSite = "jd.com";
                list.Add(pro);
            }
      
            return list;

        }

        //public List<Products> GetTaobaoList(string keyword)
        //{
        //    keyword = Url.Encode(keyword);
        //    string url = "https://s.taobao.com/search?q="+keyword+"&imgfile=&commend=all&ssid=s5-e&search_type=item&sourceId=tb.index&spm=a21bo.50862.201856-taobao-item.1&ie=utf8&initiative_id=tbindexz_20171014";
        //    string str = Common.HttpHelper.HttpGet(url);
        //    Document htmlDoc = NSoup.NSoupClient.Parse(str);
        //    //获得item的li
        //    Elements lis = htmlDoc.GetElementsByClass("m-itemlist").Select(".g-clearfix .items:nth(0)");
        //    List<Products> list = new List<Products>();
        //    Products pro = null;
        //    foreach (var item in lis)
        //    {
        //        //过滤广告的item
        //        if (item.Attr("data-type") == "activity") continue;
        //        //li内的warp
        //        var liWarp = item.Select(".gl-i-wrap");
        //        pro = new Products();
        //        pro.ImgUrl = "http:" + liWarp.Select(".p-img img").Attr("src");
        //        pro.Name = liWarp.Select(".p-name a em").Text;
        //        pro.Price = Convert.ToDouble(liWarp.Select(".p-price strong i").Text);
        //        pro.ShopName = liWarp.Select(".p-shop span a").Text;
        //        pro.SourceUrl = liWarp.Select(".p-name a").Attr("href");
        //        pro.SourceSite = "jd.com";
        //        list.Add(pro);
        //    }

        //    return list;

        //}
        

    }
}