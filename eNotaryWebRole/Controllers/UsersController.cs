using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;

using System.Web.Security;
using System.IO;
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
        private eNotaryDBEntities1 _db = new eNotaryDBEntities1();
        private IDataAccessRepository _repository = new DataAccessRepository();
        string username = "";

        public ActionResult Index()
        {
            var url = HttpContext.Request.PhysicalApplicationPath;
            // delete all temporary files

            Array.ForEach(Directory.GetFiles(url + "\\Fisiere"),
             delegate(string path)
             {
                 System.IO.File.Delete(path);
             });

            // delete all temporary files

            Array.ForEach(Directory.GetFiles(url + "\\PDFApplications"),
             delegate(string path)
             {
                 System.IO.File.Delete(path);
             });
            Array.ForEach(Directory.GetFiles(url + "\\Content\\pdf_preview"),
            delegate(string path)
            {
                System.IO.File.Delete(path);
            });
            var role_list = from ur in _db.UserRoles
                            select new {
                                ID= ur.ID,
                                Name = ur.RoleName
                            };
            ViewBag.RoleList = new SelectList(role_list, "ID", "Name", 0);
            // verify security points 

            username = User.Identity.Name;
            long us = (from s in _db.SecurityPoints
                       join rs in _db.RoleSecurityPoints
                       on s.ID equals rs.SecurityPointID
                       join u in _db.Users.Where(u => u.Username == username)
                       on rs.RoleID equals u.RoleID
                       where s.Name == "vizualizare utilizatori"
                       select rs.State).FirstOrDefault();
            // verify if per user is set this security point
            long us_us = (from u in _db.Users.Where(u => u.Username == username)
                          join r in _db.RoleSecurityPoints
                          on u.ID equals r.UserID
                          join s in _db.SecurityPoints
                          on r.SecurityPointID equals s.ID
                          where s.Name == "vizualizare utilizatori"
                          select r.State).FirstOrDefault();

            if (us_us == 1)
            {
                us = us_us;
            }
            if (us == 0)
            {
                return RedirectToAction("Index", "Home");
            }

            ViewBag.ViewUsers = us;

            long doc = (from s in _db.SecurityPoints
                        join rs in _db.RoleSecurityPoints
                        on s.ID equals rs.RoleID
                        join u in _db.Users.Where(u => u.Username == username)
                        on rs.RoleID equals u.RoleID
                        where s.Name == "vizualizare documente"
                        select rs.State).FirstOrDefault();
            long doc_doc = (from u in _db.Users.Where(u => u.Username == username)
                            join r in _db.RoleSecurityPoints
                            on u.ID equals r.UserID
                            join s in _db.SecurityPoints
                            on r.SecurityPointID equals s.ID
                            where s.Name == "vizualizare documente"
                            select r.State).FirstOrDefault();
            if (doc_doc == 1)
            {
                doc = doc_doc;
            }
           
            ViewBag.ViewDocuments = doc;



            long edu = (from s in _db.SecurityPoints
                        join rs in _db.RoleSecurityPoints
                        on s.ID equals rs.SecurityPointID
                        join u in _db.Users.Where(u => u.Username == username)
                        on rs.RoleID equals u.RoleID
                        where s.Name == "editare utilizatori"
                        select rs.State).FirstOrDefault();
            long edu_edu = (from u in _db.Users.Where(u => u.Username == username)
                            join r in _db.RoleSecurityPoints
                            on u.ID equals r.UserID
                            join s in _db.SecurityPoints
                            on r.SecurityPointID equals s.ID
                            where s.Name == "editare utilizatori"
                            select r.State).FirstOrDefault();
            if (edu_edu == 1)
            {
                edu = edu_edu;
            }
            ViewBag.EditUsers = edu.ToString();

            long delu = (from s in _db.SecurityPoints
                        join rs in _db.RoleSecurityPoints
                        on s.ID equals rs.SecurityPointID
                        join u in _db.Users.Where(u => u.Username == username)
                        on rs.RoleID equals u.RoleID
                        where s.Name == "editare utilizatori"
                        select rs.State).FirstOrDefault();
            long delu_delu = (from u in _db.Users.Where(u => u.Username == username)
                            join r in _db.RoleSecurityPoints
                            on u.ID equals r.UserID
                            join s in _db.SecurityPoints
                            on r.SecurityPointID equals s.ID
                            where s.Name == "editare utilizatori"
                            select r.State).FirstOrDefault();
            if (delu_delu == 1)
            {
                delu = delu_delu;
            }
            ViewBag.DeleteUsers = delu.ToString();



            long edr = (from s in _db.SecurityPoints
                        join rs in _db.RoleSecurityPoints
                        on s.ID equals rs.SecurityPointID
                        join u in _db.Users.Where(u => u.Username == username)
                        on rs.RoleID equals u.RoleID
                        where s.Name == "editare roluri"
                        select rs.State).FirstOrDefault();
            long edr_edr = (from u in _db.Users.Where(u => u.Username == username)
                            join r in _db.RoleSecurityPoints
                            on u.ID equals r.UserID
                            join s in _db.SecurityPoints
                            on r.SecurityPointID equals s.ID
                            where s.Name == "editare roluri"
                            select r.State).FirstOrDefault();
            if (edr_edr == 1)
            {
                edr = edr_edr;
            }
            ViewBag.EditRoles = edr.ToString();


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


        public ActionResult GroupRoles(long type)
        {
            if (type == 0)
            {

                ViewBag.Action = "";
            }
            else
            {
                ViewBag.Action = "disabled";
            }

         

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

            // only for this user
            RoleSecurityPoint list_user = (from u in _db.Users.Where(o => o.Username == username)
                              join rs in _db.RoleSecurityPoints
                              on u.ID equals rs.UserID
                              join s in _db.SecurityPoints
                              on rs.SecurityPointID equals s.ID
                              where s.Name == key && rs.Disabled == false && s.Disabled == false && u.Disabled == false
                              select rs).FirstOrDefault();

            if (list_user != null)
            {
                return list_user.State;
            }
            // default on user type
         
            RoleSecurityPoint list_role  = (from u in _db.Users.Where(o => o.Username == username)
                       join rs in _db.RoleSecurityPoints
                       
                       on u.RoleID equals rs.RoleID
                       join s in _db.SecurityPoints
                       on rs.SecurityPointID equals s.ID
                       where s.Name == key && u.Disabled ==false && s.Disabled == false && rs.Disabled == false
                       select rs).FirstOrDefault();
            if (list_role!= null)
            {
                return list_role.State;
            }

            ViewBag.RoleID = _db.Users.Where(x => x.Username == username).FirstOrDefault().RoleID;
            

            return 0;
        }

        public ActionResult UsersSecurityPoints()
        {
            username = User.Identity.Name;
            List<string> get_all_security_point = new List<string>();
            get_all_security_point = _db.SecurityPoints.Where(x=>x.Disabled == false).Select(x => x.Name).ToList();          

            Dictionary<string, long> sec_access = new Dictionary<string, long>();
            foreach (var sec in get_all_security_point)
            {
                long value = find_sec_point_user(sec);
                sec_access.Add(sec, value);
            }
            ViewBag.RoleID = _db.Users.Where(x => x.Username == username).FirstOrDefault().RoleID;

            return PartialView("UsersSecurityPoints",sec_access);
                 
        }

        public void SaveGroupSecurityPoint(long roleID, string ids)
        {
            string[] id_list = ids.Split(',');
            foreach (string id in id_list)
            {
                
                RoleSecurityPoint role = (
                    from rs in _db.RoleSecurityPoints.Where(r => r.RoleID == roleID)
                    join s in _db.SecurityPoints
                    on rs.SecurityPointID equals s.ID
                    where s.Name == id
                    select rs
                    ).FirstOrDefault();
                if (role != null)
                {
                    role.EditDate = DateTime.Now;
                    role.EditID = _db.Users.Where(u => u.Username == username).FirstOrDefault().ID;
                    role.State = 1;
                    role.Disabled = false;
                    role.RoleID = roleID;
                    _db.SaveChanges();
                }
                else
                {
                    if (_db.SecurityPoints.Where(x => x.Name == id).Count()> 0)
                    {
                        role = new RoleSecurityPoint()
                        {
                            CreateDate = DateTime.Now,
                            CreateID = _db.Users.Where(u => u.Username == username).FirstOrDefault().ID,
                            Disabled = false,
                            SecurityPointID = _db.SecurityPoints.Where(x => x.Name == id).FirstOrDefault().ID,
                            State = 1,
                            RoleID = roleID
                        };
                        _db.RoleSecurityPoints.Add(role);
                        _db.SaveChanges();
                    }

                }
            }

            // foreach security point in db for that group set state 0
            (from r in _db.RoleSecurityPoints.Where(x => x.RoleID == roleID)
             join s in _db.SecurityPoints
             on r.SecurityPointID equals s.ID
             where !id_list.Contains(s.Name)
             select r).ToList().ForEach(x => x.State = 0);
            _db.SaveChanges();
        }
        public void SaveUserSecurityPoint(long id_role,string ids)
        {
            username = User.Identity.Name;
            User user = (from u in _db.Users.Where(u => u.Username == username) select u).FirstOrDefault();
            user.RoleID = id_role;
            _db.SaveChanges();

            long roleID = (from u in _db.Users
                           select u.RoleID).FirstOrDefault();
            long userID = _db.Users.Where(u => u.Username == username).FirstOrDefault().ID;
            List<string> id_list = ids.Split(',').ToList();
            foreach (string id in id_list)
            {
                RoleSecurityPoint role = (
                    from rs in _db.RoleSecurityPoints.Where(r => r.UserID == userID)
                    join s in _db.SecurityPoints
                    on rs.SecurityPointID equals s.ID
                    where s.Name == id
                    select rs
                    ).FirstOrDefault();
                if (role != null)
                {
                    role.EditDate = DateTime.Now;
                    role.EditID = _db.Users.Where(u => u.Username == username).FirstOrDefault().ID;
                    role.State = 1;
                    role.Disabled = false;
                    role.UserID = userID;
                    _db.SaveChanges();
                }
                else
                {

                    role = (
                    from rs in _db.RoleSecurityPoints.Where(r => r.RoleID == roleID)
                    join s in _db.SecurityPoints
                    on rs.SecurityPointID equals s.ID
                    where s.Name == id
                    select rs
                    ).FirstOrDefault();
                    if (role == null)
                    {
                        role = new RoleSecurityPoint()
                        {
                            CreateDate = DateTime.Now,
                            CreateID = _db.Users.Where(u => u.Username == username).FirstOrDefault().ID,
                            Disabled = false,
                            SecurityPointID = _db.SecurityPoints.Where(x => x.Name == id).FirstOrDefault().ID,
                            State = 1,
                            UserID = userID
                        };
                        _db.RoleSecurityPoints.Add(role);
                    }

                    _db.SaveChanges();

                }
            }

            // foreach security point in db for that user set state 0
            (from r in _db.RoleSecurityPoints.Where(x => x.UserID == userID)
             join s in _db.SecurityPoints
             on r.SecurityPointID equals s.ID
             where !id_list.Contains(s.Name)
             select r).ToList().ForEach(x => x.State = 0);
            _db.SaveChanges();


            //if a default security point is deselected , add in db a record to say that
            List<string> list_default = (from rs in _db.RoleSecurityPoints
                                         join s in _db.SecurityPoints
                                         on rs.SecurityPointID equals s.ID
                                         join u in _db.Users.Where(u => u.ID == userID)
                                         on rs.UserID equals u.ID
                                         select s.Name).ToList().Except(id_list).ToList();
            if (list_default.Count() > 0)
            {

                foreach (string id in list_default)
                {
                    RoleSecurityPoint role = (
                              from rs in _db.RoleSecurityPoints.Where(r => r.UserID == userID)
                              join s in _db.SecurityPoints
                              on rs.SecurityPointID equals s.ID
                              where s.Name == id
                              select rs
                              ).FirstOrDefault();
                    if (role == null)
                    {
                        role = new RoleSecurityPoint()
                        {
                            CreateDate = DateTime.Now,
                            CreateID = _db.Users.Where(u => u.Username == username).FirstOrDefault().ID,
                            Disabled = false,
                            SecurityPointID = _db.SecurityPoints.Where(x => x.Name == id).FirstOrDefault().ID,
                            State = 0,
                            UserID = userID
                        };
                        _db.RoleSecurityPoints.Add(role);
                        _db.SaveChanges();
                    }
                    else
                    {
                        role.EditDate = DateTime.Now;
                        role.EditID = userID;
                        role.State = 0;
                        _db.SaveChanges();
                    }
                }
            }
        }

        public ActionResult SaveSecurityPoint(long roleID, string ids, long type)
        {
            username = User.Identity.Name;
            try
            {
                // 0 means group 1 users
                if (type == 0)
                {
                    SaveGroupSecurityPoint(roleID, ids);

                }
                else
                {
                    
                    SaveUserSecurityPoint(roleID,ids);

                }
            }
            catch (Exception ex)
            {
                return Json("Punctele de securitate nu au fost salvate! Repetati actiune");
            }
            return Json("Salvarea s-a efectuat cu succes");
        }

    }
}
