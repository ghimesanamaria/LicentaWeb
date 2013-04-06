using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using eNotaryWebRole.Models;

namespace eNotaryWebRole.Models
{
    public class DataAccessRepository:IDataAccessRepository
    {
        private static eNotaryDBEFEntities _db = new eNotaryDBEFEntities();

        public  string getRole(string username)
        {
            string roleName = "";
            try
            {
                var role = (from r in _db.Users.Where(o => o.Username == username)
                            join ur in _db.UserRoles
                            on r.RoleID equals ur.ID
                            select ur.RoleName).FirstOrDefault();
                roleName = role;
            }
            catch (Exception ex)
            {
                roleName = "admin";
            }
           
                return roleName;
            
        }
    }
}