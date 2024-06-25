using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using SV20T1020460.BusinessLayers;
using SV20T1020460.DomainModels;
using SV20T1020460.Web.Models;
using System.Text.RegularExpressions;

namespace SV20T1020460.Web.Controllers
{
    [Authorize(Roles = $"{WebUserRoles.Administrator},{WebUserRoles.Employee}")]
    public class SupplierController : Controller
    {
        private const int PAGE_SIZE = 20;
        private const string SUPPLIER_SEARCH = "supplier_search";
        public IActionResult Index()
        {
            PaginationSearchInput? input = ApplicationContext.GetSessionData<PaginationSearchInput>(SUPPLIER_SEARCH);
            if (input == null)
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
            var data = CommomDataService.ListOfSuppliers(out rowCount, input.Page, input.PageSize, input.SearchValue ?? "");
            var model = new SupplierSearchResult()
            {
                Page = input.Page,
                PageSize = input.PageSize,
                SearchValue = input.SearchValue ?? "",
                RowCount = rowCount,
                Data = data
            };
            ApplicationContext.SetSessionData(SUPPLIER_SEARCH, model);
            return View(model);
        }
        public IActionResult Create()
        {
            ViewBag.Title = "Bổ sung nhà cung cấp";
            Supplier model = new Supplier()
            {
                SupplierID = 0,
            };
            return View("Edit", model);
        }
        public IActionResult Edit(int id = 0)
        {
            ViewBag.Title = "Cập nhật thông tin nhà cung cấp";
            Supplier? model = CommomDataService.GetSupplier(id);
            if(model == null) 
                return RedirectToAction("Index");
            return View(model);
        }
        [HttpPost]
        public IActionResult Save(Supplier data)
        {
            try
            {
                if (string.IsNullOrWhiteSpace(data.SupplierName))
                    ModelState.AddModelError(nameof(data.SupplierName), "Tên không được để trống");
                if (string.IsNullOrWhiteSpace(data.ContactName))
                    ModelState.AddModelError(nameof(data.ContactName), "Tên giao dịch không được để trống");
                if (string.IsNullOrWhiteSpace(data.Email))
                    ModelState.AddModelError(nameof(data.Email), "Vui lòng nhập email khách hàng");
                if (string.IsNullOrWhiteSpace(data.Phone))
                    ModelState.AddModelError(nameof(data.Phone), "Số điện thoại không được để trống");
                if (string.IsNullOrWhiteSpace(data.Provice))
                    ModelState.AddModelError(nameof(data.Provice), "Vui lòng chọn tỉnh thành");
                if (!Regex.IsMatch(data.SupplierName, @"^[\p{L}\s']+$"))
                    ModelState.AddModelError(nameof(data.SupplierName), "Tên bắt buộc ký tự dạng chữ");
                if (!Regex.IsMatch(data.Email, @"^[a-zA-Z0-9._%+-]+@[a-zA-Z0-9.-]+\.[a-zA-Z]{2,}$"))
                    ModelState.AddModelError(nameof(data.Email), "Địa chỉ email không hợp lệ");
                if (!Regex.IsMatch(data.Phone, @"^0\d{9}$"))
                    ModelState.AddModelError(nameof(data.Phone), "Số điện thoại phải nhập đúng 10 ký tự");
                if (!ModelState.IsValid)
                {
                    ViewBag.Tittle = data.SupplierID == 0 ? "Bổ sung nhà cung cấp" : "Cập nhật thông tin nhà cung cấp";
                    return View("Edit", data);
                }
                if (data.SupplierID == 0)
                {
                    int id = CommomDataService.AddSupplier(data);
                    
                }
                else
                {
                    bool result = CommomDataService.UpdateSupplier(data);

                    
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
            var model = CommomDataService.GetSupplier(id);
            if (Request.Method == "POST")
            {
                CommomDataService.DeleteSupplier(id);
                return RedirectToAction("Index");
            }
            if (model == null)
                return RedirectToAction("Index");
            ViewBag.AllowDelete = !CommomDataService.IsUsedSupplier(id);
            return View(model);
        }
    }
}
