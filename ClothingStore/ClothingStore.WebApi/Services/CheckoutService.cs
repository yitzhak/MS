using ClothingStore.WebApi.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using ClothingStore.WebApi.Helpers;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging;

namespace ClothingStore.WebApi.Services
{
    public interface ICheckoutService
    {
        Task<int> AddressAndPayment([FromForm] Order order);
    }

    public class CheckoutService : ICheckoutService
    {
        private readonly StoreContext db;
        private readonly IBaseServise _baseServise;
        private readonly IProductPiecesAvailableService _productPiecesAvailableService;
        private readonly IPaymentValidatorService _paymentValidatorService;

        private List<Product> products;

        public CheckoutService(StoreContext context,
            IBaseServise baseServise,
            IProductPiecesAvailableService productPiecesAvailableService,
            IPaymentValidatorService paymentValidatorService)
        {
            db = context;
            _baseServise = baseServise;
            _productPiecesAvailableService = productPiecesAvailableService;
            _paymentValidatorService = paymentValidatorService;
        }


        public async Task<int> AddressAndPayment([FromForm] Order order)
        {
            try
            {
                decimal paymentSum = order.OrderPayments.Sum(p => p.Amount);
                List<Product> products = await GetProductsForOrder(order);

                SetOrderItemsDetails(order, products);
                await ValidatePaymentExist(order);
                ValidatePayment(order, paymentSum);

                //TODO all changes in Database is wrap in a Transaction Middleware
                await CheckIfInStockAndSetAvailablePieces(order, products);

                //order.Username = HttpContext.User.Identity.Name;
                order.SalesmanId = _baseServise.UserId;
                order.OrderDate = DateTime.UtcNow;

                //Add the Order
                await db.Orders.AddAsync(order);

                _baseServise.Logger.Info("User {userName} started checkout of {orderId}.", order.Username, order.Id);

                // Save all changes
                await db.SaveChangesAsync();

                return order.Id;

            }
            catch (Exception ex)
            {
                _baseServise.Logger.Error("User {userName} Throw exception in checkout {Exception}.", order.Username, ex);

                //Invalid - redisplay with errors
                throw;
            }
        }

        private async Task ValidatePaymentExist(Order order)
        {
            foreach (OrderPayment payment in order.OrderPayments)
            {
                switch (payment.PaymentType)
                {
                    //case Enum.PaymentType.Check:
                    //case Enum.PaymentType.CreditCard:
                    case Enum.PaymentType.Coupon:
                        bool isValid = await _paymentValidatorService.ValidatCoupon(payment.Amount, payment.PaymentIdentity);
                        ThrowIfInvalid(payment, isValid);
                        break;
                    default:
                        break;
                }
            }
        }

        private static void ThrowIfInvalid(OrderPayment payment, bool isValid)
        {
            if (!isValid)
            {
                throw new BadRequestException(Enum.InternalCode.ValidationException, $"your payment {payment.PaymentType} is invalid");
            }
        }

        private async Task CheckIfInStockAndSetAvailablePieces(Order order, List<Product> products)
        {
            foreach (var item in order.OrderItems)
            {
                Product product = products.FirstOrDefault(p => p.Id == item.ProductId);
                if (product == null)
                {
                    throw new BadRequestException(Enum.InternalCode.NotFound, $"productId {item.ProductId} not found.");
                }
                _productPiecesAvailableService.AddPieces(product, item.Quantity * -1, db);
            }
        }

        private async Task<List<Product>> GetProductsForOrder(Order order)
        {
            if (products != null)
            {
                return products;
            }
            List<int> productIds = order.OrderItems.Select(p => p.ProductId).Distinct().ToList();
            products = await db.Products.Where(p => productIds.Contains(p.Id)).ToListAsync();
            return products;
        }

        private static void SetOrderItemsDetails(Order order, List<Product> products)
        {
            foreach (OrderItem item in order.OrderItems)
            {
                Product product = products.FirstOrDefault(p => p.Id == item.ProductId);
                if (product == null)
                {
                    throw new BadRequestException(Enum.InternalCode.NotFound, $"productId {item.ProductId} not found.");
                }
                item.UnitPrice = product.Price;
                item.CanBeReturned = product.CanBeReturned;
            }
        }

        private static void ValidatePayment(Order order, decimal paymentSum)
        {
            if (paymentSum < order.Total)
            {
                throw new BadRequestException(Enum.InternalCode.ValidationException, $"the payment {paymentSum} is less from {order.Total}.");
            }
            else if (paymentSum > order.Total)
            {
                throw new BadRequestException(Enum.InternalCode.ValidationException, $"the payment {paymentSum} is Higher from {order.Total}.");
            }
        }

    }
}
