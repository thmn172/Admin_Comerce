using Microsoft.AspNetCore.Mvc;
using SV20T1020460.BusinessLayers;
using SV20T1020460.DomainModels;
using static System.Runtime.InteropServices.JavaScript.JSType;
using System.Buffers;
using System.Drawing.Printing;
using Microsoft.AspNetCore.Mvc.RazorPages;
using SV20T1020460.Web.Models;
using Microsoft.AspNetCore.Authorization;

namespace SV20T1020460.Web.Controllers
{
    [Authorize(Roles =$"{WebUserRoles.Administrator},{WebUserRoles.Employee}")]
    public class CategoryController : Controller
    {
        private const int PAGE_SIZE = 20;
        private const string CATEGORY_SEARCH = "category_search"; 
        public IActionResult Index()
        {
            PaginationSearchInput? input = ApplicationContext.GetSessionData<PaginationSearchInput>(CATEGORY_SEARCH);
            if(input == null)
            {
                input = new PaginationSearchInput()
                {
                    Page = 1,
                    PageSize = PAGE_SIZE,
                    SearchValue = ""
                };
            }
            return View(input);
            
        }
        public IActionResult Search(PaginationSearchInput input)
        {
            int rowCount = 0;
            var data = CommomDataService.ListOfCategories(out rowCount, input.Page, input.PageSize, input.SearchValue ?? "");
            var model = new CategorySearchResult()
            {
                Page = input.Page,
                PageSize = input.PageSize,
                SearchValue = input.SearchValue??"",
                RowCount = rowCount,
                Data = data
            };
            ApplicationContext.SetSessionData(CATEGORY_SEARCH, input);
            return View(model);
        }
        public IActionResult Create()
        {
            ViewBag.Title = "Bổ sung loại hàng";
            Category model = new Category()
            {
                CategoryID = 0,
                Photo = "noneProduct.png"
            };
            return View("Edit", model);
        }
        public IActionResult Edit(int id = 0)
        {
            ViewBag.Title = "Cập nhật thông tin khách hàng";
            Category? model = CommomDataService.GetCategory(id);
            if (model == null)
                return RedirectToAction("Index");
            if (string.IsNullOrEmpty(model.Photo))
                model.Photo = "noneProduct.png";

            return View(model);
        }
        [HttpPost]
        public IActionResult Save(Category data, IFormFile? uploadPhoto) 
        {
            //try
            //{
                if (uploadPhoto != null)
                {
                    string fileName = $"{DateTime.Now.Ticks}_{uploadPhoto.FileName}";//tên file sẽ lưu
                    string folder = Path.Combine(ApplicationContext.HostEnviroment.WebRootPath, @"images\category");//đường dẫn đến thư mục lưu file
                    string filepath = Path.Combine(folder, fileName);//đường dẫn đến file cần lưu
                    using (var stream = new FileStream(filepath, FileMode.Create))
                    {
                        uploadPhoto.CopyTo(stream);
                    }
                    data.Photo = fileName;
                }
                if (string.IsNullOrWhiteSpace(data.CategoryName))
                    ModelState.AddModelError(nameof(data.CategoryName), "Tên không được để trống");
                if (data.CategoryID == 0)
                {
                    int id = CommomDataService.AddCategory(data);
                    return RedirectToAction("Index");
                }
                else
                {
                    bool result = CommomDataService.UpdateCategory(data);
                }
                return RedirectToAction("Index");
            //}
            //catch (Exception ex)
            //{
            //    ModelState.AddModelError("Error", "không lưu được dữ liệu. Vui lòng thử lại sau");
            //    return View("Edit", data);
            //}
        }
        public IActionResult Delete(int id = 0)
        {
            if(Request.Method == "POST")
            {
                CommomDataService.DeleteCategory(id);
                return RedirectToAction("Index");
            }
            var model = CommomDataService.GetCategory(id);
            if (model == null)
                return RedirectToAction("Index");
                ViewBag.AllowDelete = !CommomDataService.IsUsedCategory(id);
            return View(model);
        }
    }
}
