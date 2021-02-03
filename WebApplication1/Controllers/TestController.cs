using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using WebApplication1.Utils;

namespace WebApplication1.Controllers
{
    public class TestController : Controller
    {
        // GET: Test
        public ActionResult Index()
        {
            return View();
        }

        public ActionResult GoogleAPI()
        {
            //var url = "https://maps.googleapis.com/maps/api/place/textsearch/json?";

            //var url2 = "https://maps.googleapis.com/maps/api/place/autocomplete/json?";

            var url3 = "https://maps.googleapis.com/maps/api/place/queryautocomplete/json?";
            var parms = new Dictionary<string, string>();
            parms.Add("input", "1600+Amphitheatre");
            parms.Add("key", "AIzaSyA-CUWBYOIgYG3gNUDFV8shvJCN8iZ-gG4");
            
            var result = HttpUtils.doPost(url3, parms,null);
            Response.Write(result);

            Response.Write("<br><br><br>");

            var newURL = "https://maps.googleapis.com/maps/api/place/queryautocomplete/json?key=AIzaSyA-CUWBYOIgYG3gNUDFV8shvJCN8iZ-gG4&input=pizza";
            var result2 = HttpUtils.HttpGet(newURL, "");
            Response.Write(result2);

            Response.Write("<br><br><br>");

            var URL2 = "https://maps.googleapis.com/maps/api/place/queryautocomplete/json?key=AIzaSyA-CUWBYOIgYG3gNUDFV8shvJCN8iZ-gG4&input=pizza";
            var result3 = HttpUtils.WebRestGetData(URL2);
            Response.Write(result3);

            var result4 = HttpUtils.DoGet(url3, parms);
            Response.Write(result4);



            return View();
        }

    


        public JsonResult SaveInfo(TestInfo model)
        {
            var result = new TestInfo() {
                Name=model.Name,
                Age=model.Age,
                Address=model.Address
            };

            return Json(new { msg = "Success" });
        }
    }

    public class TestInfo
    {
        public string Name { get; set; }

        public int Age { get; set; }

        public string Address { get; set; }
    }
}