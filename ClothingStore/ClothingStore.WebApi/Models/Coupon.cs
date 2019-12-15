using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Threading.Tasks;

namespace ClothingStore.WebApi.Models
{
    public class Coupon
    {
        public Coupon()
        {
        }

        public Coupon(decimal amount, int orderId)
        {
            Amount = amount;
            OrderId = orderId;
            CreatedDate = DateTime.UtcNow;
            IsUsed = false;
            CouponIdentity = Guid.NewGuid();
        }

        public int Id { get; set; }

        [Required]
        public Guid CouponIdentity { get; internal set; }

        [Required]
        public DateTime CreatedDate { get; set; }

        [Required]
        public decimal Amount { get; set; }
        public bool IsUsed { get; set; }
        public DateTime UsedDate { get; set; }

        public int? OrderId { get; set; }
        public virtual Order Order { get; set; }
    }
}
