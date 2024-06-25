using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore.Storage.ValueConversion;
using System.Globalization;

namespace SV20T1020460.Web.Controllers
{
    public class TestController : Controller
    {
        public IActionResult Create()
        {
            var model = new Models.Person
            {
                Name = "Trần Hữu Nhật Minh",
                BirthDate = new DateTime(2025-02-25) ,
                Salary = 10.2m
            };
            return View(model);
        }
        public IActionResult Save(Models.Person model, string BirthDateInput = "")
        {
            //chuyển BirthDateInput sang giá trị kiểu ngày
            DateTime? dvalue = StringToDateTime(BirthDateInput);//kiểm tra coi có chuyển được về dữ liệu ngày tháng năm không
            if(dvalue.HasValue)
            {
                model.BirthDate = dvalue.Value;//lấy giá trị nếu dữ liệu truyền vào chuyển được về kiểu ngày tháng năm
            }
                return Json(model);
        }
        private DateTime? StringToDateTime(string s, string formats = "d/M/yyyy;d-M-yyyy;d.M.yyyy")
        {
            try
            {
                return DateTime.ParseExact(s, formats.Split(';'), CultureInfo.InvariantCulture);
            }
            catch
            {
                return null;
            }
        }
    }
}
