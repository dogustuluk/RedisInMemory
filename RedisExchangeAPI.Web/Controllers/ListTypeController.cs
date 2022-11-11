using Microsoft.AspNetCore.Mvc;
using RedisExchangeAPI.Web.Services;
using StackExchange.Redis;
using System.Collections.Generic;
using System.Linq;

namespace RedisExchangeAPI.Web.Controllers
{
    public class ListTypeController : Controller
    {
        private readonly IDatabase db;

        private readonly RedisService _redisService;

        private string listKey = "names";

        public ListTypeController(RedisService redisService)
        {
            _redisService = redisService;
            this.db = _redisService.GetDb(0);
        }

        public IActionResult Index()
        {
            List<string> namesList = new List<string>();
            if (db.KeyExists(listKey))
            {
                db.ListRange(listKey).ToList().ForEach(x =>
                {
                    namesList.Add(x.ToString());
                });//eğer başlangıç ve bitiş değeri vermezsek baştan sona tüm dataları okur.
            }
            return View(namesList);
        }
        [HttpPost]
        public IActionResult Add(string name)
        {
            db.ListRightPush(listKey, name);

            return RedirectToAction("Index");
        }
        public IActionResult DeleteItem(string name)
        {
            db.ListRemoveAsync(listKey,name).Wait();
            return RedirectToAction("Index");
        }
        public IActionResult DeleteFirstItem()
        {
            db.ListLeftPopAsync(listKey).Wait();//left pop ile baştaki değeri siler.
            return RedirectToAction("Index");
        }
    }
}
