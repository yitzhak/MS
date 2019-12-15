using ClothingStore.WebApi.Enum;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Threading.Tasks;

namespace ClothingStore.WebApi.Models
{
    public class OrderPayment
    {
        public int Id { get; set; }

        [Required]
        public PaymentType PaymentType { get; set; }
        public string PaymentIdentity { get; set; }

        [Required]
        public decimal Amount { get; set; }

        [Required]
        public int OrderId { get; set; }
        public Order Order { get; set; }
    }
}
