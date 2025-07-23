namespace Ecommerce.Web.Areas.Admin.Controllers
{
    [Area("Admin")]
    public class CategoryController : Controller
    {
        private readonly IUnitOfWork _unitOfWork;

        public CategoryController(IUnitOfWork unitOfWork)
        {
            _unitOfWork = unitOfWork;
        }
        public IActionResult Index()
        {
            return View(nameof(Index));
        }
        public IActionResult Details(int id)
        {
            var category = _unitOfWork.Categories.GetById(id);
            if (category == null)
                return View("NotFound");
            return View(category);
        }
        [HttpGet]
        public IActionResult Add()
        {
            return View();
        }
        [HttpPost]
        public IActionResult Add(Category category)
        {
            if (ModelState.IsValid)
            {
                _unitOfWork.Categories.Add(category);
                _unitOfWork.Save();
                TempData["success"] = "Added Category Successfully";
                return RedirectToAction("Index");
            }

            TempData["error"] = "Error In Adding new Category";
            return View(category);
        }

        [HttpGet]
        public IActionResult Update(int id)
        {
            var category = _unitOfWork.Categories.GetById(id);
            if (category == null)
                return View("NotFound");
            return View(category);
        }
        [HttpPost]
        public IActionResult Update(Category category)
        {
            if (ModelState.IsValid)
            {
                _unitOfWork.Categories.Update(category);
                _unitOfWork.Save();
                TempData["success"] = "Updated Category Successfully";
                return RedirectToAction("Index");
            }
            TempData["error"] = "Error In Update Category";
            return View(category);

        }

        public IActionResult Clear()
        {
            _unitOfWork.Categories.Clear();
            return RedirectToAction("Index");
        }


        #region API Call

        [HttpGet]
        public IActionResult GetAll()
        {
            var categoryList = _unitOfWork.Categories.GetAll().Select(x => new
            {
                x.Id,
                x.CategoryName,
            });
            return Json(new { data = categoryList });
        }


        [HttpDelete]
        public IActionResult Delete(int id)
        {
            var category = _unitOfWork.Categories.GetById(id);
            if (category == null)
                return Json(new { success = false, message = "Category Not Found" });

            _unitOfWork.Categories.Remove(category);
            _unitOfWork.Save();
            TempData["success"] = "Deleted Category Successfully";
            return Json(new { success = true, message = "Category Deleted Successfully" });
        }
        #endregion
    }
}
