using Microsoft.EntityFrameworkCore;
using RedisExampleApp.API.Models;
using RedisExampleApp.API.Repositories;

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
