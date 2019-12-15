using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Http;
using Microsoft.EntityFrameworkCore;
using ClothingStore.WebApi.Models;
using ClothingStore.WebApi.DTO;

namespace ClothingStore.WebApi.Services
{
    public interface IStatisticsService
    {
        Task<List<OrderStatisticsDTO>> GetOrderStatisticsForMonth(int month, int year);
    }

    public class StatisticsService : IStatisticsService
    {
        private readonly StoreContext _context;
        private readonly IOrderService _orderService;
        private readonly ICanceledOrdersService _canceledOrdersService;


        public StatisticsService(StoreContext context,
            IOrderService orderService,
            ICanceledOrdersService canceledOrdersService)
        {
            _context = context;
            _orderService = orderService;
            _canceledOrdersService = canceledOrdersService;
        }

        public async Task<List<OrderStatisticsDTO>> GetOrderStatisticsForMonth(int month, int year)
        {
            List<OrderStatisticsDTO> orderStatistics = new List<OrderStatisticsDTO>();
            DateTime startDate = new DateTime(year, month, 1);
            DateTime endDate = startDate.AddMonths(1);

            List<Order> orders = await _orderService.GetOrderByDate(startDate, endDate);
            List<CanceledOrder> canceledOrders = await _canceledOrdersService.GetCanceledOrderByDate(startDate, endDate);

            List<DateTime> ActivityDate = GetActivityDate(orders, canceledOrders);

            foreach (DateTime date in ActivityDate)
            {
                List<OrderStatisticsDTO> DayStatistics = GetStatisticsOfDay(orders, canceledOrders, date);
                orderStatistics.AddRange(DayStatistics);
            }


            return orderStatistics;
        }

        private List<OrderStatisticsDTO> GetStatisticsOfDay(List<Order> orders, List<CanceledOrder> canceledOrders, DateTime date)
        {
            List<OrderStatisticsDTO> orderStatistics = new List<OrderStatisticsDTO>();
            List<Order> dayOrders = orders.Where(p => p.OrderDate.Date == date).ToList();
            List<CanceledOrder> dayCanceledOrders = canceledOrders.Where(p => p.CanceledDate.Date == date).ToList();

            List<int> SalemanIds = dayOrders.Select(p => p.SalesmanId).ToList();
            SalemanIds.AddRange(dayCanceledOrders.Select(p => p.SalesmanId));

            SalemanIds = SalemanIds.Distinct().ToList();
            foreach (int SalemanId in SalemanIds)
            {
                string SalesmanName = dayOrders.Where(p => p.SalesmanId == SalemanId).Select(p => p.Salesman.FullName).FirstOrDefault();
                OrderStatisticsDTO statistic = new OrderStatisticsDTO
                {
                    Salesman = SalesmanName,
                    Day = date.Day,
                    Purchases = dayOrders.Where(p => p.SalesmanId == SalemanId).Sum(p => p.Total),
                    Returns = dayCanceledOrders.Where(p => p.SalesmanId == SalemanId).Sum(p => p.AmountReturnedToCustomer),
                };
                orderStatistics.Add(statistic);
            }
            return orderStatistics;
        }

        private static List<DateTime> GetActivityDate(List<Order> orders, List<CanceledOrder> canceledOrders)
        {
            List<DateTime> ActivityDate = orders.Select(p => p.OrderDate.Date).ToList();
            ActivityDate.AddRange(canceledOrders.Select(p => p.CanceledDate.Date));
            ActivityDate = ActivityDate.Distinct().ToList();
            return ActivityDate;
        }

    }
}
