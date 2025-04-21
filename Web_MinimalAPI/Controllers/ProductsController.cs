using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Web_MinimalAPI.Models;

namespace Web_MinimalAPI.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class ProductsController : ControllerBase
    {
        private readonly ShopContext _shopContext;

        public ProductsController(ShopContext shopContext)
        {
            _shopContext = shopContext;

            //to ensure the database is created
            _shopContext.Database.EnsureCreated();
        }


        //Approach 1->of returning list of items
        //[HttpGet]
        //public IEnumerable<Product> GetAllProducts()
        //{
        //    return _shopContext.Products.ToArray();
        //}


        //Approach 2->ActionResult
        //[HttpGet]
        //public ActionResult GetAllProducts()
        //{
        //    return Ok(_shopContext.Products.ToArray());
        //}


        //working code: for GetAllProducts products V1
        //Approach 3-> Asynchronous ActionResult
        //[HttpGet]
        //public async Task<ActionResult> GetAllProducts()
        //{
        //    return Ok(await _shopContext.Products.ToArrayAsync());
        //}


        //commented the above code because adding pagination in this method
        //adding pagination - v2
        //commenting this code-> because another method added as v3 for Filtering data
        //[HttpGet]
        //public async Task<ActionResult> GetAllProducts([FromQuery]QueryParameters queryParameters)
        //{
        //    IQueryable<Product> products = _shopContext.Products;   //iqueryable used, so we can query on the products for the pagination

        //    products = products.Skip(queryParameters.Size * (queryParameters.Page - 1)).Take(queryParameters.Size);

        //    return Ok(await products.ToArrayAsync());
        //}


        //this method is modified further more to add filtering on data
        [HttpGet]
        public async Task<ActionResult> GetAllProducts([FromQuery] ProductQueryParameter queryParameters)
        {
            IQueryable<Product> products = _shopContext.Products;   //iqueryable used, so we can query on the products for the pagination

            //code for filtering data
            //this code through out products whose price is lower than minPrice
            if (queryParameters.MinPrice != null)
            {
                products = products.Where(p => p.Price >= queryParameters.MinPrice.Value);  
            }

            //for max price
            if(queryParameters.MaxPrice != null)
            {
                products = products.Where(p => p.Price <= queryParameters.MaxPrice.Value);
            }

            products = products.Skip(queryParameters.Size * (queryParameters.Page - 1)).Take(queryParameters.Size);

            return Ok(await products.ToArrayAsync());
        }


        [HttpGet("{id}")]
        public async Task<ActionResult> GetProduct(int id)
        {
            var product = await _shopContext.Products.FindAsync(id);
            if(product == null)
            {
                return NotFound();
            }
            return Ok(product);
        }

        //method for checking Available Products
        [HttpGet("available")]
        public async Task<ActionResult<IEnumerable<Product>>> GetAvailableProducts()
        {
            return await _shopContext.Products.Where(p => p.IsAvailable).ToArrayAsync();
        }


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
            }catch (DbUpdateConcurrencyException ex)
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

        //delete an item from product
        [HttpDelete("{id}")]
        public async Task<ActionResult> DeleteProduct(int id)
        {
            var product = await _shopContext.Products.FindAsync(id);
            if(product == null)
            {
                return NotFound();
            }

            _shopContext.Products.Remove(product);
            await _shopContext.SaveChangesAsync();

            return Ok(product);
        }

        //delete multiple products
        [HttpDelete("Delete")]
        public async Task <ActionResult> DeleteMultipleProducts([FromQuery]int[] ids)
        {
            var products = new List<Product>();
            foreach (var id in ids)
            {
                var product = await _shopContext.Products.FindAsync(id);

                if(product == null)
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
