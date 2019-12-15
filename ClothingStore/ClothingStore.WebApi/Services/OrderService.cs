using ClothingStore.WebApi.Models;
using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace ClothingStore.WebApi.Services
{
    public interface IOrderService
    {
        Task<List<Order>> GetOrderByDate(DateTime startDate, DateTime endDate);
    }

    public class OrderService : IOrderService
    {
        private readonly StoreContext _context;

        public OrderService(StoreContext context)
        {
            _context = context;
        }

        public async Task<List<Order>> GetOrderByDate(DateTime startDate, DateTime endDate)
        {
            return await _context.Orders
                .Include(p => p.Salesman)
                .Include(p => p.OrderItems)
                .Where(p => p.OrderDate >= startDate &&
                p.OrderDate < endDate)
                .ToListAsync();
        }
    }
}
