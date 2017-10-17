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
        public ActionResult Index(string keyword,string[] cks,string sort,int? page,int btn)
        {

            
            List<Products> list = new List<Products>();
            if (string.IsNullOrEmpty(keyword)) return View(list);
            if (cks == null) return View(list);
            if (btn == 2)
            {
                if (page == null)
                {
                    page = 2;
                }
                else
                {
                    page++;
                }
            }
            for (int i = 0; i < cks.Length; i++)
            {
                if (cks[i] == "1")
                {
                    List<Products> l1 = GetJdList(keyword,page);
                    list.AddRange(l1);
                    ViewData["jd"] = true;
                }
                else if (cks[i] == "2")
                {
                    ViewData["amazon"] = true;
                    try
                    {
                        List<Products> l1 = GetAmazonList(keyword,page);
                        list.AddRange(l1);
                        
                    }
                    catch
                    {
                        continue;
                    }
                }
                else if (cks[i] == "3")
                {
                    ViewData["sn"] = true;
                    List<Products> l1 = GetSnList(keyword,page);
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
            ViewData["price"] = sort=="1" ;
            ViewData["keyword"] = keyword;
            ViewData["page"] = page;
            return View(list);
        }


        public ActionResult Test(string keyword, string[] cks, string sort,int page)
        {
            keyword = "硬盘";
            keyword = Url.Encode(keyword);
            //https://www.amazon.cn/s/ref=sr_pg_4?rh=i%3Aaps%2Ck%3A%E7%A1%AC%E7%9B%98&page=4&keywords=%E7%A1%AC%E7%9B%98&ie=UTF8&qid=1508227732
            //https://www.amazon.cn/s/ref=sr_pg_2?rh=i%3Aaps%2Ck%3A"+keyword+"&page="+page+"&keywords="+keyword+"&ie=UTF8&qid=1508227797
            string url = "https://www.amazon.cn/s/ref=sr_pg_"+page+"?rh=i%3Aaps%2Ck%3A" + keyword + "&page=" + page + "&keywords=" + keyword + "&ie=UTF8&qid=1508227797";
            string str = Common.HttpHelper.HttpGet(url);
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
        public List<Products> GetJdList(string keyword,int? page=1)
        {
            if (page % 2 == 0) page++;
            keyword = Url.Encode(keyword);
            //https://search.jd.com/Search?keyword=%E7%94%B7%E8%A3%85&enc=utf-8&qrst=1&rt=1&stop=1&vt=2&cid2=1342&page=3
            string url = "https://search.jd.com/Search?keyword=" + keyword + "&enc=utf-8&qrst=1&rt=1&stop=1&vt=2&page="+page;
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

        public List<Products> GetAmazonList(string keyword, int? page = 1)
        {
            if (page == null) page = 1;
            keyword = Url.Encode(keyword);
            string url = "https://www.amazon.cn/s/ref=sr_pg_" + page + "?rh=i%3Aaps%2Ck%3A" + keyword + "&page=" + page + "&keywords=" + keyword + "&ie=UTF8&qid=1508227797";
            string str = Common.HttpHelper.HttpGet(url);
            Document htmlDoc = NSoup.NSoupClient.Parse(str);
            //获得item的li
            Elements lis = htmlDoc.GetElementById("s-results-list-atf").Select(".s-result-item");
            List<Products> list = new List<Products>();
            Products pro = null;
            foreach (var item in lis)
            {

                //li内的warp
                var liWarp = item.Select(".s-item-container");
                pro = new Products();
                pro.ImgUrl =  liWarp.Select(".a-spacing-base img").Attr("src");
                pro.Name = liWarp.Select("h2.s-access-title").Text;
                pro.Price = Convert.ToDouble(liWarp.Select(".s-price").Text.Substring(1));
                pro.ShopName = "";
                pro.SourceUrl = liWarp.Select(".a-row .a-row .s-color-twister-title-link").Attr("href");
                pro.SourceSite = "www.amazon.cn";
                pro.commentsCount = "";
                list.Add(pro);
            }

            return list;

        }

        public List<Products> GetSnList(string keyword,int? page=1)
        {
            keyword = Url.Encode(keyword);
            if (page == null) page = 1;
            //https://search.suning.com/emall/searchProductList.do?keyword=%E7%A1%AC%E7%9B%98&ci=0&pg=01&cp=1&il=0&st=0&iy=0&adNumber=0&n=1&sesab=BBA&id=IDENTIFYING&cc=020
            string url = "https://search.suning.com/emall/searchProductList.do?keyword=" + keyword + "&ci=0&pg=01&cp="+(page-1).ToString()+"&il=0&st=0&iy=0&adNumber=0&n=1&sesab=BBA&id=IDENTIFYING&cc=020";
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