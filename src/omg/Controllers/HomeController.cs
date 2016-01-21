using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNet.Mvc;
using System.Net.Http;
using omg.Models;


namespace omg.Controllers
{
    public class HomeController : Controller
    {
        public static string myContent = "myContent";
        public IActionResult Index(string sarea)
        {
            ViewData["sarea"] = sarea;
            return View();
        }

        public IActionResult About()
        {
            ViewData["Message"] = "Your application description page.";
            return View();
        }

        public IActionResult Contact()
        {
            ViewData["Message"] = "Your contact page.";

            return View();
        }

        public IActionResult Error()
        {
            return View();
        }

        public IActionResult Info(string sarea,string id2)
        {
            ViewData["sarea"] = sarea;
            ViewData["Message2"] = id2;

            return View();
        }
        /*
        public string Get(int id)
        {
            return id + "value";
        }
        */
        omg.Models.TrafficStats stats = new TrafficStats();


        /*
                static async void MyPostJSONTask()
                {
                    var task = PostJSON("http://data.taipei/opendata/datalist/apiAccess?scope=resourceAquire&rid=55ec6d6e-dc5c-4268-a725-d04cc262172b");
                    var content = await task;
                    myContent = content;
                }

                static  async Task<string> PostJSON(string url)
                {
                    using (var client = new HttpClient())
                    {
                        string content = await client.GetStringAsync(url);
                        return content;
                    }
                }
                */
    }
}
