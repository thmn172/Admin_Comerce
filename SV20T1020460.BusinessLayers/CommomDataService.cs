using SV20T1020460.DataLayers;
using SV20T1020460.DataLayers.SQLServer;
using SV20T1020460.DomainModels;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SV20T1020460.BusinessLayers
{
    /// <summary>
    /// cung cấp các chức năng xử lý dữ liệu chung
    ///(tính/thành, khách hàng, ncc, loại hàng, người giao hàng, nhân viên)
    /// </summary>
    
    public static class CommomDataService
    {
        private static readonly ICommonDAL<Customer> customerDB;
        private static readonly ICommonDAL<Province> provinceDB;
        private static readonly ICommonDAL<Employee> employeeDB;
        private static readonly ICommonDAL<Category> categoryDB;
        private static readonly ICommonDAL<Shipper>  shipperDB;
        private static readonly ICommonDAL<Supplier> supplierDB;

        static CommomDataService()
        {
            String connetionString = Configuration.ConnectionString;
            provinceDB = new ProvinceDAL(connetionString);
            customerDB = new CustomerDAL(connetionString);
            employeeDB = new EmployeeDAL(connetionString);
            categoryDB = new CategoryDAL(connetionString);  
            supplierDB = new SupplierDAL(connetionString);
            shipperDB = new ShipperDAL(connetionString);
        }
        public static List<Province> ListOfProvinces()
        {
            return provinceDB.List().ToList();
        }
        /// <summary>
        /// Tìm kiếm và lấy danh sách khách hàng
        /// </summary>
        /// <param name="rowCount"></param>
        /// <param name="page"></param>
        /// <param name="pageSize"></param>
        /// <param name="searchValue"></param>
        /// <returns></returns>
        //---------------------------Customer------------------------------------
        public static List<Customer> ListOfCustomers(out int rowCount, int page = 1, int pageSize = 0, string searchValue = "")
        {
            rowCount = customerDB.Count(searchValue);
            return customerDB.List(page, pageSize, searchValue).ToList();
        }
        /// <summary>
        /// dựa vào danh sách khách hàng để lấy ID của khách hàng đó
        /// </summary>
        /// <param name="id"></param>
        /// <returns></returns>
        public static Customer? GetCustomer ( int id)
        {
            return customerDB.Get(id);  
        }
        /// <summary>
        /// bổ sung khách hàng
        /// </summary>
        /// <param name="customer"></param>
        /// <returns></returns>
        public static int AddCustomer(Customer customer)
        {
            return customerDB.Add(customer);
        }
        /// <summary>
        /// cập nhật thông tin khách hàng
        /// </summary>
        /// <param name="customer"></param>
        /// <returns></returns>
        public static bool UpdateCustomer(Customer customer)
        {
            return customerDB.Update(customer);
        }
        /// <summary>
        /// xóa khách hàng, không cho xóa nếu khashc hàng đang có dữ liệu liên quan
        /// </summary>
        /// <param name="id"></param>
        /// <returns></returns>
        public static bool DeleteCustomer(int id)
        {
            if(customerDB.IsUsed(id)) 
                return false;
            return customerDB.Delete(id);
        }
        /// <summary>
        /// kiểm tra xem 1 khashc hàng đang có dữ liệu liên quan không
        /// </summary>
        /// <param name="id"></param>
        /// <returns></returns>
        public static bool IsUsedCustomer(int id)
        {
            return customerDB.IsUsed(id);
        }

        //---------------------------Supplier------------------------------------
        
        /// <summary>
        /// Tìm kiếm và lấy danh sách nhà cung cấp
        /// </summary>
        /// <param name="rowCount"></param>
        /// <param name="page"></param>
        /// <param name="pageSize"></param>
        /// <param name="searchValue"></param>
        /// <returns></returns>
        public static List<Supplier> ListOfSuppliers (out int rowCount, int page = 1, int pageSize = 0, string searchValue = "")
        {
            rowCount = supplierDB.Count(searchValue);
            return supplierDB.List(page, pageSize, searchValue).ToList();
        }
        /// <summary>
        /// lấy thông tin của 1 nhà cung cấp theo mã nhà cung cấp
        /// </summary>
        /// <param name="id"></param>
        /// <returns></returns>
        public static Supplier? GetSupplier(int id)
        {
            return supplierDB.Get(id);
        }
        /// <summary>
        /// Bổ sung nhà cung cấp mới
        /// </summary>
        /// <param name="supplier"></param>
        /// <returns></returns>
        public static int AddSupplier(Supplier supplier)
        {
            return supplierDB.Add(supplier);
        }
        /// <summary>
        /// Cập nhật nhà cung cấp
        /// </summary>
        /// <param name="supplier"></param>
        /// <returns></returns>
        public static bool UpdateSupplier(Supplier supplier)
        {
            return supplierDB.Update(supplier);
        }
        /// <summary>
        /// xóa nhà cung cấp có mã là ID
        /// </summary>
        /// <param name="id"></param>
        /// <returns></returns>
        public static bool DeleteSupplier(int id)
        {
            if (supplierDB.IsUsed(id))
                return false;
            return supplierDB.Delete(id);
        }
        /// <summary>
        /// kiểm tra xem nhà cung cấp có mã ID hiện có dữ liệu liên quan hay không
        /// </summary>
        /// <param name="id"></param>
        /// <returns></returns>
        public static bool IsUsedSupplier(int id)
        {
            return supplierDB.IsUsed(id);
        }

        //---------------------------Shipper------------------------------------
        public static List<Shipper> ListOfShippers(out int rowCount, int page = 1, int pageSize = 0, string searchValue = "")
        {
            rowCount = shipperDB.Count(searchValue);
            return shipperDB.List(page, pageSize, searchValue).ToList();
        }
        public static Shipper? GetShipper(int id)
        {
            return shipperDB.Get(id);
        }
        public static int AddShipper(Shipper shipper)
        {
            return shipperDB.Add(shipper);
        }
        public static bool UpdateShipper(Shipper shipper)
        {
            return shipperDB.Update(shipper);
        }
        public static bool DeleteShipper(int id)
        {
            if (shipperDB.IsUsed(id))
                return false;
            return shipperDB.Delete(id);
        }
        public static bool IsUsedShipper(int id)
        {
            return shipperDB.IsUsed(id);
        }

        //---------------------------Employee------------------------------------

        public static List<Employee> ListOfEmployees(out int rowCount, int page = 1, int pageSize = 0, string searchValue = "")
        {
            rowCount = employeeDB.Count(searchValue);
            return employeeDB.List(page, pageSize, searchValue).ToList();
        }
        public static Employee? GetEmployee(int id)
        {
            return employeeDB.Get(id);
        }
        public static int AddEmployee(Employee employee)
        {
            return employeeDB.Add(employee);
        }
        public static bool UpdateEmployee(Employee employee)
        {
            return employeeDB.Update(employee);
        }
        public static bool DeleteEmployee(int id)
        {
            if (employeeDB.IsUsed(id))
                return false;
            return employeeDB.Delete(id);
        }
        public static bool IsUsedEmployee(int id)
        {
            return employeeDB.IsUsed(id);
        }
        
        //---------------------------Category------------------------------------

        public static List<Category> ListOfCategories(out int rowCount, int page = 1, int pageSize = 0, string searchValue = "")
        {
            rowCount = categoryDB.Count(searchValue);
            return categoryDB.List(page, pageSize, searchValue).ToList();
        }
        public static Category? GetCategory(int id)
        {
            return categoryDB.Get(id);
        }
        public static int AddCategory(Category category)
        {
            return categoryDB.Add(category);
        }
        public static bool UpdateCategory(Category category)
        {
            return categoryDB.Update(category);
        }
        public static bool DeleteCategory(int id)
        {
            if (categoryDB.IsUsed(id))
                return false;
            return categoryDB.Delete(id);
        }
        public static bool IsUsedCategory(int id)
        {
            return categoryDB.IsUsed(id);
        }
    }
}
