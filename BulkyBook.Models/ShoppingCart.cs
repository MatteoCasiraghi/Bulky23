using Microsoft.AspNetCore.Mvc.ModelBinding.Validation;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace BulkyBook.Models
{
    public class ShoppingCart
    {
        public int Id { get; set; }
        public int ProductId { get; set; }
        [ForeignKey(nameof(ProductId))]
        [ValidateNever]
        public Product Product { get; set; } = null!;
        [Range(1, 1000, ErrorMessage = "Please enter a value between {0} and {1}")]
        public int Count { get; set; }
        public string ApplicationUserId { get; set; } = null!;
        [ForeignKey(nameof(ApplicationUserId))]
        [ValidateNever]
        public ApplicationUser ApplicationUser { get; set; } = null!;
        [NotMapped]
        public double Price { get; set; }

    }
}
