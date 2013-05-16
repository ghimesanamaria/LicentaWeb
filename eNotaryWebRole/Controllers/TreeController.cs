using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using eNotaryWebRole.Models;
using System.Web.Script.Serialization;
using System.Data.SqlClient;

namespace eNotaryWebRole.Controllers
{
    public class TreeController : Controller
    {

        private eNotaryDBEFEntities _db = new eNotaryDBEFEntities();
        private IDataAccessRepository _rep = new DataAccessRepository();
        private string _username = "admin";
        //
        // GET: /Tree/

        public ActionResult Index()
        {

            

            string role = _rep.getRole(_username);
            var treemodel = GetTreeRootData();
            TreeNodesViewModel model = new TreeNodesViewModel();
            model.treeVariables = new JavaScriptSerializer().Serialize(treemodel.Data);

            return PartialView("Index", model);
        }


        [HttpPost]
        public JsonResult GetTreeRootData()
        {
            string username = "";

            if (!string.IsNullOrEmpty(User.Identity.Name))
            {
                string[] usernameSplit = User.Identity.Name.Split('\\');
                username = usernameSplit[usernameSplit.Count() - 1];

                //pt geoscart
                //username =User.Identity.Name;
            }
            else
            {
                username = "admin";
            }

           // long userID = _db.Contacts.Where(o => o.Username == username).First().ID;

            var obj1 = new
            {
                data = "Documente nesemnate si nevizualizate",
                id = -1,
                attr = new { id = -1, description = "UnsignedUnvDocs" },
                state = "closed"
            };

            var obj2 = new
            {
                data = "Documente semnate",
                id =-2,
                attr = new { id = -2, description = "SignedDocs" },
                state = "closed"
            };

            var obj3 = new
            {
                data = "Documente nesemnate, dar in asteptare",
                id = -3,
                attr = new { id = -3, description = "UnsignedWtDocs" },
                state = "closed"
            };
            List<object> list = new List<object>();
            list.Add(obj1);
            list.Add(obj2);
            list.Add(obj3);


            IList<JsTreeModel> tree = new List<JsTreeModel>();
            return Json(list);
          

        }

        [HttpPost]
        public ActionResult GetTreeData(string id, string modelID, string umID, string openedDeviceIDs, int isPlant, int typeAct)
        {
            string username="";

            if (!string.IsNullOrEmpty(User.Identity.Name))
            {
                string[] usernameSplit = User.Identity.Name.Split('\\');
                username = usernameSplit[usernameSplit.Count() - 1];

                //pt geoscart
                //username =User.Identity.Name;
            }
            else
            {
                username = "admin";
            }

            


            switch (id)
            {
                case "-1":
                    {
                        // we must find all people that has unsigned and unvisualized acts 
                        // we have to sort them depending on their role

                        // 1.

                        var q = ((from a in _db.Acts.Where(o => o.Signed == false && o.State == "nevizualizat")
                                 join p in _db.PersonDetails
                                 on a.PersonDetailsID equals p.ID
                                 where a.Disabled ==false

                                 select
                                 new
                                 {
                                     
                                     data = p.LastName,
                                     id = p.ID,
                                     attr = new { id = p.ID, description = "person" },
                                     state = "closed"

                                 })).ToList();
                        List<JsTreeModel> list_Person = new List<JsTreeModel>();
                        foreach (var v in q)
                        {
                            JsTreeData data = new JsTreeData(v.data, "");
                            JsTreeAttribute attr = new JsTreeAttribute(v.id + "_unsignedUV", false, "","person");
                            list_Person.Add(
                                new JsTreeModel(data,"closed","",attr,null)
                                );
                        }
                     

                       
                        return Json(list_Person);


                    } 
                    break;
                case "-2":
                    {
                        var q2 = ( from  a in _db.SignedActs
                                   join p in _db.PersonDetails
                                  on a.CreatePersonID equals p.ID

                                   select
                                   new
                                   {

                                       data = p.LastName,
                                       id = p.ID,
                                       attr = new { id = p.ID, description="person" },
                                       state = "closed"

                                   });


                        List<JsTreeModel> list_Person = new List<JsTreeModel>();
                        foreach (var v in q2)
                        {
                            JsTreeData data = new JsTreeData(v.data, "");
                            JsTreeAttribute attr = new JsTreeAttribute(v.id + "_signed", false, "", "person");
                            list_Person.Add(
                                new JsTreeModel(data, "closed", "", attr, null)
                                );
                        }
                        return Json(list_Person);
                    }
                    break;
                case "-3":
                    {
                        var q3 = (from a in _db.Acts.Where(o => o.Signed == false && o.State == "vizualizat")
                                  join p in _db.PersonDetails
                                 on a.PersonDetailsID equals p.ID

                                  select
                                  new
                                  {

                                      data = p.LastName,
                                      id = p.ID,
                                      attr = new { id = p.ID , description = "person" },
                                      state = "closed"

                                  });
                        List<JsTreeModel> list_Person = new List<JsTreeModel>();
                        foreach (var v in q3)
                        {
                            JsTreeData data = new JsTreeData(v.data, "");
                            JsTreeAttribute attr = new JsTreeAttribute(v.id + "_unsignedVI", false, "", "person");
                            list_Person.Add(
                                new JsTreeModel(data, "closed", "", attr, null)
                                );
                        }
                        return Json(list_Person);
                    }
                    break;
                default:
                    {
                        // must verify what type of act must be  displayed - signed, unsigned or visualized but unsigned
                        if (typeAct == -1)
                        {
                            var q4 = (from a in _db.Acts.Where(o => o.Signed == false && o.State == "nevizualizat")
                                      select
                                      new
                                      {
                                          data = a.ExternalUniqueReference,
                                          id = a.ID,
                                          attr = new { id = a.ID, description = "act" },
                                          state = String.Empty
                                      });


                            List<JsTreeModel> list_acts = new List<JsTreeModel>();
                            foreach (var v in q4)
                            {
                                JsTreeData data = new JsTreeData(v.data, "");
                                JsTreeAttribute attr = new JsTreeAttribute(v.id + "_unsignedUVAct", false, "", "act");
                                list_acts.Add(
                                    new JsTreeModel(data, "", "", attr, null)
                                    );
                            }
                            return Json(list_acts);
                        }
                        else
                            if (typeAct == -2)
                            {

                                var q4 = (from a in _db.SignedActs
                                          select
                                          new
                                          {
                                              data = a.ExternalUniqueReference,
                                              id = a.ID,
                                              attr = new { id = a.ID, description = "act" },
                                              state = String.Empty
                                          });

                                List<JsTreeModel> list_acts = new List<JsTreeModel>();
                                foreach (var v in q4)
                                {
                                    JsTreeData data = new JsTreeData(v.data, "");
                                    JsTreeAttribute attr = new JsTreeAttribute(v.id + "_unsignedVIAct", false, "", "act");
                                    list_acts.Add(
                                        new JsTreeModel(data, "", "", attr, null)
                                        );
                                }
                                return Json(list_acts);

                            }
                            else
                            {

                                var q5 = (from a in _db.Acts.Where(o => o.State == "vizualizat" && o.Signed == false)
                                          select
                                          new
                                          {
                                              data = a.ExternalUniqueReference,
                                              id = a.ID,
                                              attr = new { id = a.ID, description = "act" },
                                              state = String.Empty
                                          });
                                List<JsTreeModel> list_acts = new List<JsTreeModel>();
                                foreach (var v in q5)
                                {
                                    JsTreeData data = new JsTreeData(v.data, "");
                                    JsTreeAttribute attr = new JsTreeAttribute(v.id + "_signedAct", false, "", "act");
                                    list_acts.Add(
                                        new JsTreeModel(data, "", "", attr, null)
                                        );
                                }
                                return Json(list_acts);
                            }
                    }
                    break;
            }


          

           


             return this.Json("");
            
        }
        [HttpPost]
        public ActionResult SearchTreeData(string id)
        {
            return Json("");

        }

       

    }
}
