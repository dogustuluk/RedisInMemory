using Microsoft.EntityFrameworkCore;
using RedisExampleApp.API.Models;
using RedisExampleApp.API.Repositories;
using RedisExampleApp.Cache;
using StackExchange.Redis;

var builder = WebApplication.CreateBuilder(args);

// Add services to the container.

builder.Services.AddControllers();
// Learn more about configuring Swagger/OpenAPI at https://aka.ms/aspnetcore/swashbuckle
builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen();

builder.Services.AddScoped<IProductRepository, ProductRepository>();
builder.Services.AddDbContext<AppDbContext>(options =>
{
    options.UseInMemoryDatabase("myDatabase");
});


#region sp=>
/*
 * RedisService url'i d��ar�dan ald��� i�in kendi nesne �rne�imizi kendimizin olu�turmas� gerekmektedir. Bundan dolay� burada sp(service provider) ile i�eri gireriz.
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
 * in-memory �zerinden seed datalar� g�rmek i�in veri taban�na ba�lan�p ensure created yapmam�z gerekir. Bu sadece in-memory ile �al���nca yap�lmas� gereken bir durum. Bu kodlar ile database'i uygulama her aya�a kalkt���nda s�f�rdan olu�turur.
 * using ile garbage collector'a ihtiya� duymadan memory'den silinmesi i�in kulland�k.
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
