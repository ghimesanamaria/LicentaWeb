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



            List<JsTreeModel> tree = new List<JsTreeModel>();
          
           
            JsTreeData data = new JsTreeData("Documente nesemnate si nevizualizate", "");
            JsTreeAttribute attr = new JsTreeAttribute("-1", false, "", "UnsignedUnvDocs");
            JsTreeModel root_node = new JsTreeModel(data, "closed", "-1", attr, null);
            tree.Add(root_node);
            data = new JsTreeData("Documente semnate", "");
            attr = new JsTreeAttribute("-2", false, "", "SignedDocs");
            root_node = new JsTreeModel(data, "closed", "-2", attr, null);
            tree.Add(root_node);

            
            
            string url = Url.Content("~/Images/User-Files-icon.png");
             data = new JsTreeData("Documente", url);
            attr = new JsTreeAttribute("0", false, "", "root");
            root_node = new JsTreeModel(data, "open", "0", attr, tree);

            List<JsTreeModel> list = new List<JsTreeModel>();
            list.Add(root_node);
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
                                     id = SqlFunctions.StringConvert((decimal?)p.ID).Trim() + "_unsignedUV",
                                     attr = new { id = SqlFunctions.StringConvert((decimal?) p.ID).Trim()+"_unsignedUV", description = "person" },
                                     state = "closed"

                                 })).Distinct().ToList();
                
                       
                        return Json(q);


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
                            var q4 = (from a in _db.Acts.Where(o => o.Signed == false && o.State == "nevizualizat" && o.Disabled == false)
                                      select
                                      new
                                      {
                                          data = a.ExternalUniqueReference,
                                          id = SqlFunctions.StringConvert((decimal?)a.ID).Trim() + "_unsignedUVAct",
                                          attr = new { id = SqlFunctions.StringConvert((decimal?)a.ID).Trim() + "_unsignedUVAct", description = "act" },
                                          state = String.Empty
                                      }).Distinct();

                            return Json(q4);
                        }
                        else
                            if (typeAct == -2)
                            {

                                var q4 = (from a in _db.SignedActs
                                          select
                                          new
                                          {
                                              data = a.ExternalUniqueReference,
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
            return Json("");

        }

       

    }
}
