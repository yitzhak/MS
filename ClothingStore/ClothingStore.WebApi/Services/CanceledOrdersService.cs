using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Http;
using Microsoft.EntityFrameworkCore;
using ClothingStore.WebApi.Models;
using ClothingStore.WebApi.Helpers;
using ClothingStore.WebApi.DTO;
using ClothingStore.WebApi.Enum;

namespace ClothingStore.WebApi.Services
{


    public interface ICanceledOrdersService
    {
        Task<List<CanceledOrder>> GetCanceledOrderByDate(DateTime startDate, DateTime endDate);
        Task<CustomerPaybackDTO> PostCanceledOrder(CanceledOrder canceledOrder);
    }

    public class CanceledOrdersService : ICanceledOrdersService
    {
        private readonly StoreContext _context;
        private readonly ICouponsService _couponsService;
        private readonly IBaseServise _baseServise;

        public CanceledOrdersService(StoreContext context,
            ICouponsService couponsService,
            IBaseServise baseServise)
        {
            _context = context;
            _couponsService = couponsService;
            _baseServise = baseServise;
        }

        public async Task<List<CanceledOrder>> GetCanceledOrderByDate(DateTime startDate, DateTime endDate)
        {
            return await _context.CanceledOrders
                .Include(p => p.Salesman)
                .Include("ReturnedItems.OrderItem")
                .Where(p => p.CanceledDate >= startDate &&
                p.CanceledDate < endDate)
                .ToListAsync();
        }

        public async Task<CustomerPaybackDTO> PostCanceledOrder(CanceledOrder canceledOrder)
        {
            CustomerPaybackDTO customerPayback = new CustomerPaybackDTO();
            if (!canceledOrder.ReturnedItems.Any())
            {
                throw new BadRequestException(Enum.InternalCode.ValidationException, "To cancel order you must add 'ReturnedItems'");
            }
            List<int> OrderItemIds = canceledOrder.ReturnedItems.Select(p => p.OrderItemId).Distinct().ToList();
            List<OrderItem> orderItems = await _context.OrderItems
                .Include(p => p.Product)
                .Where(p => OrderItemIds.Contains(p.Id)).ToListAsync();
            SetOrderItem(canceledOrder, orderItems);

            CheckForBelongItemsToOrder(canceledOrder, orderItems);
            CheckForItemsCantReturn(orderItems);
            await CheckQuantityOfReturnItems(canceledOrder, orderItems);

            Order order = await _context.Orders.FindAsync(canceledOrder.OrderId);
            if ((DateTime.UtcNow - order.OrderDate).TotalDays >= 30)
            {
                throw new BadRequestException(Enum.InternalCode.DateOver, $"you can return an item only before 30 days from the order, order date {order.OrderDate}");
            }


            customerPayback.Amount = canceledOrder.ReturnedItems.Sum(p => p.OrderItem.UnitPrice * p.Quantity);

            if ((DateTime.UtcNow - order.OrderDate).TotalDays < 16)
            {
                //TODO Handling customers who have not paid cash
                customerPayback.PaymentType = PaymentType.Cash;
            }
            else
            {
                Coupon coupon = await _couponsService.CreateCoupon(customerPayback.Amount, canceledOrder.OrderId);
                customerPayback.PaymentType = PaymentType.Coupon;
                customerPayback.Identity = coupon.CouponIdentity;
            }

            canceledOrder.CanceledDate = DateTime.UtcNow;
            canceledOrder.SalesmanId = _baseServise.UserId;
            _context.CanceledOrders.Add(canceledOrder);
            await _context.SaveChangesAsync();

            return customerPayback;
        }

        private static void SetOrderItem(CanceledOrder canceledOrder, List<OrderItem> orderItems)
        {
            foreach (ReturnedItem item in canceledOrder.ReturnedItems)
            {
                item.OrderItem = orderItems.SingleOrDefault(p => p.Id == item.OrderItemId);
            }
        }

        private static void CheckForBelongItemsToOrder(CanceledOrder canceledOrder, List<OrderItem> orderItems)
        {
            if (orderItems.Any(p => p.OrderId != canceledOrder.OrderId))
            {
                List<int> notBelongItemsToOrder = orderItems
                    .Where(p => p.OrderId != canceledOrder.OrderId)
                    .Select(p => p.Id).ToList();
                throw new BadRequestException(Enum.InternalCode.ValidationException,
                    $"the above items {String.Join(",", notBelongItemsToOrder)} do not belong to the order ");
            }
        }

        private static void CheckForItemsCantReturn(List<OrderItem> orderItems)
        {
            if (orderItems.Any(p => !p.CanBeReturned))
            {
                List<int> notBelongItemsToOrder = orderItems
                    .Where(p => !p.CanBeReturned)
                    .Select(p => p.Id).ToList();
                throw new BadRequestException(Enum.InternalCode.ValidationException,
                    $"the above items {string.Join(",", notBelongItemsToOrder)} Can't Be Return.");
            }
        }

        /// <summary>
        /// Check Quantity Of an Items is valid for the bought, and history of returns
        /// </summary>
        /// <param name="canceledOrder"></param>
        /// <param name="orderItems"></param>
        private async Task CheckQuantityOfReturnItems(CanceledOrder canceledOrder, List<OrderItem> orderItems)
        {
            var lookUpItems = canceledOrder.ReturnedItems.ToLookup(p => p.OrderItemId);
            List<CanceledOrder> canceledHistory = await _context.CanceledOrders
                .Include(p => p.ReturnedItems)
                .Where(p => p.OrderId == canceledOrder.OrderId).ToListAsync();
            foreach (var item in lookUpItems)
            {
                int orderItemId = item.Key;
                int Quantity = item.Sum(p => p.Quantity);
                OrderItem orderItem = orderItems.SingleOrDefault(p => p.Id == orderItemId);
                int returnHistory = canceledHistory.Sum(p => p.ReturnedItems.Where(x => x.OrderItemId == orderItemId).Sum(x => x.Quantity));
                int orderItemQuantity = orderItem.Quantity - returnHistory;
                if (orderItemQuantity < Quantity)
                {
                    throw new BadRequestException(Enum.InternalCode.ValidationException,
                    $"you Can't Be Return {Quantity} of {orderItem.Product.Title} because you bought only {orderItemQuantity}.");
                }
            }
        }

    }
}
