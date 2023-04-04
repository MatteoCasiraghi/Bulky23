using Microsoft.AspNetCore.Mvc.ModelBinding.Validation;
using System.ComponentModel;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace BulkyBook.Models
{
	public class OrderHeader
	{
		public int Id { get; set; }
		public string ApplicationUserId { get; set; } = null!;
		[ForeignKey(nameof(ApplicationUserId))]
		[ValidateNever]
		public ApplicationUser ApplicationUser { get; set; } = null!;
		public DateTime OrderDate { get; set; }
		public DateTime? ShippingDate { get; set; }
		public double OrderTotal { get; set; }
		public string? OrderStatus { get; set; }
		public string? PaymentStatus { get; set; }
		public string? TrackingNumber { get; set; }
		public string? Carrier { get; set; }
		public DateTime? PaymentDate { get; set; }
		public DateTime? PaymentDueDate { get; set; }
		public string? SessionId { get; set; }
		public string? PaymentIntentId { get; set; }
		[Required][DisplayName("Phone Number")][MinLength(5, ErrorMessage = "{0} must be at least {1} digits")] public string PhoneNumber { get; set; } = null!;
		[Required][DisplayName("Street Address")][MinLength(2, ErrorMessage = "{0} must be at least {1} chars")] public string StreetAddress { get; set; } = null!;
		[Required][MinLength(2, ErrorMessage = "{0} must be at least {1} chars")] public string City { get; set; } = null!;
		[Required][MinLength(2, ErrorMessage = "{0} must be at least {1} chars")] public string State { get; set; } = null!;
		[Required][DisplayName("Postal Code")][MinLength(2, ErrorMessage = "{0} must be at least {1} chars")] public string PostalCode { get; set; } = null!;
		[Required] public string Name { get; set; } = null!;
	}
}
