using ClothingStore.WebApi.Helpers;
using ClothingStore.WebApi.Models;
using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace ClothingStore.WebApi.Services
{
    public interface IProductPiecesAvailableService
    {
        void AddPieces(Product product, int value, StoreContext context);
    }

    public class ProductPiecesAvailableService : IProductPiecesAvailableService
    {
        static Dictionary<int, object> _lockListProduct = new Dictionary<int, object>();
        private static object _lock = new object();

        public void AddPieces(Product product, int value, StoreContext context)
        {
            bool saveFailed;
            if (product == null)
            {
                throw new BadRequestException(Enum.InternalCode.NotFound, $"productId {product.Id} not found.");
            }
            do
            {
                saveFailed = false;
                try
                {
                    object _lockProduct = GetProductLocker(product.Id);
                    lock (_lockProduct)
                    {
                        SetAvailable(product, value);
                        context.SaveChanges();
                    }
                }
                catch (DbUpdateConcurrencyException ex)
                {
                    saveFailed = true;

                    var entry = ex.Entries.Single();
                    var databaseValues = entry.GetDatabaseValues();

                    // Update the original values with the database values 
                    entry.OriginalValues.SetValues(databaseValues);
                }
                catch (Exception)
                {
                    throw;
                }
            } while (saveFailed);

        }

        private static void SetAvailable(Product product, int value)
        {
            if (value < 0)
            {
                if (product.PiecesAvailable < (value * -1))
                {
                    throw new BadRequestException(Enum.InternalCode.OutOfStock, $"Currently only {product.PiecesAvailable} available  in stock for this product.");
                }
            }
            product.PiecesAvailable += value;
        }

        private static object GetProductLocker(int ProductId)
        {
            lock (_lock)
            {
                object _lockProduct = null;
                if (_lockListProduct.ContainsKey(ProductId))
                {
                    _lockProduct = _lockListProduct[ProductId];
                }
                else
                {
                    _lockProduct = new object();
                    _lockListProduct.Add(ProductId, _lockProduct);
                }
                return _lockProduct;
            }
        }
    }
}
