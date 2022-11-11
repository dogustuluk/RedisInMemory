using Microsoft.Extensions.Configuration;
using StackExchange.Redis;

namespace RedisExchangeAPI.Web.Services
{
    /// <summary>
    /// Bu sınıf üzerinden istediğimiz veri tabanını alıp, bu veri tabanı üzerinde tüm veri tipleri ile çalışabiliriz.
    /// </summary>
    public class RedisService
    {
        private readonly string _redisHost;
        private readonly string _redisPort;
        private ConnectionMultiplexer _redis; //bu class üzerinden redis server ile haberleşilecek.
        public IDatabase db { get; set; }
        public RedisService(IConfiguration configuration)
        {
            _redisHost = configuration["Redis:Host"];
            _redisPort = configuration["Redis:Port"];
        }

        /// <summary>
        /// Herhangi bir geri dönüş tipi olmamakla beraber redis service ile haberleşecek olan metot. Uygulama ayağa kalktığında çağırılması gerekmektedir. Middleware olarak eklenir.
        /// </summary>
        public void Connect()
        {

            var configString = $"{_redisHost}:{_redisPort}";

            _redis = ConnectionMultiplexer.Connect(configString);

        }
        /// <summary>
        /// Veri tabanını alan metot.
        /// </summary>
        /// <param name="db"></param>
        /// <returns></returns>
        public IDatabase GetDb(int db)
        {
            return _redis.GetDatabase(db);
        }
    }
}
