using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using SV20T1020460.BusinessLayers;
using SV20T1020460.DomainModels;
using SV20T1020460.Web.Models;
using System.Buffers;
using System.Text.RegularExpressions;

namespace SV20T1020460.Web.Controllers
{
    [Authorize(Roles = $"{WebUserRoles.Administrator},{WebUserRoles.Employee}")]
    public class ShipperController : Controller
    {
        private const int PAGE_SZE = 20;
        private const string SHIPPER_SEARCH = "shipper_search";
        public IActionResult Index( )
        {
            PaginationSearchInput? input = ApplicationContext.GetSessionData<PaginationSearchInput>(SHIPPER_SEARCH);
            if (input == null)
            {
                input = new PaginationSearchInput()
                {
                    Page = 1,
                    PageSize = PAGE_SZE,
                    SearchValue = ""
                };
            }
            return View(input);
        }
        public IActionResult Search(PaginationSearchInput input)
        {
            int rowCount = 0;
            var data = CommomDataService.ListOfShippers(out rowCount, input.Page, input.PageSize, input.SearchValue ?? "");
            var model = new ShipperSearchResult()
            {
                Page = input.Page,
                PageSize = input.PageSize,
                SearchValue = input.SearchValue ?? "",
                RowCount = rowCount,
                Data = data
            };
            ApplicationContext.SetSessionData(SHIPPER_SEARCH, input);
            return View(model);
        }
        public IActionResult Create()
        {
            ViewBag.Title = "Bổ sung người giao hàng";
            Shipper model = new Shipper()
            {
                ShipperID = 0
            };
            return View("Edit", model);
        }
        public IActionResult Edit(int id = 0)
        {
            ViewBag.Title = "Cập nhật thông tin người giao hàng";
            Shipper? model = CommomDataService.GetShipper(id);
            if (model == null)
                return RedirectToAction("Index");
            return View(model);
        }
        [HttpPost]
        public IActionResult Save(Shipper data) 
        {
            try
            {
                if (string.IsNullOrWhiteSpace(data.ShipperName))
                    ModelState.AddModelError("ShipperName", "Tên không được để trống");
                if (string.IsNullOrWhiteSpace(data.Phone))
                    ModelState.AddModelError("Phone", "Số điện thoại không được để trống");
                if (!Regex.IsMatch(data.ShipperName, @"^[\p{L}\s']+$"))
                    ModelState.AddModelError(nameof(data.ShipperName), "Tên bắt buộc ký tự dạng chữ");
                if (!Regex.IsMatch(data.Phone, @"^\d{10}$"))
                    ModelState.AddModelError(nameof(data.Phone), "Số điện thoại phải nhập đúng 10 ký tự");
                if (!Regex.IsMatch(data.Phone, @"^0\d{9}$"))
                    ModelState.AddModelError(nameof(data.Phone), "Số điện thoại không hợp lệ");
                if (!ModelState.IsValid)
                {
                    ViewBag.Tittle = data.ShipperID == 0 ? "Bổ sung người giao hàng" : "Cập nhật thông tin người giao hàng";
                    return View("Edit", data);
                }
                if (data.ShipperID == 0)
                {
                    int id = CommomDataService.AddShipper(data);
                    if(id <= 0)
                    {
                        ModelState.AddModelError(nameof(data.Phone), "Số điện thoại bị trùng");
                        return View("Edit", data);
                    }
                }
                else
                {
                    bool result = CommomDataService.UpdateShipper(data);
                    if (!result)
                    {
                        ModelState.AddModelError(nameof(data.Phone), "Số điện thoại bị trùng");
                        return View("Edit", data);
                    }
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
            var model = CommomDataService.GetShipper(id);
            if (Request.Method == "POST")
            {
                CommomDataService.DeleteShipper(id);
                return RedirectToAction("Index");
            }
            if (model == null)
                return RedirectToAction("Index"); 
            ViewBag.AllowDelete = !CommomDataService.IsUsedShipper(id);
            return View(model);
        }
    }
}
