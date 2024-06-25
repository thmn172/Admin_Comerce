using Dapper;
using Microsoft.IdentityModel.Tokens;
using SV20T1020460.DomainModels;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SV20T1020460.DataLayers.SQLServer
{
    public class ProductDAL : _BaseDAL, IProductDAL
    {
        public ProductDAL(string connectionString) : base(connectionString)
        {
        }

        public int Add(Product data)
        {
            int id = 0;
            using(var connection = OpenConnection())
            {
                var sql = @"if exists(select * from Products where ProductName = @ProductName)
                                select -1
                            else
                                begin
                                    insert into Products(ProductName,ProductDescription,SupplierID,CategoryID,Unit,Price,Photo,IsSelling)
                                    values(@ProductName,@ProductDescription,@SupplierID,@CategoryID,@Unit,@Price,@Photo,@IsSelling);
                                    select @@identity;
                                end";
                var parameters = new
                {
                    ProductName = data.ProductName ?? "",
                    ProductDescription = data.ProductDescription ?? "",
                    CategoryID = data.CategoryID,
                    SupplierID = data.SupplierID,
                    Unit = data.Unit ?? "",
                    Price = data.Price,
                    IsSelling = data.IsSelling,
                    Photo = data.Photo ?? ""
                };
                id = connection.ExecuteScalar<int>(sql: sql, param: parameters, commandType: System.Data.CommandType.Text);
                connection.Close();
            }
            return id;
        }

        public long AddAttribute(ProductAttribute data)
        {
            long id = 0;
            using (var connection = OpenConnection())
            {
                var sql = @"insert into ProductAttributes (ProductID,AttributeName, AttributeValue, DisplayOrder)
	                                    values(@ProductID,@AttributeName, @AttributeValue, @DisplayOrder);
                                        select @@identity";
                var parameters = new
                {
                    ProductID = data.ProductID, 
                    AttributeName = data.AttributeName,
                    AttributeValue = data.AttributeValue,
                    DisplayOrder = data.DisplayOrder
                };
                id = connection.ExecuteScalar<long>(sql: sql, param: parameters, commandType: System.Data.CommandType.Text);
                connection.Close();
            }
            return id;
        }

        public long AddPhoto(ProductPhoto data)
        {
            long id = 0;
            using (var connections = OpenConnection())
            {
                var sql = @"insert into ProductPhotos(ProductID, Description,DisplayOrder,Photo, IsHidden)
                                    values(@ProductID,@Description,@DisplayOrder,@Photo,@IsHidden)
                                    select @@identity";
                var parameters = new
                {
                    ProductID = data.ProductID,
                    Description = data.Description ?? "",
                    DisplayOrder = data.DisplayOrder,
                    Photo = data.Photo ?? "",
                    Ishidden = data.IsHidden
                };
                id = connections.ExecuteScalar<long>(sql:sql, param: parameters, commandType: System.Data.CommandType.Text);
                connections.Close();
            }
            return id;
        }

        public int Count(string searchValue = "", int categoryID = 0, int supplierID = 0, decimal minPrice = 0, decimal maxPrice = 0)
        {
            int count = 0;
            if (!string.IsNullOrEmpty(searchValue))
            {
                searchValue = "%" + searchValue + "%";
            }

            using (var connection = OpenConnection())
            {
                var sql = @"select count(*) from Products
                where (@searchValue = N'' or ProductName like @searchValue)
                and (@CategoryID = 0 or CategoryID = @CategoryID)
                and (@SupplierID = 0 or SupplierId = @SupplierID)
                and (Price >= @MinPrice)
                and (@MaxPrice <= 0 or Price <= @MaxPrice)";
                var parameters = new
                {
                    searchValue = searchValue ?? "",
                    CategoryID = categoryID ,
                    SupplierID = supplierID ,
                    MinPrice = minPrice ,
                    MaxPrice = maxPrice 
                };

                count = connection.ExecuteScalar<int>(sql: sql, param: parameters, commandType: System.Data.CommandType.Text);
                connection.Close();
            }

            return count;
        }

        public bool Delete(int productID)
        {
            bool result = false;
            using (var connection = OpenConnection())
            {
                var sql = @"delete from Products where ProductID = @ProductID";
                var parameters = new
                {
                    ProductID = productID
                };
                result = connection.Execute(sql: sql, param: parameters, commandType: System.Data.CommandType.Text) > 0;
                connection.Close();
            }
            return result;
           
        }

        public bool DeleteAttribute(long attributeID)
        {
            bool result = false;
            using (var connection = OpenConnection())
            {
                var sql = @"delete from ProductAttributes where AttributeID = @AttributeID";
                var parameters = new
                {
                    AttributeID = attributeID
                };
                result = connection.Execute(sql: sql, param: parameters, commandType: System.Data.CommandType.Text) > 0;
                connection.Close();
            }
            return result;

        }

        public bool DeletePhoto(long photoID)
        {
            bool result = false;
            using (var connection = OpenConnection())
            {
                var sql = @"delete from ProductPhotos where PhotoID = @PhotoID";
                var parameters = new
                {
                    PhotoID = photoID
                };
                result = connection.Execute(sql: sql, param: parameters, commandType: System.Data.CommandType.Text) > 0;
                connection.Close();
            }
            return result;

        }

        public Product? Get(int productID)
        {
            Product? data = null;
            using (var connection = OpenConnection())
            {
                var sql = @"select * from Products where ProductID = @ProductID ";
                var parameters = new
                {
                    ProductID = productID
                };
                data = connection.QueryFirstOrDefault<Product>(sql: sql, param: parameters, commandType: System.Data.CommandType.Text);
                connection.Close();
            }
            return data;
        }

        public ProductAttribute? GetAttribute(long attributeID)
        {
            ProductAttribute? data = null;
            using(var connection = OpenConnection())
            {
                var sql = @"select * from ProductAttributes where AttributeID = @AttributeID";
                var parameters = new
                {
                    AttributeID = attributeID
                };
                data = connection.QueryFirstOrDefault<ProductAttribute>(sql:sql, param:parameters, commandType: System.Data.CommandType.Text);
            }
            return data;
        }

        public ProductPhoto? GetPhoto(long photoID)
        {
            ProductPhoto? data = null;
            using (var connection = OpenConnection())
            {
                var sql = @"select * from ProductPhotos where PhotoID = @PhotoID";
                var parameters = new
                {
                    PhotoID = photoID
                };
                data = connection.QueryFirstOrDefault<ProductPhoto>(sql: sql, param: parameters, commandType: System.Data.CommandType.Text);
                connection.Close();
            }
            return data;
        }

        public bool InUsed(int productID)
        {
            bool result = false;
            using (var connection = OpenConnection())
            {
                var sql = @"if exists(select * from OrderDetails where ProductID = @ProductID)
                                select 1
                            else 
                                select 0";
                var parameters = new
                {
                    ProductID = productID
                };
                result = connection.ExecuteScalar<bool>(sql: sql, param: parameters, commandType: System.Data.CommandType.Text);
                connection.Close();
            }
            return result;
        }

        public IList<Product> List(int page = 1, int pageSize = 0, string searchValue = "", int categoryID = 0, int supplierID = 0, decimal minPrice = 0, decimal maxPrice = 0)
        {
            List<Product> data = new List<Product>();
            if (!string.IsNullOrEmpty(searchValue))
            {
                searchValue = "%" + searchValue + "%";
            }

            using (var connection = OpenConnection())
            {
                var sql = @"with cte as
                (
                select  *,
                row_number() over(order by ProductName) as RowNumber
                from    Products
                where   (@SearchValue = N'' or ProductName like @SearchValue)
                and (@CategoryID = 0 or CategoryID = @CategoryID)
                and (@SupplierID = 0 or SupplierId = @SupplierID)
                and (Price >= @MinPrice)
                and (@MaxPrice <= 0 or Price <= @MaxPrice)
                )
                select * from cte
                where   (@PageSize = 0)
                or (RowNumber between (@Page - 1)*@PageSize + 1 and @Page * @PageSize)";

                var parameters = new
                {
                    page = page,
                    pageSize = pageSize,
                    searchValue = searchValue ?? "",
                    categoryID = categoryID,
                    supplierID = supplierID,
                    minPrice = minPrice,
                    maxPrice = maxPrice
                };
                data = connection.Query<Product>(sql: sql, param: parameters, commandType: System.Data.CommandType.Text).ToList();
                connection.Close();

            }

            return data;
        }

        public IList<ProductAttribute> ListAttributes(int productID)
        {
            List<ProductAttribute> data = new List<ProductAttribute>();
            using (var connection = OpenConnection())
            {
                var sql = @"select * from ProductAttributes where ProductID = @ProductID order by DisplayOrder ASC";
                var parameters = new { productID = productID };
                data = connection.Query<ProductAttribute>(sql: sql, param:parameters,commandType: System.Data.CommandType.Text).ToList();
                connection.Close();
            }
            
            return data;
        }

        public IList<ProductPhoto> ListPhotos(int productID)
        {
            List<ProductPhoto> data = new List<ProductPhoto>();
            using (var connection = OpenConnection())
            {
                var sql = @"select * from ProductPhotos where ProductID = @ProductID order by DisplayOrder ASC";
                var parameters = new { productID = productID };
                data = connection.Query<ProductPhoto>(sql: sql, param: parameters, commandType: System.Data.CommandType.Text).ToList();
                connection.Close();
            }
            
            return data;
        }

        public bool Update(Product data)
        {
            bool result = false;
            using (var connection = OpenConnection())
            {
                var sql = @"if not exists(select * from Products where ProductID <> @ProductID and ProductName = @ProductName)
                                begin
                                    update Products 
                                    set ProductName = @ProductName,
                                        ProductDescription = @ProductDescription, 
                                        CategoryID = @CategoryID, 
                                        SupplierID = @SupplierID, 
                                        Unit = @Unit, 
                                        Price = @Price, 
                                        IsSelling = @IsSelling, 
                                        Photo = @Photo
                                    where ProductID = @ProductID
                                end";
                var parameters = new
                {
                    ProductID = data.ProductID,
                    ProductName = data.ProductName ?? "",
                    ProductDescription = data.ProductDescription ?? "",
                    CategoryID = data.CategoryID,
                    SupplierID = data.SupplierID,
                    Unit = data.Unit ?? "",
                    Price = data.Price,
                    IsSelling = data.IsSelling,
                    Photo = data.Photo ?? ""
                };
                result = connection.Execute(sql: sql, param: parameters, commandType: System.Data.CommandType.Text)>0;
                connection.Close();
            }
            return result;
        }

        public bool UpdateAttribute(ProductAttribute data)
        {
            bool result = false;
            using (var connection = OpenConnection())
            {
                var sql = @"update ProductAttributes 
                                        set AttributeName = @AttributeName, 
                                        AttributeValue = @AttributeValue, 
                                        DisplayOrder = @DisplayOrder
                                    where AttributeID = @AttributeID
                                ";
                var parameters = new
                {
                    AttributeID = data.AttributeID,
                    AttributeName = data.AttributeName ?? "",
                    AttributeValue = data.AttributeValue ?? "",
                    DisplayOrder = data.DisplayOrder
                };
                result = connection.Execute(sql: sql, param: parameters, commandType: System.Data.CommandType.Text) > 0;
                connection.Close();
            }
            return result;
        }

        public bool UpdatePhoto(ProductPhoto data)
        {
            bool result = false;
            using (var connection = OpenConnection())
            {
                var sql = @"update ProductPhotos
                                    set Description = @Description,
                                        DisplayOrder = @DisplayOrder,
                                        Photo = @Photo,
                                        IsHidden = @IsHidden
                                    where PhotoID = @PhotoID";
                var parameters = new
                {
                    Description = data.Description ?? "",
                    DisplayOrder = data.DisplayOrder,
                    Photo = data.Photo ?? "",
                    Ishidden = data.IsHidden,
                    PhotoID = data.PhotoID,
                };
                result = connection.Execute(sql: sql, param: parameters, commandType: System.Data.CommandType.Text) > 0;
                connection.Close();
            }
            return result;
        }
    }
}
