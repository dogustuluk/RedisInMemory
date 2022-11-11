using Microsoft.AspNetCore.Mvc;
using RedisExchangeAPI.Web.Services;
using StackExchange.Redis;

namespace RedisExchangeAPI.Web.Controllers
{
    public class StringTypeController : Controller
    {
        private readonly RedisService _redisService;
        private readonly IDatabase db;

        public StringTypeController(RedisService redisService)
        {
            _redisService= redisService;
            db = _redisService.GetDb(0);
        }
        public IActionResult Index()
        {
            //var db = _redisService.GetDb(0); Eğer her metotta farklı bir db var ise bunu metot içerisinde tanımlarız eğer tüm metotlar aynı db'yi kullanıyor ise di olarak geçilmeli.

            db.StringSet("name", "Doğuş Tuluk");
            db.StringSet("ziyaretci", 100);


            return View();
        }
        public IActionResult Show()
        {
            var value = db.StringGet("name");
            
            //db.StringIncrement("ziyaretci", 10); //sayfa her yenilendiğinde ziyaretci key'ine sahip değer 10 artacak.
            db.StringDecrementAsync("ziyaretci", 1).Result.ToString();
            //db.StringDecrementAsync("ziyaretci", 1).Wait; //gelen data ile ilgilenmiyorsak wait ile çözebiliriz.
            var count = db.StringDecrementAsync("ziyaretci", 1).Result; //gelecek sonuç ile ilgileniyorsak bir değişkene atamalıyız.

            var value2 = db.StringGetRange("name", 0, 3);
            var value3 = db.StringLength("name");

            if (value.HasValue)
            {
                ViewBag.value = value.ToString();
                ViewBag.value2 = value2.ToString();
                ViewBag.value3 = value3.ToString();
            }


            //Byte dosyasını almak istersek
            byte[] resimbyte = default(byte[]);
            db.StringSet("resim", resimbyte);//Artık resim dosyasını binary olarak memory'de tutacak.
            #region description JsonSerialize
            /*Eğer bir class'ı, bir instance'ını memory'de tutmak istersek;
             * JsonSerialize işlemine tabii tuttuktan sonra, string üzerinden serialize edilmiş olan datayı gönderebilirizi. --> db.StringSet("keyAdı", serializeEdilmişOlanDatalar);
             */
            #endregion

            return View();
        } 
    }
}
