using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using SV20T1020460.BusinessLayers;
using SV20T1020460.DomainModels;
using SV20T1020460.Web.Models;
using System.Drawing.Text;
using System.Text.RegularExpressions;

namespace SV20T1020460.Web.Controllers
{
    [Authorize(Roles = $"{WebUserRoles.Administrator},{WebUserRoles.Employee}")]
    public class CustomerController : Controller
    {
        private const int PAGE_SIZE = 20;
        private const string CUSTOMER_SEARCH = "customer_search"; // tên biến để lưu trong session
        public IActionResult Index()
        {
            //lấy đầu vào tìm kiếm hiện đang lưu lại trong session 
            PaginationSearchInput? input = ApplicationContext.GetSessionData<PaginationSearchInput>(CUSTOMER_SEARCH);
            //trường hợp trong session chưa có điều kiện được lưu lại thì tạo điều kiện mới để dùng
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
            var data = CommomDataService.ListOfCustomers(out rowCount, input.Page, input.PageSize, input.SearchValue?? "");
            var model = new CustomerSearchResult() 
            {
                Page = input.Page,
                PageSize=input.PageSize,
                SearchValue = input.SearchValue ?? "",
                RowCount = rowCount,
                Data = data
            };

            //lưu lại điều kiện tìm kiếm vào trong session
            ApplicationContext.SetSessionData(CUSTOMER_SEARCH, input);    

            return View(model);
        }

        public IActionResult Create()
        {
            ViewBag.Title = "Bổ sung khách hàng";
            Customer model = new Customer()
            {
                CustomerId = 0
            }; 
            return View("Edit", model);
        }
        public IActionResult Edit(int id = 0)
        {
            ViewBag.Title = "Cập nhật thông tin khách hàng";
            Customer? model = CommomDataService.GetCustomer(id);
            if(model == null)
                return RedirectToAction("Index");

            return View(model);
        }
        /// <summary>
        /// nhận dữ liệu với phương thức POST
        /// </summary>
        [HttpPost]
        // thay vì khai báo từng tham số cho Action để nhận dữ liệu, thì nên dùng model
        //model (view model): là 1 class có các thuộc tính trùng tên với ccasc tham số
        public IActionResult Save (Customer data) //("Customer data" == String customerID, customerName,..)
        {
            try
            {
                //kiểm soast dữ liệu dầu vào và đưa các thông báo lỗi vào trong ModelState (nếu có)
                if (string.IsNullOrWhiteSpace(data.CustomerName))
                    ModelState.AddModelError(nameof(data.CustomerName), "Tên không được để trống");
                if (string.IsNullOrWhiteSpace(data.ContactName))
                    ModelState.AddModelError(nameof(data.ContactName), "Tên giao dịch không được để trống");
                if (string.IsNullOrWhiteSpace(data.Phone))
                    ModelState.AddModelError(nameof(data.Phone), "Số điện thoại không được để trống");
                if (string.IsNullOrWhiteSpace(data.Email))
                    ModelState.AddModelError(nameof(data.Email), "Vui lòng nhập email khách hàng");
                if (string.IsNullOrEmpty(data.Province))
                    ModelState.AddModelError(nameof(data.Province), "Vui lòng chọn tỉnh thành");
                if (!Regex.IsMatch(data.Phone, @"^0\d{9}$"))
                    ModelState.AddModelError(nameof(data.Phone), "Số điện thoại không hợp lệ");
                if (!Regex.IsMatch(data.Phone, @"^\d{10}$"))
                    ModelState.AddModelError(nameof(data.Phone), "Số điện thoại phải nhập đúng 10 ký tự");
                if (!Regex.IsMatch(data.CustomerName, @"^[\p{L}\s']+$"))
                    ModelState.AddModelError(nameof(data.CustomerName), "Tên bắt buộc ký tự dạng chữ");
                if (!Regex.IsMatch(data.Email, @"^[a-zA-Z0-9._%+-]+@[a-zA-Z0-9.-]+\.[a-zA-Z]{2,}$"))
                    ModelState.AddModelError(nameof(data.Email), "Địa chỉ email không hợp lệ");

                //thông qua thuộc tính Isvalid của ModelState để kiểm tra xem có tồn tại lỗi hay không

                if (!ModelState.IsValid)
                {
                    ViewBag.Tittle = data.CustomerId == 0 ? "Bổ sung khách hàng" : "Cập nhật thông tin khách hàng";
                    return View("Edit", data);
                }

                if (data.CustomerId == 0)
                {
                    int id = CommomDataService.AddCustomer(data);
                    if(id <= 0)
                    {
                        ModelState.AddModelError(nameof(data.Email), "Địa chỉ email bị trùng");
                        return View("Edit", data);
                    }
                    //return Json(data);
                }
                else
                {
                    bool result = CommomDataService.UpdateCustomer(data);
                    if (!result)
                    {
                        ModelState.AddModelError(nameof(data.Email), "Địa chỉ Email trùng với khách hàng khác");
                        return View("Edit", data);
                    }
                }
                return RedirectToAction("Index");
            }
            catch (Exception ex)
            {
                ModelState.AddModelError("Error","không lưu được dữ liệu vui lòng thử lại sau");
                return View("Edit", data);
            }
        }
        public IActionResult Delete(int id = 0)
        {   
            if(Request.Method == "POST")
            {
                CommomDataService.DeleteCustomer(id);
                return RedirectToAction("Index");
            }
            var model = CommomDataService.GetCustomer(id);
            if(model == null)
                return RedirectToAction("Index");
                ViewBag.AllowDelete = !CommomDataService.IsUsedCustomer(id);//cho phép xóa trong trường hợp not Isused
            return View(model);
        }

    }
}
