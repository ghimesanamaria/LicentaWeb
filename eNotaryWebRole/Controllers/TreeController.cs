using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using eNotaryWebRole.Models;
using System.Web.Script.Serialization;

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

        public ActionResult GetTreeData(long id, string modelID, string umID, string openedDeviceIDs, int isPlant)
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
            
            
            

          

           


             return this.Json("");
            
        }
        [HttpPost]
        public ActionResult SearchTreeData(string id)
        {
            return Json("");

        }

       

    }
}
