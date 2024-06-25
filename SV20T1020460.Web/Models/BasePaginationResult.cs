using Microsoft.EntityFrameworkCore.Metadata.Conventions;
using SV20T1020460.DomainModels;

namespace SV20T1020460.Web.Models
{
    /// <summary>
    /// lớp cha cho các lớp để biểu diễn dữ liệu các kết quả liên quan đến tìm kiếm và phân trang
    /// </summary>
    public abstract class BasePaginationResult
    {
        public int Page { get; set; }   
        public int PageSize { get; set; }
        public string SearchValue { get; set; } = "";
        public int RowCount { get; set; }
        public int categoryID { get; set;}
        public int supplierID {  get; set; }
        public decimal minPrice {  get; set; }
        public decimal maxPrice { get; set; }
        public int PageCount
        {
            get
            {
                if (PageSize == 0)
                    return 1;
                int c = RowCount / PageSize;
                if (RowCount % PageSize > 0)
                    c += 1;
                return c;
            }
        }
    }
    /// <summary>
    /// kết quả tìm kiếm và lấy danh sachs khách hàng
    /// </summary>
    public class CustomerSearchResult : BasePaginationResult
    {
        public List<Customer> Data { get; set; }
    }
    public class CategorySearchResult : BasePaginationResult
    {
        public List<Category> Data { get; set; }
    }
    public class SupplierSearchResult : BasePaginationResult
    {
        public List<Supplier> Data { get; set; }
    }
    public class ShipperSearchResult : BasePaginationResult
    {
        public List<Shipper> Data { get; set; }
    }
    public class EmployeeSearchResult : BasePaginationResult
    {
        public List<Employee> Data { get; set; }
    }
    public class ProvinceSearchResult : BasePaginationResult
    {
        public List<Province> Data { get; set; }
    }
    public class ProductSearchResult : BasePaginationResult
    {
        public List<Product> Data { get; set; }
    }
    public class ProductAttributeSearchResult : BasePaginationResult
    {
        public List<ProductAttribute> Data { get; set; }
    }
    public class ProductPhotoSearchResult : BasePaginationResult
    {
        public List<ProductPhoto> Data { get; set; }
    }
}
