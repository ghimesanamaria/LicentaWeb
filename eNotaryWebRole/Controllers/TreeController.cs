using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using eNotaryWebRole.Models;
using System.Web.Script.Serialization;
using System.Data.SqlClient;
using System.Data.Objects.SqlClient;

namespace eNotaryWebRole.Controllers
{
    public class TreeController : Controller
    {

        private eNotaryDBEntities1 _db = new eNotaryDBEntities1();
        private IDataAccessRepository _rep = new DataAccessRepository();
        private string _username = "";
        //
        // GET: /Tree/

        public ActionResult Index()
        {
            _username = User.Identity.Name;
           
                string role = _rep.getRole(_username);
                var treemodel = GetTreeRootData();
                TreeNodesViewModel model = new TreeNodesViewModel();
                model.treeVariables = new JavaScriptSerializer().Serialize(treemodel.Data);
                ViewBag.Role = role;

                return PartialView("Index", model);
           
            
        }


        [HttpPost]
        public JsonResult GetTreeRootData()
        {

            _username = User.Identity.Name;

           // long userID = _db.Contacts.Where(o => o.Username == username).First().ID;



            List<JsTreeModel> tree = new List<JsTreeModel>();

            string url =Url.Content("~/Images/unsigned.png");
            JsTreeData data = new JsTreeData("Documente nesemnate", url);
            JsTreeAttribute attr = new JsTreeAttribute("-1", false, "", "UnsignedUnvDocs");
            JsTreeModel root_node = new JsTreeModel(data, "closed", "-1", attr, null);
            tree.Add(root_node);
             url = Url.Content("~/Images/Stamp-icon.png");
            data = new JsTreeData("Documente semnate", url);
            attr = new JsTreeAttribute("-2", false, "", "SignedDocs");
            root_node = new JsTreeModel(data, "closed", "-2", attr, null);
            tree.Add(root_node);



            url = Url.Content("~/Images/Document-Copy-icon.png");
             data = new JsTreeData("Documente", url);
            attr = new JsTreeAttribute("0", false, "", "root");
            root_node = new JsTreeModel(data, "open", "0", attr, tree);

            List<JsTreeModel> list = new List<JsTreeModel>();
            list.Add(root_node);
            return Json(list);
          

        }

        [HttpPost]
        public ActionResult GetTreeData(string id, string filter, int typeAct,string role)
        {
            

          
            //long id_person = long.Parse(id.Split('_')[0]);

            long id_person = 0;
             string url ;
             _username = User.Identity.Name;
           // change the interface to tree if the user is not admin or notar
             if (role != "angajat")
             {
                 id_person = _db.Users.Where(u => u.Username == _username).FirstOrDefault().ID;
                 typeAct = int.Parse(id);
                 id = "";
             }

            switch (id)
            {
                case "-1":
                    {
                        // we must find all people that has unsigned and unvisualized acts 
                        // we have to sort them depending on their role

                        // 1.
                        url =Url.Content("~/Images/Person.png");

                        var q = ((from a in _db.Acts.Where(o => o.Signed == false && o.State == "nevizualizat")
                                  join p in _db.PersonDetails
                                  on a.PersonDetailsID equals p.ID
                                  join at in _db.ActTypes 
                                  on a.ActTypeID equals at.ID
                                  where a.Disabled == false
                                  && (string.IsNullOrEmpty(filter)||(at.ActTypeName == filter))

                                  select
                                  new
                                  {

                                      data = new { title = p.LastName, icon = url},
                                      id = SqlFunctions.StringConvert((decimal?)p.ID).Trim() + "_unsignedUV",
                                      attr = new { id = SqlFunctions.StringConvert((decimal?)p.ID).Trim() + "_unsignedUV", description = "person" },
                                      state = "closed"

                                  })).Distinct().ToList();
                
                       
                        return Json(q);


                    } 
                    break;
                case "-2":
                    {
                        url = Url.Content("~/Images/Person.png");
                        var q2 = ( from  a in _db.SignedActs
                                   join p in _db.PersonDetails
                                  on a.CreatePersonID equals p.ID
                                  join sa in _db.Acts
                                  on a.ActID equals sa.ID
                                  join at in _db.ActTypes
                                  on sa.ActTypeID equals at.ID
                                  
                                  where  (string.IsNullOrEmpty(filter)||(at.ActTypeName == filter))
                                 
                                   select
                                   new
                                   {

                                       data = new {title=p.LastName, icon=url},
                                       id = SqlFunctions.StringConvert((decimal?)p.ID).Trim() + "_signed",
                                       attr = new { id = SqlFunctions.StringConvert((decimal?)p.ID).Trim() + "_signed", description = "person" },
                                       state = "closed"

                                   }).Distinct();


                        return Json(q2);
                    }
                    break;
          
                default:
                    {


                        // must verify what type of act must be  displayed - signed, unsigned or visualized but unsigned
                        if (typeAct == -1)
                        {

                            url = Url.Content("~/Images/normal.png");
                            var q4 = (from a in _db.Acts.Where(o => o.Signed == false && o.State == "nevizualizat" && o.Disabled == false)
                                      join at in _db.ActTypes
                                      on a.ActTypeID equals at.ID
                                      where   (string.IsNullOrEmpty(filter)||(at.ActTypeName == filter))
                                      && a.PersonDetailsID == id_person
                                      select
                                      new
                                      {
                                          data = new { title = a.ExternalUniqueReference, icon = url },
                                          id = SqlFunctions.StringConvert((decimal?)a.ID).Trim() + "_unsignedUVAct",
                                          attr = new { id = SqlFunctions.StringConvert((decimal?)a.ID).Trim() + "_unsignedUVAct", description = "act" },
                                          state = String.Empty
                                      }).Distinct();

                            return Json(q4);
                        }
                        else
                            if (typeAct == -2)
                            {
                                url = Url.Content("~/Images/cert.png");
                                var q4 = (from a in _db.SignedActs
                                          join sa in _db.Acts
                                          on a.ActID equals sa.ID
                                          join at in _db.ActTypes
                                          on sa.ActTypeID equals at.ID
                                           where (string.IsNullOrEmpty(filter) || (at.ActTypeName == filter))
                                           && sa.PersonDetailsID == id_person
                                          select
                                          new
                                          {
                                              data = new { title = a.ExternalUniqueReference, icon = url },
                                              id = SqlFunctions.StringConvert((decimal?)a.ID).Trim() + "_signedAct",
                                              attr = new { id = SqlFunctions.StringConvert((decimal?)a.ID).Trim() + "_signedAct", description = "act" },
                                              state = String.Empty
                                          });

                                return Json(q4);

                            }
                
                    }
                    break;
            }


          

           


             return this.Json("");
            
        }
        [HttpPost]
        public ActionResult SearchTreeData(string id)
        {
            // search in tree only after name documents 
            // id represents search string

            // first search in usigned data
            var ua = from a in _db.Acts
                    where a.ExternalUniqueReference.Contains(id)
                    select a.PersonDetailsID;
            // second search in signed data
            var sa = from s in _db.SignedActs
                     join a in _db.Acts
                     on s.ActID equals a.ID
                     where s.ExternalUniqueReference.Contains(id)
                     select a.PersonDetailsID;
            IList<string> found = new List<string>();

            found.Add("#-1");
            found.Add("#-2");

            foreach (long pd in ua)
            {
                found.Add("#"+pd.ToString());
            }
            foreach(long pd in sa){
                  found.Add("#"+pd.ToString());
            }

             var f = found.ToArray();            


            return Json(f);

        }

       

    }
}
