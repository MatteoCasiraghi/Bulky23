using Microsoft.AspNetCore.Mvc.Rendering;

namespace BulkyBook.Models.ViewModels
{
    public class ProductVM
    {
        public Product Product { get; set; } = null!;
        public IEnumerable<SelectListItem> CategoryList { get; set; } = null!;
        public IEnumerable<SelectListItem> CoverTypeList { get; set; } = null!;

    }
}
