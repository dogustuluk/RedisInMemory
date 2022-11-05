using InMemoryApp.Web.Models;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Caching.Memory;
using System;

namespace InMemoryApp.Web.Controllers
{
    public class ProductController : Controller
    {
        private IMemoryCache _memoryCache;
        public ProductController(IMemoryCache memoryCache)
        {
            _memoryCache = memoryCache;
        }
        public IActionResult Index()
        {
            #region description Get() & Set()
            /*
             * Get -> memory'den datayı alır, Set -> memory'e datayı kaydetmek için kullanılır.
             * Bunlar generic metotlardır.
             * Memory'de her şeyi tutabiliriz. Bir sınfıı serialize edip de tutabiliriz. Tek sınırı memory'nin kapasitesidir.
             * cache'te her şey key-value şeklinde tutulur.
             */
            #endregion
            #region description TryGetValue() & Remove()
            /*
             * TryGetValue -> Değeri almak için kullanırız.
             * Remove -> Memory'deki herhangi bir key'i silmek için kullanırız.
             */
            #endregion
            #region GetOrCreate
            //Bu metot ile verilen key'i almaya çalışılır, eğer yok ise oluşturulur. Burada bir fonksiyon oluşturulur.
            #endregion
            #region AbsoluteExpiration & SlidingExpiration
            /*Bir cache oluştururken ömrünü belirleyecek olan property'lerdir.
             * AbsoluteExpiration -> Eğer burada bir ömür verirsek, cache'in ömrü burada belirtilen süre kadar olur.
             * SlidingExpiration -> Eğer burada bir ömür verirsek; vermiş olduğumuz süre kadar inaktif kalırsa cache silinir, eğer bu süre içerisinde dataya erişilirse, bu cache'in ömrü otomatik olarak verilen süre kadar artar.
             * Sliding Expiration veriyorsak sonsuz döngüden kurtulmak ve bayat dataları almaktan kurtulmak için ek olarak Absolute Expiration da vermemiz gerekmektedir.
             */
            #endregion
            #region CachePriority
            /*
             * CacheOptions üzerinden set edebiliriz.
             * ne işe yarar -> memory dolduğunda yeni bir data kaydetmek istediğimizde memory'deki hangi key'lerin silineceğini belirtiriz.
             */
            #endregion
            #region RegisterPostEvictionCallback
            /*
             * memory'deki datanın hangi nedenle silindiğini anlamamıza yardımcı olan metottur.
             */
            #endregion
            #region Complex Types Caching
            /*
             * complex types -> uygulama içerisinde kullanılan classlar ve bunlardan alınan nesne örneklerinin cache'lenmesi.
             * 
             */
            #endregion

            //key değerinin memory'de olup olmadığının kontrolü. 2 yol vardır.
            ////1.yol
            //if (String.IsNullOrEmpty(_memoryCache.Get<string>("zaman")))
            //{
            //    _memoryCache.Set<string>("zaman", DateTime.Now.ToString());

            //}

            //2.yol -> önerilir
            //if (!_memoryCache.TryGetValue("zaman", out string zamancache))
            //{
            //cache ömrü verelim.
            MemoryCacheEntryOptions options = new MemoryCacheEntryOptions();
            options.AbsoluteExpiration = DateTime.Now.AddMinutes(1); //10 saniyelik bir ömür verdik.
            options.SlidingExpiration = TimeSpan.FromSeconds(10);
            options.Priority = CacheItemPriority.High;//neverRemove exception'a düşürebilir eğer tüm dataları böyle işaretlersek.
            options.RegisterPostEvictionCallback((key, value, reason, state) =>
            {
                _memoryCache.Set("callback", $"{key}->{value} => sebep:{reason}");
            });
            _memoryCache.Set<string>("zaman", DateTime.Now.ToString(), options);

            //}
            //true dönerse zamancache'i alıp index tarafında kullanabiliriz.


            Product p = new Product { Id = 1, Name = "Kalem", Price = 200 };
            _memoryCache.Set<Product>("product:1", p);


            return View();
        }
        public IActionResult Show()
        {
            //_memoryCache.Remove("zaman");

            //_memoryCache.GetOrCreate<string>("zaman", entry =>
            //{
            //    return DateTime.Now.ToString();
            //});


            _memoryCache.TryGetValue("zaman", out string zamancache);

            _memoryCache.TryGetValue("callback", out string callback);
            ViewBag.callback = callback;

            ViewBag.zaman = zamancache;

            //ViewBag.zaman = _memoryCache.Get<string>("zaman");

            ViewBag.product = _memoryCache.Get<Product>("product:1");
            
            
            
            return View();
        }
    }
}
