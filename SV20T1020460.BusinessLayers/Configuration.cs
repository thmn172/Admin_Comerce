using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SV20T1020460.BusinessLayers
{
    public static class Configuration
    {
        ///<summary>
        ///Chuỗi kết thông số kết nối đến csdl
        ///</summary>
        public static string ConnectionString { get; private set; } = "";
        //<summary>
        //khởi tạo cấu hình BussinessLayer (hàm này phải được gọi trước khi ứng dụng chạy)
        //</sumary>
        //<param name = "connectionStrng" ></param>
        //<returns></returns>
        public static void Initialize(string connectionString) 
        { 
            Configuration.ConnectionString = connectionString;
        }
    }
}
