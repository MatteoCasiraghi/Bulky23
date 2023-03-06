﻿using BulkyBook.DataAccess.Repository.IRepository;
using BulkyBook.Models;
using BulkyBook.Models.ViewModels;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;

namespace BulkyBookWeb.Controllers
{
    [Area("Admin")]
    public class ProductController : Controller
    {
        private readonly IUnitOfWork _unitOfWork;
        private readonly IWebHostEnvironment _hostEnvironment;
        public ProductController(IUnitOfWork unitOfWork, IWebHostEnvironment hostEnvironment)
        {
            _unitOfWork = unitOfWork;
            _hostEnvironment = hostEnvironment;
        }

        //GET
        public IActionResult Index()
        {
            return View();
        }

        //GET
        public IActionResult Upsert(int? id)
        {
            ProductVM productVM = new()
            {
                Product = new Product(),
                CategoryList = _unitOfWork.Category.GetAll().Select(i => new SelectListItem
                {
                    Text = i.Name,
                    Value = i.Id.ToString()
                }),
                CoverTypeList = _unitOfWork.CoverType.GetAll().Select(i => new SelectListItem
                {
                    Text = i.Name,
                    Value = i.Id.ToString()
                }),
            };

            if (id == null || id == 0)
            {
                //restituisce una view per la creazione di un nuovo prodotto
                return View(productVM);
            }
            else
            {
                var productInDb = _unitOfWork.Product.GetFirstOrDefault(u => u.Id == id);
                if (productInDb != null)
                {
                    productVM.Product = productInDb;
                    //restituisce una view per l'aggiornamento del prodotto
                    //questa view riceve un productVM con tutti i campi di Product
                    return View(productVM);
                }
                //il prodotto con l'id inviato non è stato trovato nel database.
                //restituisce una view per creare un nuovo prodotto
                return View(productVM);

            }
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public IActionResult Upsert(ProductVM obj, IFormFile? file)
        {
            //se si tratta di un nuovo prodotto --> Id ==0 e ImageUrl==null
            //se si tratta di un aggiornamento di un prodotto --> Id!=0 e ImageUrl!=null
            if (ModelState.IsValid)
            {
                string wwwRootPath = _hostEnvironment.WebRootPath;
                if (file != null)
                {
                    //creiamo un nuovo nome per il file che l'utente ha caricato
                    //facciamo in modo che non possano esistere due file con lo stesso nome
                    string fileName = Guid.NewGuid().ToString();
                    var uploadDir = Path.Combine(wwwRootPath, "images", "products");
                    var fileExtension = Path.GetExtension(file.FileName);
                    //nel caso di upload dell'immagine del prodotto, il precedente file, se esiste, deve essere rimosso
                    if (obj.Product.ImageUrl != null)
                    {
                        var oldImagePath = Path.Combine(wwwRootPath, obj.Product.ImageUrl.TrimStart(Path.DirectorySeparatorChar));
                        if (System.IO.File.Exists(oldImagePath))
                        {
                            System.IO.File.Delete(oldImagePath);
                        }
                    }
                    var filePath = Path.Combine(uploadDir, fileName + fileExtension);
                    var fileUrlString = filePath[wwwRootPath.Length..].Replace(@"\\", @"\");
                    using (var fileStream = new FileStream(filePath, FileMode.Create))
                    {
                        file.CopyTo(fileStream);
                    }
                    obj.Product.ImageUrl = fileUrlString;
                }
                if (obj.Product.Id == 0)//new Product
                {
                    _unitOfWork.Product.Add(obj.Product);
                    TempData["success"] = "Product created successfully";
                }
                else //update exsisting Product
                {
                    _unitOfWork.Product.Update(obj.Product);
                    TempData["success"] = "Product updated successfully";
                }
                _unitOfWork.Save();

                return RedirectToAction(nameof(Index));
            }
            return View(obj);
        }


        #region API CALLS
        [HttpGet]
        public IActionResult GetAll()
        {
            var productList = _unitOfWork.Product.GetAll(includeProperties: "Category,CoverType");
            return Json(new { data = productList });
        }

        [HttpDelete]
        public IActionResult Delete(int? id)
        {
            var objFromDbFirst = _unitOfWork.Product.GetFirstOrDefault(u => u.Id == id);
            if (objFromDbFirst == null)//l'oggetto con l'id specificato non è stato trovato
            {
                return Json(new { success = false, message = "Error while deleting" });
            }
            else //l'oggetto con l'id specificato è stato trovato
            {
                if (objFromDbFirst.ImageUrl != null) //l'oggetto ha un ImageUrl!=null
                {
                    var oldImagePath = Path.Combine(_hostEnvironment.WebRootPath, objFromDbFirst.ImageUrl.TrimStart(Path.DirectorySeparatorChar));
                    if (System.IO.File.Exists(oldImagePath))//se il file corrispondente all'ImageUrl esiste va eliminato
                    {
                        System.IO.File.Delete(oldImagePath);
                    }
                }
                _unitOfWork.Product.Remove(objFromDbFirst);
                _unitOfWork.Save();
                return Json(new { success = true, message = "Delete Successful" });
            }
        }

        #endregion
    }

}
