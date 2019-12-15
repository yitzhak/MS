using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using ClothingStore.WebApi.Models;
using ClothingStore.WebApi.Services;
using ClothingStore.WebApi.Attributes;
using ClothingStore.WebApi.Enum;

namespace ClothingStore.WebApi.Controllers
{
    [Authorize(AccessLevel.Salesman, AccessLevel.Administrator)]
    [Route("api/[controller]")]
    [ApiController]
    public class ProductsController : ControllerBase
    {
        private readonly IProductsService _productsService;

        public ProductsController(IProductsService productsService)
        {
            _productsService = productsService;
        }

        // GET: api/Products
        [HttpGet]
        public async Task<ActionResult<IEnumerable<Product>>> GetProducts()
        {
            List<Product> products = await _productsService.GetProducts();
            return products;
            //return new ActionResult<IEnumerable<Product>>(products);
        }

        // GET: api/Products/5
        [HttpGet("{id}")]
        public async Task<ActionResult<Product>> GetProduct(int id)
        {
            var product = await _productsService.GetProduct(id);

            if (product == null)
            {
                return NotFound();
            }

            return product;
        }

        // PUT: api/Products/5
        [HttpPut("{id}")]
        public async Task<IActionResult> PutProduct(int id, Product product)
        {
            if (id != product.Id)
            {
                return BadRequest();
            }

            try
            {
                await _productsService.PutProduct(id, product);
            }
            catch (DbUpdateConcurrencyException)
            {
                if (!await _productsService.ProductExists(id))
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

        // PUT: api/Products/AddAvailablePieces/5
        /// <summary>
        /// value to add Available Pieces This can be a negative number
        /// </summary>
        /// <param name="id">product id</param>
        /// <param name="value">value to add This can be a negative number</param>
        /// <returns></returns>
        [HttpPut("AddAvailablePieces/{id}/{value}")]
        public async Task<IActionResult> AddAvailablePieces(int id, int value)
        {
            try
            {
                await _productsService.AddAvailablePieces(id, value);
            }
            catch (DbUpdateConcurrencyException)
            {
                if (!await _productsService.ProductExists(id))
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


        // POST: api/Products
        [HttpPost]
        public async Task<ActionResult<Product>> PostProduct(Product product)
        {
            int productId = await _productsService.PostProduct(product);
            return CreatedAtAction("GetProduct", new { id = productId }, product);
        }

        // DELETE: api/Products/5
        [HttpDelete("{id}")]
        public async Task<ActionResult<bool>> DeleteProduct(int id)
        {
            if (!await _productsService.ProductExists(id))
            {
                return NotFound();
            }
            bool isSuccess = await _productsService.DeleteProduct(id);
            return isSuccess;
        }
    }
}
