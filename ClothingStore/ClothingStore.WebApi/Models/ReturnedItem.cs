using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Threading.Tasks;

namespace ClothingStore.WebApi.Models
{
    public class ReturnedItem
    {
        public ReturnedItem()
        {
            OrderItem = new OrderItem();
        }

        public int Id { get; set; }

        [Required]
        public int OrderItemId { get; set; }

        [Required]
        public int Quantity { get; set; }

        public virtual OrderItem OrderItem { get; set; }
    }
}
