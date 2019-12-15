using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Http;
using Microsoft.EntityFrameworkCore;
using ClothingStore.WebApi.Models;

namespace ClothingStore.WebApi.Services
{
    public interface IProductsService
    {
        Task<bool> AddAvailablePieces(int id, int value);
        Task<bool> DeleteProduct(int id);
        Task<Product> GetProduct(int id);
        Task<List<Product>> GetProducts();
        Task<int> PostProduct(Product product);
        Task<bool> ProductExists(int id);
        Task PutProduct(int id, Product product);
    }

    public class ProductsService : IProductsService
    {
        private readonly StoreContext _context;
        private readonly IProductPiecesAvailableService _piecesAvailableService;

        public ProductsService(StoreContext context,
            IProductPiecesAvailableService piecesAvailableService)
        {
            _context = context;
            _piecesAvailableService = piecesAvailableService;
        }

        public async Task<List<Product>> GetProducts()
        {
            return await _context.Products.ToListAsync();
        }

        public async Task<Product> GetProduct(int id)
        {
            var product = await _context.Products.FindAsync(id);
            return product;
        }

        public async Task PutProduct(int id, Product product)
        {
            var DBProduct = await _context.Products.FindAsync(id);

            DBProduct.Title = product.Title;
            DBProduct.CanBeReturned = product.CanBeReturned;
            DBProduct.Price = product.Price;
            DBProduct.ProductUrl = product.ProductUrl;
            DBProduct.Updated = DateTime.UtcNow;

            await _context.SaveChangesAsync();
        }

        /// <summary>
        /// value to add Available Pieces This can be a negative number
        /// </summary>
        /// <param name="id">product id</param>
        /// <param name="value">value to add This can be a negative number</param>
        /// <returns></returns>
        public async Task<bool> AddAvailablePieces(int id, int value)
        {
            var DBProduct = await _context.Products.FindAsync(id);
            _piecesAvailableService.AddPieces(DBProduct, value, _context);
            DBProduct.Updated = DateTime.UtcNow;

            await _context.SaveChangesAsync();
            return true;
        }

        public async Task<int> PostProduct(Product product)
        {
            _context.Products.Add(product);
            await _context.SaveChangesAsync();
            return product.Id;
        }

        public async Task<bool> DeleteProduct(int id)
        {
            var product = await _context.Products.FindAsync(id);
            _context.Products.Remove(product);
            await _context.SaveChangesAsync();

            return true;
        }

        public Task<bool> ProductExists(int id)
        {
            return _context.Products.AnyAsync(e => e.Id == id);
        }
    }
}
