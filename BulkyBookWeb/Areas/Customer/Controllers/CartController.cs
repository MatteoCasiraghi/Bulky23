using BulkyBook.DataAccess.Repository.IRepository;
using BulkyBook.Models.ViewModels;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using System.Security.Claims;

namespace BulkyBookWeb.Areas.Customer.Controllers
{
    [Area("Customer")]
    [Authorize]
    public class CartController : Controller
    {
        private readonly IUnitOfWork _unitOfWork;
        public ShoppingCartVM ShoppingCartVM { get; set; }

        public CartController(IUnitOfWork unitOfWork)
        {
            _unitOfWork = unitOfWork;
            ShoppingCartVM = new ShoppingCartVM();
        }
        public IActionResult Index()
        {
            var userIdentity = User.Identity;
            if (userIdentity != null)
            {
                var claimsIdentity = (ClaimsIdentity)userIdentity;
                var claim = claimsIdentity?.FindFirst(ClaimTypes.NameIdentifier);
                if (claim != null)
                {
                    ShoppingCartVM = new ShoppingCartVM()
                    {
                        ListCart = _unitOfWork.ShoppingCart.GetAll(u => u.ApplicationUserId == claim.Value, includeProperties: "Product")
                    };
                    foreach (var cart in ShoppingCartVM.ListCart)
                    {
                        cart.Price = GetPriceBasedOnQuantity(cart.Count, cart.Product.Price, cart.Product.Price50, cart.Product.Price100);
                        ShoppingCartVM.CartTotal += cart.Price * cart.Count;
                    }
                }
            }
            return View(ShoppingCartVM);
        }
        private static double GetPriceBasedOnQuantity(double quantity, double price, double price50, double price100) => quantity switch
        {
            <= 50 => price,
            <= 100 => price50,
            _ => price100,
        };

    }
}
