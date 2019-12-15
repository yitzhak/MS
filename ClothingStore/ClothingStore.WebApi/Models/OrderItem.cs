using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Threading.Tasks;

namespace ClothingStore.WebApi.Models
{
    public class OrderItem
    {
        public int Id { get; set; }

        [Required]
        public int OrderId { get; set; }

        [Required]
        public int ProductId { get; set; }

        [Required]
        public int Quantity { get; set; }

        [Required]
        [ScaffoldColumn(true)]
        public decimal UnitPrice { get; internal set; }

        public bool CanBeReturned { get; set; }

        public virtual Product Product { get; set; }
    }
}
