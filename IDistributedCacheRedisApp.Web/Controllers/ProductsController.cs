using IDistributedCacheRedisApp.Web.Models;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Caching.Distributed;
using Newtonsoft.Json;
using System;
using System.Text;
using System.Threading.Tasks;

namespace IDistributedCacheRedisApp.Web.Controllers
{
    public class ProductsController : Controller
    {
        private IDistributedCache _distributedCache;
        public ProductsController(IDistributedCache distributedCache)
        {
            _distributedCache = distributedCache;

        }
        public async Task<IActionResult> Index()
        {
            DistributedCacheEntryOptions cacheEntryOptions = new DistributedCacheEntryOptions();
            cacheEntryOptions.AbsoluteExpiration = DateTime.Now.AddMinutes(15);

            //_distributedCache.SetString("name", "dogus", cacheEntryOptions);

            Product product = new Product { Id=2, Name="Kalem2", Price=100 };
            //json serializer
            string jsonProduct = JsonConvert.SerializeObject(product);

            //await _distributedCache.SetStringAsync("product:2", jsonProduct, cacheEntryOptions);



            //binary serialize
            Byte[] byteProduct = Encoding.UTF8.GetBytes(jsonProduct);
            _distributedCache.Set("product:3", byteProduct, cacheEntryOptions);

            return View();
        }
        public IActionResult Show()
        {
            //string name = _distributedCache.GetString("name"); //memory'deki datayı okuyabiliriz.

            Byte[] byteProduct = _distributedCache.Get("product:3");

            //string jsonProduct = _distributedCache.GetString("product:1");

            //binary deserialize
            string jsonProduct = Encoding.UTF8.GetString(byteProduct);



            Product p = JsonConvert.DeserializeObject<Product>(jsonProduct);
            
            ViewBag.product = p;
            return View();
        }
        public IActionResult Remove()
        {
            _distributedCache.Remove("name");
            return View();
        }
    }
}
