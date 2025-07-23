namespace Ecommerce.Web.Areas.Admin.Controllers
{
    [Area("Admin")]
    public class ProductController : Controller
    {
        private readonly IUnitOfWork _unitOfWork;
        private readonly IWebHostEnvironment _webHostEnvironment;

        public ProductController(IUnitOfWork unitOfWork, IWebHostEnvironment webHostEnvironment)
        {
            _unitOfWork = unitOfWork;
            _webHostEnvironment = webHostEnvironment;
        }

        public IActionResult Index()
        {
            var ProductsList = _unitOfWork.Products.GetAll(null, nameof(Category));
            return View(ProductsList);
        }
        public IActionResult Details(int id)
        {
            var product = _unitOfWork.Products.GetById(id);
            if (product == null)
                return View("NotFound");
            return View(product);
        }
        [HttpGet]
        public IActionResult Add()
        {
            var addProductVM = new AddProductViewModel
            {
                CategoryList = _unitOfWork.Categories.GetAll().Select(x => new SelectListItem
                {
                    Value = x.Id.ToString(),
                    Text = x.CategoryName,
                })
            };

            return View(addProductVM);
        }
        [HttpPost]
        public IActionResult Add(AddProductViewModel addProductViewModel)
        {
            if (ModelState.IsValid)
            {
               
                Product product = new Product
                {
                    Name = addProductViewModel.Name,
                    Author = addProductViewModel.Author,
                    Description = addProductViewModel.Description,
                    ISBN = addProductViewModel.ISBN,
                    ListPrice = addProductViewModel.ListPrice,
                    Price100 = addProductViewModel.Price100,
                    Price50 = addProductViewModel.Price50,
                    ImageUrl = ConvertImage(addProductViewModel.ImageFile!),
                    CategoryId = addProductViewModel.CategoryId,
                };

                _unitOfWork.Products.Add(product);
                _unitOfWork.Save();
                TempData["success"] = "Added Category Successfully";

                return RedirectToAction("Index");
            }
            addProductViewModel.CategoryList = _unitOfWork.Categories.GetAll().Select(x => new SelectListItem
            {
                Value = x.Id.ToString(),
                Text = x.CategoryName,
            });
            TempData["error"] = "Error In Adding new Product";
            return View(addProductViewModel);
        }

        [HttpGet]
        public IActionResult Update(int id)
        {
            var product = _unitOfWork.Products.GetById(id);
            if (product == null)
                return View("NotFound");
            var updateProductViewModel = new UpdateProductViewModel
            {
                Id = product.Id,
                Name = product.Name,
                ISBN = product.ISBN,
                Author = product.Author,
                ListPrice = product.ListPrice,
                Description = product.Description,
                Price50 = product.Price50,
                Price100 = product.Price100,
                ImageUrl = product.ImageUrl,
                CategoryId = product.CategoryId,
                CategoryList = _unitOfWork.Categories.GetAll().Select(x => new SelectListItem
                {
                    Value = x.Id.ToString(),
                    Text = x.CategoryName,
                }),

            };
            return View(updateProductViewModel);
        }
        [HttpPost]
        public IActionResult Update(UpdateProductViewModel updateProductViewModel)
        {
            if (ModelState.IsValid)
            {
                string? imageUrl = _unitOfWork.Products.GetById(updateProductViewModel.Id)?.ImageUrl;
                if (updateProductViewModel.ImageFile != null)
                {
                    RemoveImageFromServer(imageUrl ?? "");
                    imageUrl = ConvertImage(updateProductViewModel.ImageFile);
                }
                var product = new Product
                {
                    Id = updateProductViewModel.Id,
                    CategoryId = updateProductViewModel.CategoryId,
                    Name = updateProductViewModel.Name,
                    Author = updateProductViewModel.Author,
                    ListPrice = updateProductViewModel.ListPrice,
                    Description = updateProductViewModel.Description,
                    Price50 = updateProductViewModel.Price50,
                    Price100 = updateProductViewModel.Price100,
                    ISBN = updateProductViewModel.ISBN,
                    ImageUrl = imageUrl,
                }
            ;

                _unitOfWork.Products.Update(product);
                _unitOfWork.Save();
                TempData["success"] = "Updated Product Successfully";
                return RedirectToAction("Index");
            }
            TempData["error"] = "Error in Update Products";
            updateProductViewModel.CategoryList = _unitOfWork.Categories.GetAll().Select(x => new SelectListItem { Text = x.CategoryName, Value = x.Id.ToString() });
            return View(updateProductViewModel);

        }

        public IActionResult Clear()
        {
            _unitOfWork.Products.Clear();
            RemoveImageFromServer("", true);
            return RedirectToAction("Index");
        }

        #region API Call

        [HttpGet]
        public IActionResult GetAll()
        {
            var products = _unitOfWork.Products
                 .GetAll(null,nameof(Category))
                 .Select(x => new
                 {
                     Id = x.Id,
                     Name = x.Name,
                     Author = x.Author,
                     ISBN = x.ISBN,
                     ListPrice = x.ListPrice,
                     CategoryName = x.Category.CategoryName
                 })
                 .ToList();
            return Json(new { data = products });

        }

        [HttpDelete]
        public IActionResult Delete(int id)
        {
            var product = _unitOfWork.Products.GetById(id);
            if (product == null)
                return Json(new { success = false, message = "Error While Deleting" });

            RemoveImageFromServer(product.ImageUrl ?? "");
            _unitOfWork.Products.Remove(product);
            _unitOfWork.Save();
            return Json(new { success = true, message = "Deleting Successfully" });
        }
        #endregion

        #region private Function
        public string ConvertImage(IFormFile file)
        {
            var fileName = Guid.NewGuid().ToString() + Path.GetExtension(file.FileName);
            var folderPath = Path.Combine(_webHostEnvironment.WebRootPath, "images/products");

            if (!Directory.Exists(folderPath))
            {
                Directory.CreateDirectory(folderPath);
            }

            var fullPath = Path.Combine(folderPath, fileName);

            using (var fileStream = new FileStream(fullPath, FileMode.Create))
            {
                file.CopyTo(fileStream);
            }
            return fileName;

        }

        private void RemoveImageFromServer(string imageName, bool clearAll = false)
        {
            string folderPath = Path.Combine(_webHostEnvironment.WebRootPath, "images/products");
            if (clearAll && Directory.Exists(folderPath))
            {
                foreach (var file in Directory.GetFiles(folderPath))
                {
                    System.IO.File.Delete(file);
                }
            }

            if (!string.IsNullOrEmpty(imageName))
            {
                string imagePath = Path.Combine(folderPath, imageName);
                if (System.IO.File.Exists(imagePath))
                {
                    System.IO.File.Delete(imagePath);
                }
            }


        }

        #endregion
    }
}
