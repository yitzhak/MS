using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Threading.Tasks;

namespace ClothingStore.WebApi.Models
{
    public class Product
    {
        public Product()
        {
            OrderItems = new List<OrderItem>();
            Created = DateTime.UtcNow;
            Updated = DateTime.UtcNow;
        }

        public int Id { get; set; }

        [Required]
        [StringLength(160, MinimumLength = 2)]
        public string Title { get; set; }

        [Required]
        [Range(0.01,double.MaxValue)]
        public decimal Price { get; set; }

        [StringLength(1024)]
        public string ProductUrl { get; set; }

        public bool CanBeReturned { get; set; }

        [Required]
        public int PiecesAvailable { get; set; }

        [Required]
        public DateTime Created { get; set; }

        [Required]
        public DateTime Updated { get; set; }

        public virtual List<OrderItem> OrderItems { get; set; }

        public virtual List<CanceledOrder> CanceledOrders { get; set; }

    }
}
