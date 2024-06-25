using Microsoft.Data.SqlClient;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SV20T1020460.DataLayers.SQLServer
{
    //<summary>
    //Lớp cha cho các lớp cài đặt các phép xử lý dũ liệu trên SQL Server 
    //</summary>
    public abstract class _BaseDAL
    {
        protected string _connectionString = "";
        //<summary>
        //Ctor
        //</summary>
        //<param name="connectionString"></param>
        public _BaseDAL(string connectionString) 
        {
            _connectionString = connectionString;
        }
        //<summary>
        ///Tạo và mở để kết nối csdl
        //</summary>
        //<returns></returns>
        protected SqlConnection OpenConnection()
        {
            SqlConnection connection = new SqlConnection();
            connection.ConnectionString = _connectionString;
            connection.Open();
            return connection;
        }
    }
}
