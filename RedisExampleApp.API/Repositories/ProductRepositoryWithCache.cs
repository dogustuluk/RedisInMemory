using RedisExampleApp.API.Models;
using RedisExampleApp.Cache;
using StackExchange.Redis;
using System.Text.Json;

namespace RedisExampleApp.API.Repositories
{
    /// <summary>
    /// Decorator design pattern ile IProductRepository'yi implemente edecek fakat dataların cache'ten geleceği sınıftır. Burada database ile ilgili bir işlem olduğu için ilgili Repository interface'i alınacaktır. Service ya da Controller katmanlarından ilgili sınıf alınamaz. 
    /// </summary>
    public class ProductRepositoryWithCacheDecorator : IProductRepository
    {
        private const string productKey = "productCaches";
        private readonly IProductRepository _productRepository;
        private readonly RedisService _redisService;
        private readonly IDatabase _cacheRepository;
        public ProductRepositoryWithCacheDecorator(IProductRepository productRepository, RedisService redisService)
        {
            _productRepository = productRepository;
            _redisService = redisService;
            _cacheRepository = _redisService.GetDb(1);
        }
        /// <summary>
        /// Eğer varsa hem gerçek db'ye eklemesi hem de cache'e eklemesi gerekiyor. 
        /// </summary>
        /// <param name="product"></param>
        /// <returns></returns>
        public async Task<Product> CreateAsync(Product product)
        {
            var newProduct = await _productRepository.CreateAsync(product); //gerçek db'ye eklendi.

            //data cacheRepository'de olmayabilir, bunun kontrolü yapılır.
            if (await _cacheRepository.KeyExistsAsync(productKey))
            {
                //cache'e ekle
                await _cacheRepository.HashSetAsync(productKey, product.Id, JsonSerializer.Serialize(newProduct));
            }

            return newProduct;
        }

        public async Task<List<Product>> GetAsync()
        {
            //cache'te data yoksa, datayı cache'le.
            if (!await _cacheRepository.KeyExistsAsync(productKey))
                return await LoadToCacheFromDbAsync();

            //data varsa cache'lenmiş datayı dön.
            var products = new List<Product>();

            //cache'lenmiş datayı alır.
            var cacheProducts = await _cacheRepository.HashGetAllAsync(productKey);
            foreach (var item in cacheProducts.ToList())
            {
                var product = JsonSerializer.Deserialize<Product>(item.Value);

                products.Add(product);
            }

            return products;


        }

        public async Task<Product> GetByIdAsync(int id)
        {
            if (await _cacheRepository.KeyExistsAsync(productKey))
            {
                var product = await _cacheRepository.HashGetAsync(productKey, id);

                return product.HasValue ? JsonSerializer.Deserialize<Product>(product) : null;
            }

            //eğer data yok ise datayı cache'e yükle.
            var products = await LoadToCacheFromDbAsync();

            return products.FirstOrDefault(x => x.Id == id);
        }
        /// <summary>
        /// Datayı cache'ten alıyor ve geriye List Product tipinde dönüyor.
        /// </summary>
        /// <returns></returns>
        private async Task<List<Product>> LoadToCacheFromDbAsync()
        {
            //tüm datayı db'den al.
            var products = await _productRepository.GetAsync();

            //cache'le
            products.ForEach(product =>
            {
                _cacheRepository.HashSetAsync(productKey, product.Id, JsonSerializer.Serialize(product));
            });

            return products;
        }

    }
}
