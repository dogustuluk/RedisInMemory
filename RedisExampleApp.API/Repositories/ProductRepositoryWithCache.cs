﻿using RedisExampleApp.API.Models;
using RedisExampleApp.Cache;

namespace RedisExampleApp.API.Repositories
{
    /// <summary>
    /// Decorator design pattern ile IProductRepository'yi implemente edecek fakat dataların cache'ten geleceği sınıftır.
    /// </summary>
    public class ProductRepositoryWithCache : IProductRepository
    {
        private readonly IProductRepository _productRepository;
        private readonly RedisService _redisService;
        public ProductRepositoryWithCache(IProductRepository productRepository, RedisService redisService)
        {
            _productRepository = productRepository;
            _redisService = redisService;
        }

        public Task<Product> CreateAsync(Product product)
        {
            throw new NotImplementedException();
        }

        public Task<List<Product>> GetAsync()
        {
            throw new NotImplementedException();
        }

        public Task<Product> GetByIdAsync(int id)
        {
            throw new NotImplementedException();
        }
    }
}
