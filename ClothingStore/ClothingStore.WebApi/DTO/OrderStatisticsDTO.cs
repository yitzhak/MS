using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace ClothingStore.WebApi.DTO
{
    public class OrderStatisticsDTO
    {
        public int Day { get; internal set; }
        public string Salesman { get; internal set; }
        public decimal Purchases { get; internal set; }
        public decimal Returns { get; internal set; }
    }
}
