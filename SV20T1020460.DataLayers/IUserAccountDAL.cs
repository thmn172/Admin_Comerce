using SV20T1020460.DomainModels;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SV20T1020460.DataLayers
{
    public interface IUserAccountDAL
    {
        /// <summary>
        /// xác thwucj tài khoản đăng nhập của người dùng
        /// hàm trả về thông tin tài khoản nếu xác thực thành công
        /// ngược lại thì trả về NULL
        /// </summary>
        /// <param name="userName"></param>
        /// <param name="password"></param>
        /// <returns></returns>
        UserAccount? Authorize(string userName, string password);
        
        /// <summary>
        /// đổi mật khẩu
        /// </summary>
        /// <param name="userName"></param>
        /// <param name="oldPassword"></param>
        /// <param name="newpassword"></param>
        /// <returns></returns>
        bool ChangePassword(string userName, string oldPassword, string newpassword);
    }
}
