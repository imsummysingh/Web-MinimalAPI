using Asp.Versioning;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Web_MinimalAPI.Models;

namespace Web_MinimalAPI.Controllers
{
    /// <summary>
    /// API Version 2
    /// </summary>

    [ApiVersion("2.0")]
    //[Route("api/[controller]")]
    
    //url versioning
    [Route("api/v2/[controller]")]

    //header versioning
    //[Route("products")]

    [ApiController]
    public class ProductsV2Controller : ControllerBase
    {
        private readonly ShopContext _shopContext;

        public ProductsV2Controller(ShopContext shopContext)
        {
            _shopContext = shopContext;

            //to ensure the database is created
            _shopContext.Database.EnsureCreated();
        }


        //this method is modified further more to add filtering, searching, sorting on data
        //modifying this GetAllProduct method in Version2-> Return only available products
        [HttpGet]
        public async Task<ActionResult> GetAllProducts([FromQuery] ProductQueryParameter queryParameters)
        {
            IQueryable<Product> products = _shopContext.Products.Where(p => p.IsAvailable == true);

            //code for filtering data
            //this code through out products whose price is lower than minPrice
            if (queryParameters.MinPrice != null)
            {
                products = products.Where(p => p.Price >= queryParameters.MinPrice.Value);
            }
            //for max price
            if (queryParameters.MaxPrice != null)
            {
                products = products.Where(p => p.Price <= queryParameters.MaxPrice.Value);
            }


            //code for Searching Item
            //for seraching both sku/name in one search
            if (!string.IsNullOrEmpty(queryParameters.SearchTerm))
            {
                products = products.Where(p => p.Sku.ToLower() == queryParameters.SearchTerm.ToLower() || p.Name.ToLower() == queryParameters.SearchTerm.ToLower());
            }
            //for sku
            if (!string.IsNullOrEmpty(queryParameters.Sku))
            {
                products = products.Where(p => p.Sku == queryParameters.Sku);
            }
            //for name
            if (!string.IsNullOrEmpty(queryParameters.Name))
            {
                products = products.Where(p => p.Name.ToLower().Contains(queryParameters.Name.ToLower()));
            }


            //code for sorting items
            if (!string.IsNullOrEmpty(queryParameters.SortBy))
            {
                if (typeof(Product).GetProperty(queryParameters.SortBy) != null)
                {
                    products = products.OrderByCustom(queryParameters.SortBy, queryParameters.SortOrder);
                }
            }


            products = products.Skip(queryParameters.Size * (queryParameters.Page - 1)).Take(queryParameters.Size);

            return Ok(await products.ToArrayAsync());
        }


        //--------------------------------------------------------------------------------------------------

        [HttpGet("{id}")]
        public async Task<ActionResult> GetProduct(int id)
        {
            var product = await _shopContext.Products.FindAsync(id);
            if (product == null)
            {
                return NotFound();
            }
            return Ok(product);
        }

        //--------------------------------------------------------------------------------------------------

        //method for checking Available Products
        [HttpGet("available")]
        public async Task<ActionResult<IEnumerable<Product>>> GetAvailableProducts()
        {
            return await _shopContext.Products.Where(p => p.IsAvailable).ToArrayAsync();
        }

        //--------------------------------------------------------------------------------------------------


        //create the product
        [HttpPost]
        public async Task<ActionResult> PostProduct(Product product)
        {
            if (!ModelState.IsValid)
            {
                return BadRequest();
            }

            _shopContext.Products.Add(product);
            await _shopContext.SaveChangesAsync();

            return CreatedAtAction(nameof(GetProduct), new { id = product.Id }, product);
        }

        //--------------------------------------------------------------------------------------------------


        //Update the Product
        [HttpPut("{id}")]
        public async Task<ActionResult> PutProduct(int id, Product product)
        {
            if (id != product.Id)
            {
                return BadRequest();
            }

            _shopContext.Entry(product).State = EntityState.Modified;
            try
            {
                await _shopContext.SaveChangesAsync();
            }
            catch (DbUpdateConcurrencyException ex)
            {
                if (!_shopContext.Products.Any(p => p.Id == id))
                {
                    return NotFound();
                }
                else
                {
                    throw;
                }
            }
            return NoContent();
        }

        //--------------------------------------------------------------------------------------------------

        //delete an item from product
        [HttpDelete("{id}")]
        public async Task<ActionResult> DeleteProduct(int id)
        {
            var product = await _shopContext.Products.FindAsync(id);
            if (product == null)
            {
                return NotFound();
            }

            _shopContext.Products.Remove(product);
            await _shopContext.SaveChangesAsync();

            return Ok(product);
        }

        //--------------------------------------------------------------------------------------------------

        //delete multiple products
        [HttpDelete("Delete")]
        public async Task<ActionResult> DeleteMultipleProducts([FromQuery] int[] ids)
        {
            var products = new List<Product>();
            foreach (var id in ids)
            {
                var product = await _shopContext.Products.FindAsync(id);

                if (product == null)
                {
                    return NotFound();
                }
                products.Add(product);
            }

            _shopContext.Products.RemoveRange(products);
            await _shopContext.SaveChangesAsync();

            return Ok(products);
        }

    }
}
