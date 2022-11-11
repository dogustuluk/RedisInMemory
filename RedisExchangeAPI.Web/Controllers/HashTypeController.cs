using Microsoft.AspNetCore.Mvc;
using RedisExchangeAPI.Web.Services;
using StackExchange.Redis;
using System.Collections.Generic;
using System.Linq;

namespace RedisExchangeAPI.Web.Controllers
{
    public class HashTypeController : Controller
    {
        private readonly IDatabase db;

        private readonly RedisService _redisService;

        public string listKey = "hashnames"; //setnames olması daha doğru olur.
        public string hashKey { get; set; } = "sozluk";

        public HashTypeController(RedisService redisService)
        {
            _redisService = redisService;
            db = _redisService.GetDb(0);
        }
        public IActionResult Index()
        {
            Dictionary<string, string> list = new Dictionary<string, string>();

            if (db.KeyExists(hashKey))
            {
                db.HashGetAll(hashKey).ToList().ForEach(x =>
                {
                    list.Add(x.Name, x.Value);
                }); 
            }

            return View(list);
        }
        [HttpPost]
        public IActionResult Add(string name, string val)
        {
            db.HashSet(hashKey, name, val);

            return RedirectToAction("Index");
        }
        public IActionResult DeleteItem(string name)
        {
            db.HashDelete(hashKey, name);

            return RedirectToAction("Index");
        }
    }
}
