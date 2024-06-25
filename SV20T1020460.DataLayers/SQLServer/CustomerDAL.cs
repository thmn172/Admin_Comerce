using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Microsoft.Data.SqlClient;
using Dapper;
using SV20T1020460.DomainModels;
using System.ComponentModel.DataAnnotations;
using System.Net;
using System.Numerics;
namespace SV20T1020460.DataLayers.SQLServer
{
    public class CustomerDAL : _BaseDAL, ICommonDAL<Customer>
    {
        public CustomerDAL(string connectionString) : base(connectionString)
        {
        }

        public int Add(Customer data)
        {
            int id = 0;
            using (var connection = OpenConnection())
            {
                var sql = @"if exists(select * from Customers where Email = @Email)
                                select -1
                            else
                                begin
                                    insert into Customers(CustomerName,ContactName,Province,Address,Phone,Email,IsLocked)
                                    values(@CustomerName,@ContactName,@Province,@Address,@Phone,@Email,@IsLocked);
                                    select @@identity;
                                end";
                var parameters = new
                {
                    // ký tự : ?? "" nghĩa là khi null sẽ để giá trị rỗng
                    Email = data.Email ?? "",
                    CustomerName = data.CustomerName ?? "",
                    ContactName = data.ContactName ?? "",
                    Province = data.Province ?? "",
                    Phone = data.Phone ?? "",
                    Address = data.Address ?? "",
                    IsLocked = data.Islocked
                };
                id = connection.ExecuteScalar<int>(sql: sql, param: parameters, commandType: System.Data.CommandType.Text);
                //thực thi câu lệnh
                connection.Close();
                //đóng kết nối sau khi thực thi
            }
            return id;
        }

        public int Count(string searchValue = "")
        {
            int count = 0;
            if (!string.IsNullOrEmpty(searchValue))
                searchValue = "%" + searchValue + "%";
            using (var connection = OpenConnection())
            {
                var sql = @"select count(*) from Customers 
                            where (@searchValue = N'') or (CustomerName like @searchValue)";
                var parameters = new
                {
                    searchValue = searchValue ?? "",
                };
                count = connection.ExecuteScalar<int>(sql: sql, param: parameters, commandType: System.Data.CommandType.Text);
                connection.Close();
            }
            return count;
        }

        public bool Delete(int id)
        {
            bool result = false;
            using (var connection = OpenConnection())
            {
                var sql = @"delete from Customers where CustomerID = @CustomerID";
                var parameters = new
                {
                    CustomerID = id
                };
                result = connection.Execute(sql: sql, param: parameters, commandType: System.Data.CommandType.Text) > 0;
                connection.Close();
            }
            return result;

        }

        public Customer? Get(int id)
        {
            Customer? data = null;
            using (var connection = OpenConnection())
            {
                var sql = @"select * from Customers where CustomerID = @CustomerID";
                var parameters = new
                {
                    CustomerID = id
                };
                data = connection.QueryFirstOrDefault<Customer>(sql: sql, param: parameters, commandType: System.Data.CommandType.Text);
                // lấy 1 dòng (lấy về 1 bảng ghi) sử dụng QueryFirstOrDefault 
                connection.Close();
            }
            return data;
        }

        public bool IsUsed(int id)
        {
            bool result = false;
            using (var connection = OpenConnection())
            {
                var sql = @"if exists(select * from Orders where CustomerID = @CustomerID)
                                select 1
                            else 
                                select 0";
                var parameters = new
                {
                    CustomerID = id
                };
                result = connection.ExecuteScalar<bool>(sql: sql, param: parameters, commandType: System.Data.CommandType.Text);
                connection.Close() ;
            }
            return result;
        }

        public IList<Customer> List(int page = 1, int pageSize = 0, string searchValue = "")
        {
            List<Customer> data = new List<Customer>();
            if (!string.IsNullOrEmpty(searchValue))
                searchValue = "%" + searchValue + "%";
            using (var connection = OpenConnection())
            {
                var sql = @"with cte as
                            (
	                            select	*, row_number() over (order by CustomerName) as RowNumber
	                            from	Customers 
	                            where	(@searchValue = N'') or (CustomerName like @searchValue)
                            )
                            select * from cte
                            where  (@pageSize = 0) 
	                            or (RowNumber between (@page - 1) * @pageSize + 1 and @page * @pageSize)
                            order by RowNumber";
                var parameters = new
                {
                    page = page,
                    pageSize = pageSize,
                    searchValue = searchValue ?? ""
                };
                data = connection.Query<Customer>(sql: sql, param: parameters, commandType: System.Data.CommandType.Text).ToList();
                connection.Close();
            }
            return data;
        }

        public bool Update(Customer data)
        {
            bool result = false;
            using (var connection = OpenConnection())
            {
                var sql = @"if not exists(select * from Customers where CustomerID <> @CustomerID and Email = @email)
                                begin
                                    update Customers 
                                    set CustomerName = @customerName,
                                        ContactName = @contactName,
                                        Province = @province,
                                        Address = @address,
                                        Phone = @phone,
                                        Email = @email,
                                        IsLocked = @isLocked
                                    where CustomerID = @CustomerID
                                end";
                //if not exists(select * from Customers where CustomerID <> @CustomerID and Email = @email)
                //=> kiểm tra email chúng ta cần đổi có trùng với khách hàng nào trong csdl ko nếu ko trùng thì mới thực hiện update
                var parameters = new
                {
                    CustomerID = data.CustomerId,
                    Email = data.Email ?? "",
                    CustomerName = data.CustomerName ?? "",
                    ContactName = data.ContactName ?? "",
                    Province = data.Province ?? "",
                    Phone = data.Phone ?? "",
                    Address = data.Address ?? "",
                    IsLocked = data.Islocked
                };
                result = connection.Execute(sql:sql, param: parameters, commandType: System.Data.CommandType.Text ) > 0;
                connection.Close();
            }
            return result;
        }
    }
    //Ctrl + m + o : thu gọn code
    //Ctrl + m + l : mở code lại
}
