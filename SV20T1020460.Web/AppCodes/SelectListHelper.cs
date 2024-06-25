using Microsoft.AspNetCore.Mvc.Rendering;
using SV20T1020460.BusinessLayers;
using SV20T1020460.DomainModels;

namespace SV20T1020460.Web
{
    public static class SelectListHelper
    {
        public static List<SelectListItem> Province()
        {

            List <SelectListItem> list = new List<SelectListItem>();
            list.Add(new SelectListItem()
            {
                Value =  "",
                Text = "--- Chọn tỉnh/thành --"
            });
            foreach(var item in CommomDataService.ListOfProvinces())
            {
                list.Add(new SelectListItem()
                {
                    Value = item.ProvinceName,
                    Text = item.ProvinceName
                });
            }
            return list;
        }
        
        public static List <SelectListItem> Category() 
        {
            List<SelectListItem > list = new List<SelectListItem>();
            list.Add(new SelectListItem()
            {
                Value = "",
                Text = "--- Chọn loại hàng ---"
            });
            int rowCount = 0;
            int page = 1;
            int PAGE_SIZE = 20;
            string searchValue = "";
            foreach (var i in CommomDataService.ListOfCategories(out rowCount, page, PAGE_SIZE, searchValue ?? ""))
            {
                list.Add(new SelectListItem()
                {
                    Value = i.CategoryID.ToString(),
                    Text = i.CategoryName
                });
            }
            return list;
        }
        public static List<SelectListItem> Supplier()
        {
            List<SelectListItem> list = new List<SelectListItem> ();
            list.Add(new SelectListItem()
            {
                Value = "",
                Text = "--- Chọn nhà cung cấp ---"
            });
            int rowCount = 0;
            int page = 1;
            int PAGE_SIZE = 20;
            string searchValue = "";
            foreach (var item in CommomDataService.ListOfSuppliers(out rowCount, page, PAGE_SIZE, searchValue ?? ""))
            {
                list.Add(new SelectListItem()
                {
                    Value = item.SupplierID.ToString(),
                    Text = item.SupplierName
                });
            }
            return list;
        }
        //public static List<SelectListItem> Customer()
        //{
        //    List<SelectListItem> list = new List<SelectListItem>();
        //    list.Add(new SelectListItem()
        //    {
        //        Value = "",
        //        Text = "--- Chọn khách hàng ---"
        //    });
        //    int rowCount = 0;
        //    int page = 1;
        //    int PAGE_SIZE = 20;
        //    string searchValue = "";
        //    foreach (var item in SV20T1020460.BusinessLayers.CommomDataService.ListOfCustomers(out rowCount, page, PAGE_SIZE, searchValue ?? ""))
        //    {
        //        list.Add(new SelectListItem()
        //        {
        //            Value = item.CustomerId.ToString(),
        //            Text = item.CustomerName
        //        });
        //    }
        //    return list;
        //}



    }
}
