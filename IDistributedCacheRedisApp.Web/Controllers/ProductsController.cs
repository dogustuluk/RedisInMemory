using IDistributedCacheRedisApp.Web.Models;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Caching.Distributed;
using Newtonsoft.Json;
using System;
using System.IO;
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

        /// <summary>
        /// Resim ya da pdf gibi dosya formatlarını cachelemekle sorumlu metot.
        /// </summary>
        /// <returns></returns>
        public IActionResult ImageCache()
        {
            string path = Path.Combine(Directory.GetCurrentDirectory(), "wwwroot/Images/photo.jpg");
            
            byte[] imageByte = System.IO.File.ReadAllBytes(path);

            _distributedCache.Set("resim", imageByte);

            return View();
        }
        /// <summary>
        /// Resim dosya format türüne sahip dosyaların yolunu almakla sorumlu metot.
        /// </summary>
        /// <returns></returns>
        public IActionResult ImageUrl()
        {
            byte[] resimByte = _distributedCache.Get("resim");

            return File(resimByte,"image/jpg");
        }
    }
}
