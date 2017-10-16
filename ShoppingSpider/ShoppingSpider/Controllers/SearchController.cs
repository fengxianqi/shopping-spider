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
                    List<Products> l1 = GetJdList(keyword);
                    list.AddRange(l1);
                }
                else if(cks[i]=="3")
                {
                    List<Products> l1 = GetSnList(keyword, 1);
                    list.AddRange(l1);
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
            //https://search.suning.com/emall/searchProductList.do?keyword=%E7%A1%AC%E7%9B%98&ci=0&pg=01&cp=1&il=0&st=0&iy=0&adNumber=0&n=1&sesab=BBA&id=IDENTIFYING&cc=020
            string url = "https://search.suning.com/" + keyword+"/";
            string str = Common.HttpHelper.GetHttpContent(url);
            //Document htmlDoc = NSoup.NSoupClient.Parse(str);
            //获得item的li
            //Elements lis = htmlDoc.GetElementById("filter-results").Select(".clearfix .product");
            //List<Products> list = new List<Products>();
            //Products pro = null;
            StringBuilder sb = new StringBuilder();
            //foreach (var item in lis)
            //{
            //    //过滤广告的item
            //    var liWarp = item.Select(".wrap");
            //    pro = new Products();
            //    pro.ImgUrl = "http:" + liWarp.Select(".res-img img").Attr("src");
            //    pro.Name = liWarp.Select(".res-info .sell-point a").Text;
            //    pro.Price = 0;
            //    pro.ShopName = liWarp.Select(".res-info .seller").Text;
            //    pro.SourceUrl = liWarp.Select(".res-info .sell-point a").Attr("href");
            //    pro.SourceSite = "suning.com";
            //    list.Add(pro);
            //    sb.Append(liWarp.Text);
            //    sb.Append("------------------------------------------------------------------------------------------");
            //}


           
            ViewData["keyword"] = keyword;
            ViewData["str"] = str ;
            return View();
        }
        public List<Products> GetJdList(string keyword,int page=1)
        {
            keyword = Url.Encode(keyword);
            //https://search.jd.com/Search?keyword=%E7%94%B7%E8%A3%85&enc=utf-8&qrst=1&rt=1&stop=1&vt=2&cid2=1342&page=3
            string url = "https://search.jd.com/Search?keyword=" + keyword + "&enc=utf-8&qrst=1&rt=1&stop=1&vt=2&page=";
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
                pro.commentsCount = liWarp.Select(".p-commit a").Text;
                list.Add(pro);
            }
      
            return list;

        }
        public List<Products> GetSnList(string keyword,int page=1)
        {
            keyword = "硬盘";
            keyword = Url.Encode(keyword);
            //https://search.suning.com/emall/searchProductList.do?keyword=%E7%A1%AC%E7%9B%98&ci=0&pg=01&cp=1&il=0&st=0&iy=0&adNumber=0&n=1&sesab=BBA&id=IDENTIFYING&cc=020
            string url = "https://search.suning.com/emall/searchProductList.do?keyword=" + keyword + "&ci=0&pg=01&cp=1&il=0&st=0&iy=0&adNumber=0&n=1&sesab=BBA&id=IDENTIFYING&cc=020";
            string str = Common.HttpHelper.GetHttpContent(url);
            Document htmlDoc = NSoup.NSoupClient.Parse(str);
            //获得item的li
            Elements lis = htmlDoc.GetElementById("filter-results").GetElementsByClass("product");
            List<Products> list = new List<Products>();
            Products pro = null;
            foreach (var item in lis)
            {
               
                //li内的warp
                var liWarp = item.Select(".wrap");
                pro = new Products();
                pro.ImgUrl = "http:" + liWarp.Select(".res-img img").Attr("src");
                pro.Name = liWarp.Select(".res-info .sell-point a").Text;
                pro.Price = 0;
                pro.ShopName = liWarp.Select(".res-info .seller").Text;
                pro.SourceUrl = liWarp.Select(".res-info .sell-point a").Attr("href");
                pro.SourceSite = "suning.com";
                pro.commentsCount = liWarp.Select(".res-info .com-cnt a.num").Text;
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