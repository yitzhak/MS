using ClothingStore.WebApi.Enum;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Threading.Tasks;

namespace ClothingStore.WebApi.Models
{
    public class CanceledOrder
    {
        public int Id { get; set; }

        [Required]
        public DateTime CanceledDate { get; set; }

        [Required]
        public PaymentType PaymentType { get; set; }

        [Required]
        public int OrderId { get; set; }

        [Required]
        public int SalesmanId { get; set; }

        public Order Order { get; set; }

        public User Salesman { get; set; }

        [Required]
        public List<ReturnedItem> ReturnedItems { get; set; }

        public decimal AmountReturnedToCustomer
        {
            get
            {
                decimal amount = ReturnedItems?.Sum(p => p.Quantity * p.OrderItem.UnitPrice) ??0;
                return amount;
            }
        }

    }
}
