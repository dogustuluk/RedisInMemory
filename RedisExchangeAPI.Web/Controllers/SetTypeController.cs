using Microsoft.AspNetCore.Mvc;
using RedisExchangeAPI.Web.Services;
using StackExchange.Redis;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace RedisExchangeAPI.Web.Controllers
{
    public class SetTypeController : Controller
    {
        private readonly IDatabase db;

        private readonly RedisService _redisService;

        private string listKey = "hashnames"; //setnames olması daha doğru olur.

        public SetTypeController(RedisService redisService)
        {
            _redisService = redisService;
            db = _redisService.GetDb(0);
        }
        public IActionResult Index()
        {
            #region HashSet
            //Normal bir List'ten farkı; içerisindeki değerler unique olmakla beraber sırasız bir şekilde tutulmaktadır.
            #endregion
            HashSet<string> namesList = new HashSet<string>();

            if (db.KeyExists(listKey))
            {
                db.SetMembers(listKey).ToList().ForEach(x =>
                {
                    namesList.Add(x.ToString());
                });
            }

            return View(namesList);
        }
        [HttpPost]
        public IActionResult Add(string name)
        {
            #region Absolut Expression
            //eğer absolute expression şeklinde olmasını istiyorsak;
            //if (!db.KeyExists(listKey))
            //{
            //    db.KeyExpire(listKey, DateTime.Now.AddMinutes(5));

            //    //Eğer key varsa tekrardan süre eklemez.
            //}
            #endregion
            #region SlidingExpressionOnRedis
            /* Redis tarafında sliding expression özelliği yoktur. Absolute expression bulunmaktadır.
             * Fakat istersek aşağıdaki gibi bir davranış yapmasını sağlayarak sliding expression özelliğini kazandırmış oluruz.
             * Bu şekilde Add metoduna her istek yapıldığında mevcutta olan süreye istediğimiz kadar ek yapılmasını sağlayabiliriz.
             */
            #endregion
            db.KeyExpire(listKey, DateTime.Now.AddMinutes(5));

            db.SetAdd(listKey, name);


            return RedirectToAction("Index");
        }
        public async Task<IActionResult> DeleteItem(string name)
        {
            await db.SetRemoveAsync(listKey, name);

            return RedirectToAction("Index");
        }
    }
}
