using System;
using System.Collections.Generic;
using System.Linq;
using System.Net.Http;
using System.Web;

namespace ShoppingSpider.Common
{
    public static class HttpHelper
    {
        public static string GetHttpContent(string url)
        {
            //HttpRequest hr = new HttpRequest();
            string str=string.Empty;
            using (var client = new HttpClient())
            {
                 var responseString = client.GetStringAsync(url);
                str = responseString.Result;
            }
            return str;
        }
    }
}