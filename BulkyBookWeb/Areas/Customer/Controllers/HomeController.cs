﻿using BulkyBook.DataAccess.Repository.IRepository;
using BulkyBook.Models;
using Microsoft.AspNetCore.Mvc;
using System.Diagnostics;

namespace BulkyBookWeb.Controllers;
[Area("Customer")]
public class HomeController : Controller
{
    private readonly ILogger<HomeController> _logger;
    private readonly IUnitOfWork _unitOfWork;

    //private readonly IUnitOfWork _unitOfWork;
    public HomeController(ILogger<HomeController> logger, IUnitOfWork unitOfWork)
    {
        _logger = logger;
        _unitOfWork = unitOfWork;
    }


    //public HomeController(ILogger<HomeController> logger, IUnitOfWork unitOfWork)
    //{
    //    _logger = logger;
    //    _unitOfWork = unitOfWork;
    //}

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
                Product = selectedProductInDb

            };
            return View(cartObj);
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
