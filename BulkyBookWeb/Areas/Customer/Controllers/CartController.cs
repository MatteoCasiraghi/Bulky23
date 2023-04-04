using BulkyBook.DataAccess.Repository.IRepository;
using BulkyBook.Models;
using BulkyBook.Models.ViewModels;
using BulkyBook.Utility;
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
		[BindProperty]
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
						ListCart = _unitOfWork.ShoppingCart.GetAll(u => u.ApplicationUserId == claim.Value, includeProperties: "Product"),
						OrderHeader = new()
					};
					foreach (var cart in ShoppingCartVM.ListCart)
					{
						cart.Price = GetPriceBasedOnQuantity(cart.Count, cart.Product.Price, cart.Product.Price50, cart.Product.Price100);
						ShoppingCartVM.OrderHeader.OrderTotal += cart.Price * cart.Count;
					}
				}
			}
			return View(ShoppingCartVM);
		}

		public IActionResult Summary()
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
						//recupero i dati della ShoppingCart dal database
						ListCart = _unitOfWork.ShoppingCart.GetAll(u => u.ApplicationUserId == claim.Value, includeProperties: "Product"),
						OrderHeader = new()
					};
					//recupero i dati dell'utente a partire dal suo Id --> claim.value corrisponde all'Id dell'utente in AspNetUsers
					ShoppingCartVM.OrderHeader.ApplicationUser = _unitOfWork.ApplicationUser.GetFirstOrDefault(u => u.Id == claim.Value)!;
					ShoppingCartVM.OrderHeader.Name = ShoppingCartVM.OrderHeader.ApplicationUser.Name;
					ShoppingCartVM.OrderHeader.PhoneNumber = ShoppingCartVM.OrderHeader.ApplicationUser.PhoneNumber ?? string.Empty;
					ShoppingCartVM.OrderHeader.StreetAddress = ShoppingCartVM.OrderHeader.ApplicationUser.StreetAddress ?? string.Empty;
					ShoppingCartVM.OrderHeader.City = ShoppingCartVM.OrderHeader.ApplicationUser.City ?? string.Empty;
					ShoppingCartVM.OrderHeader.State = ShoppingCartVM.OrderHeader.ApplicationUser.State ?? string.Empty;
					ShoppingCartVM.OrderHeader.PostalCode = ShoppingCartVM.OrderHeader.ApplicationUser.PostalCode ?? string.Empty;
					//calcolo il totale da mostrare nel summary
					foreach (var cart in ShoppingCartVM.ListCart)
					{
						cart.Price = GetPriceBasedOnQuantity(cart.Count, cart.Product.Price, cart.Product.Price50, cart.Product.Price100);
						ShoppingCartVM.OrderHeader.OrderTotal += cart.Price * cart.Count;
					}
				}
			}
			return View(ShoppingCartVM);
		}

		[HttpPost]
		[ActionName("Summary")]
		[ValidateAntiForgeryToken]
		public IActionResult SummaryPOST()
		{
			var userIdentity = User.Identity;
			if (userIdentity != null)
			{
				var claimsIdentity = (ClaimsIdentity)userIdentity;
				var claim = claimsIdentity?.FindFirst(ClaimTypes.NameIdentifier);
				if (claim != null)
				{
					//definisco il contenuto dell'ordine
					//recupero dal database i prodotti nella ShoppingCart
					ShoppingCartVM.ListCart = _unitOfWork.ShoppingCart.GetAll(u => u.ApplicationUserId == claim.Value, includeProperties: "Product");
					//definisco i dati di OrderHeader
					ShoppingCartVM.OrderHeader.PaymentStatus = SD.PaymentStatusPending;
					ShoppingCartVM.OrderHeader.OrderStatus = SD.StatusPending;
					ShoppingCartVM.OrderHeader.OrderDate = DateTime.Now;
					ShoppingCartVM.OrderHeader.ApplicationUserId = claim.Value;
					//calcolo il totale dell'ordine e lo salvo in OrderHeader.OrderTotal
					foreach (var cart in ShoppingCartVM.ListCart)
					{
						cart.Price = GetPriceBasedOnQuantity(cart.Count, cart.Product.Price, cart.Product.Price50, cart.Product.Price100);
						ShoppingCartVM.OrderHeader.OrderTotal += cart.Price * cart.Count;
					}
					//salvo OrderHeader nel database -
					//da questo momento in avanti ho l'Id di OrderHeader nel database che serve come FK in OrderDetail
					_unitOfWork.OrderHeader.Add(ShoppingCartVM.OrderHeader);
					_unitOfWork.Save();

					//definisco i dati di OrderDetail
					//ogni articolo nell'ordine, con relativa quantità e prezzo, corrisponde ad una riga nella tabella di OrderDetail
					//ciascuna riga ha una FK su OrderId di OrderHeader
					foreach (var cart in ShoppingCartVM.ListCart)
					{
						//definisco la riga in OrderDetail
						OrderDetail orderDetail = new()
						{
							ProductId = cart.ProductId,
							OrderId = ShoppingCartVM.OrderHeader.Id,
							Price = cart.Price,
							Count = cart.Count
						};
						//salvo la riga nel database
						_unitOfWork.OrderDetail.Add(orderDetail);
						_unitOfWork.Save();
					}
					//rimuovo gli articoli messi nell'ordine dalla ShoppingCart dell'utente
					_unitOfWork.ShoppingCart.RemoveRange(ShoppingCartVM.ListCart);
					_unitOfWork.Save();
				}
			}
			return RedirectToAction("Index", "Home");
		}

		public IActionResult Plus(int cartId)
		{
			var cart = _unitOfWork.ShoppingCart.GetFirstOrDefault(u => u.Id == cartId);
			if (cart != null)
			{
				_unitOfWork.ShoppingCart.IncrementCount(cart, 1);
				_unitOfWork.Save();
			}
			return RedirectToAction(nameof(Index));
		}

		public IActionResult Minus(int cartId)
		{
			var cart = _unitOfWork.ShoppingCart.GetFirstOrDefault(u => u.Id == cartId);
			if (cart != null)
			{
				if (cart.Count <= 1)
				{
					_unitOfWork.ShoppingCart.Remove(cart);
				}
				else
				{
					_unitOfWork.ShoppingCart.DecrementCount(cart, 1);
				}
				_unitOfWork.Save();
			}
			return RedirectToAction(nameof(Index));
		}

		public IActionResult Remove(int cartId)
		{
			var cart = _unitOfWork.ShoppingCart.GetFirstOrDefault(u => u.Id == cartId);
			if (cart != null)
			{
				_unitOfWork.ShoppingCart.Remove(cart);
				_unitOfWork.Save();
			}
			return RedirectToAction(nameof(Index));
		}
		private static double GetPriceBasedOnQuantity(double quantity, double price, double price50, double price100) => quantity switch
		{
			<= 50 => price,
			<= 100 => price50,
			_ => price100,
		};

	}
}
