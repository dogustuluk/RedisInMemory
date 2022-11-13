using Microsoft.EntityFrameworkCore;
using RedisExampleApp.API.Models;
using RedisExampleApp.API.Repositories;
using RedisExampleApp.API.Services;
using RedisExampleApp.Cache;
using StackExchange.Redis;

var builder = WebApplication.CreateBuilder(args);

// Add services to the container.

builder.Services.AddControllers();
// Learn more about configuring Swagger/OpenAPI at https://aka.ms/aspnetcore/swashbuckle
builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen();

builder.Services.AddScoped<IProductService,ProductService>();

builder.Services.AddScoped<IProductRepository>(sp =>
{
    var appDbContext = sp.GetRequiredService<AppDbContext>();
    
    var productRepository = new ProductRepository(appDbContext);

    var redisService = sp.GetRequiredService<RedisService>();

    return new ProductRepositoryWithCacheDecorator(productRepository, redisService);
});
builder.Services.AddDbContext<AppDbContext>(options =>
{
    options.UseInMemoryDatabase("myDatabase");
});


#region sp=>
/*
 * RedisService url'i dýþarýdan aldýðý için kendi nesne örneðimizi kendimizin oluþturmasý gerekmektedir. Bundan dolayý burada sp(service provider) ile içeri gireriz.
 */
#endregion
builder.Services.AddSingleton<RedisService>(sp =>
{
    return new RedisService(builder.Configuration["CacheOptions:Url"]);
});

builder.Services.AddSingleton<IDatabase>(sp =>
{
    var redisService = sp.GetRequiredService<RedisService>();

    return redisService.GetDb(0);
});


var app = builder.Build();

#region Scope
/*
 * in-memory üzerinden seed datalarý görmek için veri tabanýna baðlanýp ensure created yapmamýz gerekir. Bu sadece in-memory ile çalýþýnca yapýlmasý gereken bir durum. Bu kodlar ile database'i uygulama her ayaða kalktýðýnda sýfýrdan oluþturur.
 * using ile garbage collector'a ihtiyaç duymadan memory'den silinmesi için kullandýk.
 */
#endregion
using (var scope = app.Services.CreateScope())
{
    var dbContext = scope.ServiceProvider.GetRequiredService<AppDbContext>();
    dbContext.Database.EnsureCreated();
}

// Configure the HTTP request pipeline.
if (app.Environment.IsDevelopment())
{
    app.UseSwagger();
    app.UseSwaggerUI();
}

app.UseHttpsRedirection();

app.UseAuthorization();

app.MapControllers();

app.Run();
