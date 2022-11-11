using Microsoft.AspNetCore.Mvc;
using RedisExchangeAPI.Web.Services;
using StackExchange.Redis;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace RedisExchangeAPI.Web.Controllers
{
    public class SortedSetTypeController : Controller
    {
        private readonly IDatabase db;

        private readonly RedisService _redisService;

        private string listKey = "sortedsetnames";

        public SortedSetTypeController(RedisService redisService)
        {
            _redisService = redisService;
            db = _redisService.GetDb(0);
        }
        public IActionResult Index()
        {
            HashSet<string> list = new HashSet<string>();

            if (db.KeyExists(listKey))
            {
                #region SortedSetScan
                //cache'te nasıl tutuluyorsa o sıraya göre gelir.
                #endregion
                //db.SortedSetScan(listKey).ToList().ForEach(x =>
                //{
                //    list.Add(x.ToString());

                //});


                //sıralama yaparak listelemek istersek kullanırız fakat burada score değeri gelmez.
                //db.SortedSetRangeByRank(listKey, order: Order.Descending).ToList().ForEach(x =>
                //{
                //    list.Add(x.ToString());
                //});

                db.SortedSetRangeByRankWithScores(listKey, order: Order.Descending).ToList().ForEach(x =>
                {
                    list.Add(x.ToString());
                });

            }


            return View(list);
        }
        [HttpPost]
        public IActionResult Add(string name, int score)
        {
            db.SortedSetAdd(listKey, name, score);

            db.KeyExpire(listKey, DateTime.Now.AddMinutes(1));

            return RedirectToAction("Index");
        }
        public async Task<IActionResult> DeleteItem(string name)
        {
            await db.SortedSetRemoveAsync(listKey, name);

            return RedirectToAction("Index");
        }
    }
}
