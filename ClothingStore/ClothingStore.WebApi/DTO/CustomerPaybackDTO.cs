using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using ClothingStore.WebApi.Enum;

namespace ClothingStore.WebApi.DTO
{
    public class CustomerPaybackDTO
    {
        public decimal Amount { get; set; }
        public PaymentType PaymentType { get; set; }
        public Guid Identity { get; set; }
    }
}
