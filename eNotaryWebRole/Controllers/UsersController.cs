using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;

using System.Web.Security;

using System.Transactions;
using eNotaryWebRole.ViewModel;
using eNotaryWebRole.Models;
using System.Web.Script.Serialization;

namespace eNotaryWebRole.Controllers
{
    public class UsersController : Controller
    {
        //
        // GET: /Users/
        private eNotaryDBEFEntities _db = new eNotaryDBEFEntities();
        private IDataAccessRepository _repository = new DataAccessRepository();
        string username = "";

        public ActionResult Index()
        {
            var role_list = from ur in _db.UserRoles
                            select new {
                                ID= ur.ID,
                                Name = ur.RoleName
                            };
            ViewBag.RoleList = new SelectList(role_list, "ID", "Name", 0);
            return View();
        }


        public ActionResult List(bool _search, int rows, int page, string sidx, string sord, string FirstName = "",
           string LastName = "", string Email = "", string Telephone = "", string Company = "", string Role = "")
        {
            string username = User.Identity.Name;
            string role = _repository.getRole(username); ;
            int pageSize = rows;
            int pageIndex = Convert.ToInt32(page) - 1;
            string roleChoosed = "";



            if (!string.IsNullOrEmpty(Role) && Role!="0")
            {
                roleChoosed = (from r in _db.UserRoles
                               select new
                               {
                                   ID = r.ID,
                                   Name = r.RoleName
                               }).ToArray().Skip(int.Parse(Role) - 1).Take(1).First().Name;
            }

            int totalRecords = 0;
            int totalPages = 0;
            ;
          

          

            var query =( from c in _db.PersonDetails
                        join u in _db.Users
                        on c.ID equals u.PersonID                        
                       join r in _db.UserRoles
                       on u.RoleID equals r.ID
                       where c.Disabled == false
                       && u.Disabled == false
                        //search 
                        where (string.IsNullOrEmpty(FirstName) || c.FirstName.Contains(FirstName))
                             &&
                              (string.IsNullOrEmpty(LastName) || c.LastName.Contains(LastName))
                            &&
                              (string.IsNullOrEmpty(Email) || c.Email.Contains(Email))
                            &&
                              (string.IsNullOrEmpty(Telephone) || c.MobilePhoneNumber.Contains(Telephone))
                           
                            &&
                              (string.IsNullOrEmpty(roleChoosed) || r.RoleName.Contains(roleChoosed))
                           
                              && c.Disabled == false
                             

                        select new
                        {
                            c.ID,
                            c.FirstName,
                            c.LastName,
                            c.Email,
                            c.MobilePhoneNumber,
                            u.Username,
                            r.RoleName
                        }).ToList() ;


            totalRecords = query.Count;


            var res = query.OrderByDescending(o => o.FirstName).Skip(pageIndex * pageSize).Take(pageSize).ToArray();
            var jsonData =
                new
                {
                    totalPages = (totalRecords + rows - 1) / rows,
                    page = page,
                    records = totalRecords,
                    rows = (
                    from r in res
                    select new
                    {
                        r.ID,
                        cell = new[]
                        {
                            r.ID.ToString(), 
                           string.IsNullOrEmpty(r.FirstName)? "-" : r.FirstName,
                           string.IsNullOrEmpty(r.LastName)? "-": r.LastName,
                           string.IsNullOrEmpty(r.Email)? "-": r.Email,
                           string.IsNullOrEmpty(r.MobilePhoneNumber)?"-": r.MobilePhoneNumber,
                           string.IsNullOrEmpty(r.Username)?"-":r.Username,
                           string.IsNullOrEmpty(r.RoleName)? "-":r.RoleName
                        }

                    })

                };
            return Json(jsonData, JsonRequestBehavior.AllowGet);
        }

        
        public ActionResult Delete(long id)
        {
             try
            {
               
                PersonDetail person = _db.PersonDetails.Where(p => p.ID== id).FirstOrDefault();
                 person.Disabled = true;
                 _db.SaveChanges();
                 User user = _db.Users.Where(u=>u.PersonID == person.ID).FirstOrDefault();
                 user.Disabled = true;
                 _db.SaveChanges();
                
            }
            catch (Exception)
            {
                return Json("The user was not deleted!");
            }

             return Json("The user was deleted successfully!", JsonRequestBehavior.AllowGet);
        }


        public ActionResult GroupRoles()
        {


            return PartialView("GroupRoles");

        }
        [HttpGet]
        public JsonResult GetGroupType()
        {

            username = User.Identity.Name;

           long user_role_id = (from  u in _db.Users
                                join ur in _db.UserRoles
                                on u.RoleID equals ur.ID
                                select ur.ID
                ).FirstOrDefault();

            var group_type = from gt in _db.UserRoles
                           select new
                           {
                               id = gt.ID,
                               name = gt.RoleName
                           };
            return Json(group_type, JsonRequestBehavior.AllowGet);
        }

        long find_Sec(long id, string key)
        {
            long q = (from sp in _db.SecurityPoints
                     join rsp in _db.RoleSecurityPoints
                     on sp.ID equals rsp.SecurityPointID
                     where rsp.RoleID == id
                     && sp.Name == key
                    select
                          rsp.State
                     ).FirstOrDefault();

            return q;
        }

        [HttpPost]
        public ActionResult GetSecurityPointPerUser(long id)
        {
            Dictionary<string, long> security_point = new Dictionary<string, long>();
            List<string> get_all_security_point = new List<string>();
            get_all_security_point = _db.SecurityPoints.Select(x=>x.Name).ToList();

            
            foreach (var t in get_all_security_point)
            {

                long val = find_Sec(id, t);
                security_point.Add(t, val);
            }
            JavaScriptSerializer jsonserializer = new JavaScriptSerializer();
            string s_point = jsonserializer.Serialize(security_point);
          


            return Json(
                s_point
        );
        }

        long find_sec_point_user(string key)
        {
            // default on user type
            long list_role =0;
            list_role  = (from u in _db.Users.Where(o => o.Username == username)
                       join rs in _db.RoleSecurityPoints
                       
                       on u.RoleID equals rs.RoleID
                       join s in _db.SecurityPoints
                       on rs.SecurityPointID equals s.ID
                       where s.Name == key
                       select rs.State).FirstOrDefault();
            if (list_role!= 0)
            {
                return list_role;
            }
                // only for this user
                long list_user = (from u in _db.Users.Where(o => o.Username == username)
                                        join rs in _db.RoleSecurityPoints
                                        on u.ID equals rs.UserID
                                        join s in _db.SecurityPoints
                                        on rs.SecurityPointID equals s.ID
                                        where s.Name == key
                                        select rs.State).FirstOrDefault();
           

            

            return list_user;
        }

        public ActionResult UsersSecurityPoints()
        {
            username = User.Identity.Name;
            List<string> get_all_security_point = new List<string>();
            get_all_security_point = _db.SecurityPoints.Select(x => x.Name).ToList();          

            Dictionary<string, long> sec_access = new Dictionary<string, long>();
            foreach (var sec in get_all_security_point)
            {
                long value = find_sec_point_user(sec);
                sec_access.Add(sec, value);
            }

            return PartialView("UsersSecurityPoints",sec_access);
                 
        }

    }
}
