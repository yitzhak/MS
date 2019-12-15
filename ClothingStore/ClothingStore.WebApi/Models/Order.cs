using ClothingStore.WebApi.Helpers;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Threading.Tasks;

namespace ClothingStore.WebApi.Models
{
    public class Order
    {
        public int Id { get; set; }

        public DateTime OrderDate { get; set; }

        public string Username { get
            {
                return $"{FirstName} {LastName}";
            }
        }

        [Required]
        [StringLength(160)]
        public string FirstName { get; set; }

        [Required]
        [StringLength(160)]
        public string LastName { get; set; }

        [Required]
        [StringLength(70, MinimumLength = 3)]
        public string Address { get; set; }

        [Required]
        [StringLength(40)]
        public string City { get; set; }

        [Required]
        [StringLength(40)]
        public string State { get; set; }

        [Required]
        [StringLength(10, MinimumLength = 5)]
        public string PostalCode { get; set; }

        [Required]
        [StringLength(40)]
        public string Country { get; set; }

        [Required]
        [StringLength(24)]
        public string Phone { get; set; }

        [Required]
        [RegularExpression(@"[A-Za-z0-9._%+-]+@[A-Za-z0-9.-]+\.[A-Za-z]{2,4}",
            ErrorMessage = "Email is not valid.")]
        public string Email { get; set; }

        [NotMapped]
        internal decimal Total
        {
            get
            {
                if (OrderItems == null || !OrderItems.Any())
                {
                    throw new BadRequestException(Enum.InternalCode.ValidationException, "Order must contain Items");
                }
                decimal total = OrderItems.Sum(p => p.UnitPrice * p.Quantity);
                return total;
            }
        }

        [Required]
        public int SalesmanId { get; set; }

        public User Salesman { get; set; }

        [ScaffoldColumn(true)]
        public virtual List<OrderPayment> OrderPayments { get; set; }
        public List<OrderItem> OrderItems { get; set; }
        public virtual List<CanceledOrder> Returns { get; set; }
    }
}
