using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using SV20T1020460.BusinessLayers;
using SV20T1020460.DomainModels;
using SV20T1020460.Web.Models;
using System.Data;
using System.Text.RegularExpressions;
using static System.Runtime.InteropServices.JavaScript.JSType;

namespace SV20T1020460.Web.Controllers
{
    [Authorize(Roles = $"{WebUserRoles.Administrator},{WebUserRoles.Employee}")]
    public class ProductController : Controller
    {
        private const int PAGE_SIZE = 20;
        private const string PRODUCT_SEARCH = "product_search";
        public IActionResult Index()
        {
            ProductSearchInput? input = ApplicationContext.GetSessionData<ProductSearchInput>(PRODUCT_SEARCH);
            if (input == null)
            {
                input = new ProductSearchInput()
                {
                    Page = 1,
                    PageSize = PAGE_SIZE,
                    SearchValue = "",
                    CategoryID = 0,
                    SupplierID = 0,
                    minPrice = 0,
                    maxPrice = 0

                };
            }
            return View(input);
        }

        public IActionResult Search(ProductSearchInput input)
        {

            int rowCount = 0;
            var data = ProductDataService.ListProducts(out rowCount, input.Page, input.PageSize, input.SearchValue ?? "",
                                                        input.CategoryID, input.SupplierID, input.minPrice, input.maxPrice);
            var model = new ProductSearchResult()
            {
                Page = input.Page,
                PageSize = input.PageSize,
                SearchValue = input.SearchValue ?? "",
                RowCount = rowCount,
                categoryID = input.CategoryID,
                supplierID = input.SupplierID,
                minPrice = input.minPrice,
                maxPrice = input.maxPrice,
                Data = data
            };
            ApplicationContext.SetSessionData(PRODUCT_SEARCH, input);
            return View(model);
        }
        public IActionResult Create()
        {
            ViewBag.Title = "Bổ sung sản phẩm";

            ViewBag.IsEdit = false;
            Product data = new Product()
            {
                ProductID = 0,
                Photo = "noneProduct.png"
            };
            return View("Edit", data);
        }
        public IActionResult Edit(int id = 0)
        {
            ViewBag.Title = "Cập nhật thông tin sản phẩm";
            ViewBag.IsEdit = true;//viết tạm
            Product? data = ProductDataService.GetProduct(id);
            if (data == null)
                return RedirectToAction("Index");
            if (string.IsNullOrEmpty(data.Photo))
                data.Photo = "noneProduct.png";
            return View(data);
        }
        [HttpPost]
        public IActionResult Save(Product data, IFormFile? uploadPhoto)
        {
            try
            {
                if (string.IsNullOrWhiteSpace(data.ProductName))
                    ModelState.AddModelError(nameof(data.ProductName), "Tên mặt hàng không được bỏ trống");
                if (data.CategoryID == 0)
                    ModelState.AddModelError(nameof(data.CategoryID), "Vui lòng chọn loại hàng");
                if (data.SupplierID == 0)
                    ModelState.AddModelError(nameof(data.SupplierID), "Vui lòng chọn nhà cung cấp");
                if (string.IsNullOrWhiteSpace(data.Unit))
                    ModelState.AddModelError(nameof(data.Unit), "Đơn vị tính không được bỏ trống");
                if ((data.Price <= 0))
                    ModelState.AddModelError(nameof(data.Price), "Giá hàng không hợp lệ");

                if (!ModelState.IsValid)
                {
                    ViewBag.Tittle = data.ProductID == 0 ? "Bổ sung khách hàng" : "Cập nhật thông tin khách hàng";
                    return View("Edit", data);
                }
                if (uploadPhoto != null)
                {
                    string filename = $"{DateTime.Now.Ticks}_{uploadPhoto.FileName}";
                    string folder = Path.Combine(ApplicationContext.HostEnviroment.WebRootPath, @"images\products");
                    string filepath = Path.Combine(folder, filename);
                    using (var stream = new FileStream(filepath, FileMode.Create))
                    {
                        uploadPhoto.CopyTo(stream);
                    }
                    data.Photo = filename;
                }
                if (data.ProductID == 0)
                {

                    int id = ProductDataService.AddProduct(data);
                    if (id <= 0)
                    {
                        ModelState.AddModelError(nameof(data.ProductName), "Tên mặt hàng bị trùng");
                        return View("Edit", data);
                    }
                    //return Json(data);
                }
                else
                {

                    bool result = ProductDataService.UpdateProduct(data);
                    if (!result)
                    {
                        ModelState.AddModelError(nameof(data.ProductName), "Tên mặt hàng bị trùng");
                        return View("Edit", data);
                    }
                    //return Json(data);
                }
                return RedirectToAction("Index");
            }

            catch (Exception ex)
            {
                ModelState.AddModelError("Error", "không lưu được dữ liệu vui lòng thử lại sau");
                return View("Edit", data);
            }
        }
        public IActionResult Delete(int id = 0)
        {
            if (Request.Method == "POST")
            {
                ProductDataService.DeleteProduct(id);
                return RedirectToAction("Index");
            }
            var model = ProductDataService.GetProduct(id);
            if (string.IsNullOrEmpty(model.Photo))
                model.Photo = "noneProduct.png";
            if (model == null)
                return RedirectToAction("Index");
            ViewBag.AllowDelete = !ProductDataService.InUsedProduct(id);
            return View(model);
        }


        public IActionResult Photo(int id, string method, int photoId = 0)
        {
            switch (method)
            {
                case "add":
                    ViewBag.Title = "Bổ sung ảnh";
                    ProductPhoto dataC = new ProductPhoto()
                    {
                        ProductID = id,
                        PhotoID = photoId,
                        Photo = "noneProduct.png"
                    };
                    return View(dataC);
                case "edit":
                    ViewBag.Title = "Thay đổi ảnh";
                    ProductPhoto? dataE = ProductDataService.GetPhoto(photoId);
                    if (dataE == null)
                        return RedirectToAction("Edit");
                    if (string.IsNullOrEmpty(dataE.Photo))
                        dataE.Photo = "noneProduct.png";
                    return View(dataE);
                case "delete":
                    //TODO: Xóa ảnh (Xóa trực tiếp, không cần xác nhận)
                    ProductDataService.DeletePhoto(photoId);
                    return RedirectToAction("Edit", new { id = id });
                default:
                    return RedirectToAction("Edit");
            }
        }
        [HttpPost]
        public IActionResult SavePhoto(ProductPhoto data, IFormFile? uploadPhoto)
        {
            try
            {

                if (string.IsNullOrWhiteSpace(data.Description))
                    ModelState.AddModelError(nameof(data.Description), "Mô tả không được để trống");

                if (data.DisplayOrder <= 0)
                    ModelState.AddModelError(nameof(data.DisplayOrder), "Vui lòng chọn thứ tự hiển thị");
                if (!ModelState.IsValid)
                {
                    ViewBag.Tittle = data.PhotoID == 0 ? "Bổ sung ảnh" : "Thay đổi ảnh";
                    return View("Photo", data);
                }
                if (uploadPhoto != null)
                {

                    string filename = $"{DateTime.Now.Ticks}_{uploadPhoto.FileName}";
                    string folder = Path.Combine(ApplicationContext.HostEnviroment.WebRootPath, @"images\products");
                    string filepath = Path.Combine(folder, filename);
                    using (var stream = new FileStream(filepath, FileMode.Create))
                    {
                        uploadPhoto.CopyTo(stream);
                    }
                    data.Photo = filename;
                }
                if (data.PhotoID == 0)
                {
                    long id = ProductDataService.AddPhoto(data);
                }

                else
                {
                    bool result = ProductDataService.UpdatePhoto(data);
                }
                return RedirectToAction("Edit", new { id = data.ProductID });
            }
            catch (Exception ex)
            {
                ModelState.AddModelError("Error", "không lưu được dữ liệu vui lòng thử lại sau");
                return View("Edit", data);
            }
        }
        public IActionResult Attribute(int id, string method, int attributeId = 0)
        {
            switch (method)
            {
                case "add":
                    ViewBag.Title = "Bổ sung thuộc tính";
                    ProductAttribute dataC = new ProductAttribute()
                    {
                        ProductID = id,
                        AttributeID = 0,
                    };
                    return View(dataC);
                case "edit":
                    ViewBag.Title = "Thay đổi thuộc tính";
                    ProductAttribute? data = ProductDataService.GetAttribute(attributeId);
                    if (data == null)
                        return RedirectToAction("Edit");
                    return View(data);
                case "delete":
                    //TODO: Xóa ảnh (Xóa trực tiếp, không cần xác nhận)
                    ProductDataService.DeleteAttribute(attributeId);
                    return RedirectToAction("Edit", new { id = id });
                default:
                    return RedirectToAction("Edit");
            }
        }
        [HttpPost]
        public IActionResult SaveAttribute(ProductAttribute data)
        {
            try
            {
                if (string.IsNullOrWhiteSpace(data.AttributeValue))
                    ModelState.AddModelError(nameof(data.AttributeValue), "giá trị thuộc tính không được để trống");
                if (string.IsNullOrWhiteSpace(data.AttributeName))
                    ModelState.AddModelError(nameof(data.AttributeName), "Tên thuộc tính không được bỏ trống");
                if (data.DisplayOrder <= 0)
                    ModelState.AddModelError(nameof(data.DisplayOrder), "Vui lòng chọn thứ tự hiển thị");
                if (!ModelState.IsValid)
                {
                    ViewBag.Tittle = data.AttributeID == 0 ? "Bổ sung thuộc tính" : "Thay đổi thuộc tính";
                    return View("Attribute", data);
                }
                if (data.AttributeID == 0)
                {
                    long id = ProductDataService.AddAttribute(data);
                }
                else
                {
                    bool result = ProductDataService.UpdateAttribute(data);
                }
                //return Json(data);
                return RedirectToAction("Edit", new { id = data.ProductID });
            }
            catch (Exception e)
            {
                ModelState.AddModelError("Error", "không lưu được dữ liệu vui lòng thử lại sau");
                return View("Edit", data);
            }
        }
    }
}
