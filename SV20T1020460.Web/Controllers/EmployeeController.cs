using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using SV20T1020460.BusinessLayers;
using SV20T1020460.DomainModels;
using SV20T1020460.Web.Models;
using System.Buffers;
using System.ComponentModel.DataAnnotations;
using System.Text.RegularExpressions;

namespace SV20T1020460.Web.Controllers
{
    [Authorize(Roles = $"{WebUserRoles.Administrator}")]
    public class EmployeeController : Controller
    {
        private const int PAGE_SIZE = 20;
        private const string EMPLOYEE_SEARCH = "employee_search";
        public IActionResult Index()
        {
            PaginationSearchInput? input = ApplicationContext.GetSessionData<PaginationSearchInput>(EMPLOYEE_SEARCH);
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
            var data = CommomDataService.ListOfEmployees(out rowCount, input.Page, input.PageSize, input.SearchValue ?? "");
            var model = new EmployeeSearchResult()
            {
                Page = input.Page,
                PageSize = input.PageSize,
                SearchValue = input.SearchValue ?? "",
                RowCount = rowCount,
                Data = data
            };
            ApplicationContext.SetSessionData(EMPLOYEE_SEARCH, input);
            return View(model);

        }
        public IActionResult Create()
        {
            ViewBag.Title = "Bổ sung nhân viên";
            Employee model = new Employee()
            {
                EmployeeID = 0,
                BirthDate = new DateTime(1990, 1, 1),
                Photo = "employee.png"
            };
            return View("Edit", model);
            //return Json(model);
        }
        public IActionResult Edit(int id = 0)
        {
            ViewBag.Title = "Cập nhật nhân viên";
            Employee? model = CommomDataService.GetEmployee(id);

            if (model == null)
                return RedirectToAction("Index");
            if (string.IsNullOrEmpty(model.Photo))
                model.Photo = "employee.png";
            return View(model);
        }
        [HttpPost]
        public IActionResult Save(Employee data, string BirthDateInput, IFormFile? uploadPhoto)
        {
            try
            {
                
                DateTime? birthDate = BirthDateInput.StringToDateTime();
                if (birthDate != null)//BirthDate.hasvalue
                    data.BirthDate = birthDate.Value;
                //xử lý ảnh upload(nếu có ảnh upload thì lưu ảnh và gán lại tên file ảnh mới cho employee)
                if (uploadPhoto != null)
                {
                    string fileName = $"{DateTime.Now.Ticks}_{uploadPhoto.FileName}";//tên file sẽ lưu
                    string folder = Path.Combine(ApplicationContext.HostEnviroment.WebRootPath, @"images\employees");//đường dẫn đến thư mục lưu file
                    string filepath = Path.Combine(folder, fileName);//đường dẫn đến file cần lưu
                    using (var stream = new FileStream(filepath, FileMode.Create))
                    {
                        uploadPhoto.CopyTo(stream);
                    }
                    data.Photo = fileName;
                }
                if (string.IsNullOrWhiteSpace(data.FullName))
                    ModelState.AddModelError(nameof(data.FullName), "Tên không được để trống");
                if (string.IsNullOrWhiteSpace(data.Phone))
                    ModelState.AddModelError(nameof(data.Phone), "Số điện thoại không được để trống");
                if (string.IsNullOrWhiteSpace(data.Email))
                    ModelState.AddModelError(nameof(data.Email), "Email không được để trống");
                if (!Regex.IsMatch(data.FullName, @"^[\p{L}\s']+$"))
                    ModelState.AddModelError(nameof(data.FullName), "Tên bắt buộc ký tự dạng chữ");
                if (!Regex.IsMatch(data.Phone, @"^0\d{9}$"))
                    ModelState.AddModelError(nameof(data.Phone), "Số điện thoại không hợp lệ");
                if (!Regex.IsMatch(data.Phone, @"^\d{10}$"))
                    ModelState.AddModelError(nameof(data.Phone), "Số điện thoại phải nhập đúng 10 ký tự");
                if (!Regex.IsMatch(data.Email, @"^[a-zA-Z0-9._%+-]+@[a-zA-Z0-9.-]+\.[a-zA-Z]{2,}$"))
                    ModelState.AddModelError(nameof(data.Email), "Địa chỉ email không hợp lệ");
                //xử lý ngày sinh
                if (!ModelState.IsValid)
                {
                    ViewBag.Tittle = data.EmployeeID == 0 ? "Bổ sung nhân viên" : "Cập nhật thông tin nhân viên";
                }
                if (data.EmployeeID == 0)
                {
                    int id = CommomDataService.AddEmployee(data);
                    if (id < 0)
                    {
                        ModelState.AddModelError(nameof(data.Email), "Địa chỉ email bị trùng");
                        return View("Edit", data);
                    }
                }
                else
                {
                    bool result = CommomDataService.UpdateEmployee(data);
                    if (!result)
                    {
                        ModelState.AddModelError(nameof(data.Email), "Địa chỉ email bị trùng");
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
                CommomDataService.DeleteEmployee(id);
                return RedirectToAction("Index");
            }
            var model = CommomDataService.GetEmployee(id);
            if (string.IsNullOrEmpty(model.Photo))
                model.Photo = "th.jpg";
            if (model == null)
                return RedirectToAction("Index");
            ViewBag.AllowDelete = !CommomDataService.IsUsedEmployee(id);
            return View(model);
        }
    }
}
