using SV20T1020460.DomainModels;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SV20T1020460.DataLayers
{
    public interface IProductDAL
    {
        /// <summary>
        /// tìm kiếm và lấy danh sách mặt hàng dưới dạng phân trang
        /// </summary>
        /// <param name="page">Trang cần hiện thị</param>
        /// <param name="pageSize">số dòng trên mỗi trang (0 nếu không phân trang)</param>
        /// <param name="searchValue">tên mặt hàng cần tìm(chuỗi rỗng nếu không cần tìm kiếm)</param>
        /// <param name="categoryID">mã loại hàng cần tìm (0 nếu không tìm theo loại hàng)</param>
        /// <param name="supplierID">mã nhà cung cấp cần tìm (0 nếu không tìm theo nhà cung cấp)</param>
        /// <param name="minPrice">mức giá nhỏ nhất trong khoảng giá cần tìm</param>
        /// <param name="maxPrice">mức giá lớn nhất trong khoảng giá cần tìm(0 nếu không hạn chế mức giá lớn nhất)</param>
        /// <returns></returns>
        IList<Product> List(int page = 1, int pageSize = 0, string searchValue = "", int categoryID = 0, int supplierID = 0, decimal minPrice = 0, decimal maxPrice = 0);
        /// <summary>
        /// đếm số lượng mặt hàng tìm kiếm được
        /// </summary>
        /// <param name="searchValue">tên mặt hàng cần tìm (chuỗi rỗng nếu không tìm kiếm)</param>
        /// <param name="categoryID">mã loại hàng cần tìm (0 nếu không tìm theo loại hàng)</param>
        /// <param name="supplierID">mã nhà cung cấp cần tìm (0 nếu không tìm theo nhà cung cấp)</param>
        /// <param name="minPrice">mức giá nhỏ nhất trong khoảng giá cần tìm</param>
        /// <param name="maxPrice">mức giá lớn nhất trong khoảng giá cần tìm (0 nếu không hạn chế mức giá lớn nhất)</param>
        /// <returns></returns>
        int Count(string searchValue = "", int categoryID = 0, int supplierID = 0, decimal minPrice = 0, decimal maxPrice = 0);
        /// <summary>
        /// lấy thông tin mặt hàng theo mã hàng
        /// </summary>
        /// <param name="productID"></param>
        /// <returns></returns>
        Product? Get(int productID);
        /// <summary>
        /// bổ sung mặt hàng mới (hàm trả về mã của mặt hàng được bổ sung) 
        /// </summary>
        /// <param name="data"></param>
        /// <returns></returns>
        int Add(Product data);
        /// <summary>
        /// cập nhật thông tin mặt hàng
        /// </summary>
        /// <param name="data"></param>
        /// <returns></returns>
        bool Update(Product data);
        /// <summary>
        /// xóa mặt hàng
        /// </summary>
        /// <param name="productID"></param>
        /// <returns></returns>
        bool Delete(int productID);
        /// <summary>
        /// kiểm tra mặt hàng hiện có đơn hàng liên quan hay không?
        /// </summary>
        /// <param name="productID"></param>
        /// <returns></returns>
        bool InUsed(int productID);

        //---------------------ProductPhoto--------------------

        /// <summary>
        /// lấy dnah sách ảnh của mặt hàng (sắp xếp theo thứ tự của DisplayOrder)
        /// </summary>
        /// <param name="productID"></param>
        /// <returns></returns>
        IList<ProductPhoto> ListPhotos(int productID);
        /// <summary>
        /// lấy thông tin 1 ảnh dựa vào ID
        /// </summary>
        /// <param name="photoID"></param>
        /// <returns></returns>
        ProductPhoto? GetPhoto(long photoID);
        /// <summary>
        /// bổ sung 1 ảnh cho mặt hàng( hàm trả về mã của ảnh được bổ sung)
        /// </summary>
        /// <param name="data"></param>
        /// <returns></returns>
        long AddPhoto(ProductPhoto data);
        /// <summary>
        /// cập nhật ảnh của mặt hàng
        /// </summary>
        /// <param name="data"></param>
        /// <returns></returns>
        bool UpdatePhoto(ProductPhoto data);
        /// <summary>
        /// xóa ảnh của mặt hàng
        /// </summary>
        /// <param name="photoID"></param>
        /// <returns></returns>
        bool DeletePhoto(long photoID);

        //---------------------ProductAttribute--------------------

        /// <summary>
        /// lấy danh sách các thuộc tính của mặt hàng, sắp xếp theo thứ tự của DisplayOrder
        /// </summary>
        /// <param name="productID"></param>
        /// <returns></returns>
        IList<ProductAttribute> ListAttributes(int productID);
        /// <summary>
        /// lấy thông tin của thuộc tính theo mã thuộc tính
        /// </summary>
        /// <param name="attributeID"></param>
        /// <returns></returns>
        ProductAttribute? GetAttribute(long attributeID);
        /// <summary>
        /// bổ sung thuộc tính cho mặt hàng
        /// </summary>
        /// <param name="data"></param>
        /// <returns></returns>
        long AddAttribute(ProductAttribute data);
        /// <summary>
        /// cập nhật thuộc tính cho mặt hàng
        /// </summary>
        /// <param name="data"></param>
        /// <returns></returns>
        bool UpdateAttribute(ProductAttribute data);
        /// <summary>
        /// xóa thuộc tính
        /// </summary>
        /// <param name="attributeID"></param>
        /// <returns></returns>
        bool DeleteAttribute(long attributeID);
    }
}
