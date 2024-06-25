using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SV20T1020460.DomainModels
{
    public class UserAccount
    {
        public string UserID { get; set; } = "";
        public string UserName { get; set; } = "";
        public string FullName { get; set; } = "";
        public string Email { get; set; } = "";
        public string Photo { get; set; } = "";
        public string PassWord { get; set; } = "";
        /// <summary>
        /// chuỗi các quyền tài khoản, phân cách bởi dấu phẩy
        /// </summary>
        public string RoleNames { get; set; } = "";
    }
}
