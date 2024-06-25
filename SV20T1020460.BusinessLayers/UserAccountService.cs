using Microsoft.Identity.Client;
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
    public static class UserAccountService
    {
        private static readonly IUserAccountDAL EmployeeAccountDB;
        static UserAccountService()
        {
            string connectionString = Configuration.ConnectionString;
            EmployeeAccountDB = new EmployeeAccountDAL(connectionString);
        }
        public static UserAccount? Authorize(string userName, string password)
        {
            return EmployeeAccountDB.Authorize(userName, password);
            //return null;
        }
        public static bool ChangePassword(string userName, string oldPassword, string newPassword)
        {
            return EmployeeAccountDB.ChangePassword(userName, oldPassword, newPassword);    
        }
    }
}
