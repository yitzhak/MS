using ClothingStore.WebApi.Enum;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Threading.Tasks;

namespace ClothingStore.WebApi.Models
{
    public class User
    {
        public int Id { get; set; }

        [Required]
        public string FirstName { get; set; }

        [Required]
        public string LastName { get; set; }

        internal string FullName
        {
            get
            {
                return $"{FirstName} {LastName}";
            }
        }


        [ScaffoldColumn(false)]
        public string Username { get; set; }

        [Required]
        [StringLength(16, MinimumLength = 7)]
        public string Password { get; set; }

        [Required]
        [ScaffoldColumn(true)]
        internal DateTime CreatedDate { get; set; }

        [Required]
        [DatabaseGenerated(DatabaseGeneratedOption.Identity)]
        [RegularExpression(@"[A-Za-z0-9._%+-]+@[A-Za-z0-9.-]+\.[A-Za-z]{2,4}",
    ErrorMessage = "Email is not valid.")]
        public string Email { get; set; }

        public AccessLevel AccessLevel { get; set; }
    }
}
