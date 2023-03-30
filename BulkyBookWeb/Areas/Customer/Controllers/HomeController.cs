using BulkyBook.DataAccess.Repository.IRepository;
using BulkyBook.Models;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using System.Diagnostics;
using System.Security.Claims;

namespace BulkyBookWeb.Controllers;
[Area("Customer")]
public class HomeController : Controller
{
    private readonly ILogger<HomeController> _logger;
    private readonly IUnitOfWork _unitOfWork;
    public HomeController(ILogger<HomeController> logger, IUnitOfWork unitOfWork)
    {
        _logger = logger;
        _unitOfWork = unitOfWork;
    }

    public IActionResult Index()
    {
        //IEnumerable<Product> productList = _unitOfWork.Product.GetAll(includeProperties: "Category,CoverType");
        //return View(productList);
        return View();
    }

    public IActionResult Details(int productId)
    {
        var selectedProductInDb = _unitOfWork.Product.GetFirstOrDefault(product => product.Id == productId, "Category,CoverType");
        if (selectedProductInDb != null)
        {
            ShoppingCart cartObj = new()
            {
                Count = 1,
                ProductId = productId,
                Product = selectedProductInDb

            };
            return View(cartObj);
        }
        return RedirectToAction(nameof(Index));
    }

    [HttpPost]
    [ValidateAntiForgeryToken]
    [Authorize]
    public IActionResult Details(ShoppingCart shoppingCart)
    {
        var userIdentity = User.Identity;
        if (userIdentity != null)
        {
            var claimsIdentity = (ClaimsIdentity)userIdentity;
            var claim = claimsIdentity?.FindFirst(ClaimTypes.NameIdentifier);
            if (claim != null)
            {
                //aggiorno il riferimento all'utente
                shoppingCart.ApplicationUserId = claim.Value;
                //controllo che l'Id passato nel form corrisponda effettivamente a un prodotto nel database
                Product? selectedProductInDb = _unitOfWork.Product.GetFirstOrDefault(product => product.Id == shoppingCart.ProductId, "Category,CoverType");
                if (selectedProductInDb != null)
                {
                    //verifico se c'è già un prodotto con lo stesso id nella shopping cart (nel database)
                    ShoppingCart? cartFromDb = _unitOfWork.ShoppingCart.GetFirstOrDefault(u => u.ApplicationUserId == claim.Value && u.ProductId == shoppingCart.ProductId);

                    if (cartFromDb == null)//il prodotto non è presente nella shopping cart --> Add
                    {
                        //salvo shoppingCart: ha valori per ProductId, ApplicationUserId e Count. L'Id verrà definito dal database
                        _unitOfWork.ShoppingCart.Add(shoppingCart);
                    }
                    else //il prodotto è già presente nella shopping cart --> Update tramite aggiornamento dell'oggetto tracciato da EF Core e salvataggio
                    {
                        _unitOfWork.ShoppingCart.IncrementCount(cartFromDb, shoppingCart.Count);
                    }
                    _unitOfWork.Save();
                    RedirectToAction(nameof(Index));
                }
            }
        }
        return RedirectToAction(nameof(Index));
    }

    public IActionResult Privacy()
    {
        return View();
    }

    [ResponseCache(Duration = 0, Location = ResponseCacheLocation.None, NoStore = true)]
    public IActionResult Error()
    {
        return View(new ErrorViewModel { RequestId = Activity.Current?.Id ?? HttpContext.TraceIdentifier });
    }
}
