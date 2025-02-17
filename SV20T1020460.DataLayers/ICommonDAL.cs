﻿using System;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SV20T1020460.DataLayers
{
    //mô tả các phép xử lý dữ liệu chung
    public interface ICommonDAL<T>where T : class
    {
        //<summary>
        //Timf kiếm và lấy danh sách dữ liệu dưới dạng phân trang
        //</sumary>
        //<param name="page">Trang cần hiển thị</param>
        //<param name = "pageSize" > Số dòng hiển thị trên mỗi trang(bằng 0 nếu không phân trang dữ liệu)</param>
        //<param name = "searchValue" > Giá trị cần tìm kiếm (Chuỗi rỗng nếu lấy toàn bộ dữ liệu )</param>
        //<returns></returns>
        IList<T> List(int page = 1, int pageSize = 0, string searchValue = "");
        //<summary>
        //đếm số dòng dữ liệu tìm được
        //</sumary>
        //<param name = "searchValue" >Giá trị cần tìm kiếm(chuoxi rỗng nếu lấy toàn bộ dữ liệu)</param>
        //<returns></returns>
        int Count(string searchValue = "");
        //<summary>
        //Bổ sung dữ liệu vào csdl. Hàm trả về ID của dữ liệu được bổ sung
        //(Trả về giá trị 0 nếu việc bổ sung không thành công)
        //</sumary>
        //<param name = "data" ></param>
        //<returns></returns>
        int Add(T data);
        bool Update(T data);
        bool Delete(int id);
        //Lấy 1 bản ghi dựa vào id(trả về null nếu không tồn tại)
        T? Get(int id);
        //kiểm tra xem bản ghi dữ liệu có mã id hiện có đang được sử dụng
        bool IsUsed(int id);
    }
}
