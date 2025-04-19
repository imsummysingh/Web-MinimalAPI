using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Web_MinimalAPI.Models;

var builder = WebApplication.CreateBuilder(args);

// Add services to the container.

builder.Services.AddControllers().ConfigureApiBehaviorOptions(options =>
{
    options.SuppressModelStateInvalidFilter= true;
});

// Learn more about configuring Swagger/OpenAPI at https://aka.ms/aspnetcore/swashbuckle
builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen();

//need to tell application to use the shopContext for inMemory database
builder.Services.AddDbContext<ShopContext>(options =>
{
    options.UseInMemoryDatabase("Shop");
});

var app = builder.Build();

// Configure the HTTP request pipeline.
if (app.Environment.IsDevelopment())
{
    app.UseSwagger();
    app.UseSwaggerUI();
}

app.UseHttpsRedirection();

app.UseAuthorization();

app.MapControllers();

//minimal API Code

using (var scope = app.Services.CreateScope())
{
    var db = scope.ServiceProvider.GetRequiredService<ShopContext>();
    await db.Database.EnsureCreatedAsync();
}

//getAllProducts
app.MapGet("/products", async (ShopContext _shopContext) =>
{
    return await _shopContext.Products.ToArrayAsync();
});

//get product by Id
app.MapGet("/products/{id}", async (int id,ShopContext _shopContext) =>
{
    var product = await _shopContext.Products.FindAsync(id);
    if(product == null)
    {
        return Results.NotFound();
    }
    return Results.Ok(product);
});

//get available Products
app.MapGet("/products/available", async (ShopContext _shopContext) =>
{
    return Results.Ok(await _shopContext.Products.Where(p => p.IsAvailable).ToArrayAsync());
});

//Create Product
app.MapPost("/products", async (ShopContext _shopContext, Product product) =>
{
    _shopContext.Products.Add(product);
    await _shopContext.SaveChangesAsync();

    return Results.CreatedAtRoute("GetProduct", new { id = product.Id }, product);
});


//update product
app.MapPut("/products/{id}", async (ShopContext _context, int id, Product product) =>
{
    if (id != product.Id)
    {
        return Results.BadRequest();
    }

    _context.Entry(product).State = EntityState.Modified;

    try
    {
        await _context.SaveChangesAsync();
    }
    catch (DbUpdateConcurrencyException)
    {
        if (!_context.Products.Any(p => p.Id == id))
        {
            return Results.NotFound();
        }
        else
        {
            throw;
        }
    }

    return Results.NoContent();
});


//delete prodduct
app.MapDelete("/products/{id}", async (ShopContext _context, int id) =>
{
    var product = await _context.Products.FindAsync(id);
    if (product == null)
    {
        return Results.NotFound();
    }

    _context.Products.Remove(product);
    await _context.SaveChangesAsync();

    return Results.Ok(product);
});

//delete multiple products
app.MapPost("/products/Delete", async (ShopContext _context, [FromQuery] int[] ids) =>
{
    var products = new List<Product>();
    foreach (var id in ids)
    {
        var product = await _context.Products.FindAsync(id);

        if (product == null)
        {
            return Results.NotFound();
        }

        products.Add(product);
    }

    _context.Products.RemoveRange(products);
    await _context.SaveChangesAsync();

    return Results.Ok(products);
});

app.Run();
