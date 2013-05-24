using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using System.Dynamic;
using System.Configuration;


namespace eNotaryWebRole.Controllers
{
    using System.Data.Objects.SqlClient;
    using eNotaryWebRole.Models;



    public class FilterTreeController : Controller
    {



        private string username = "";





        //
        // GET: /FilterTree/

        public ActionResult Index()
        {
            string model = "FilterTree/GetUnitsForFilterTree";


            return PartialView("Index", model);
        }

        [HttpPost]
        public ActionResult GetDataFilterTree()
        {

            List<JsTreeModel> list_nodes = new List<JsTreeModel>();
            JsTreeData data = new JsTreeData("Data crearii", "");
            JsTreeAttribute attr = new JsTreeAttribute("date", false, "", "date");
            JsTreeModel node = new JsTreeModel(data, "opened", "date", attr, null);
            list_nodes.Add(node);
            return this.Json(list_nodes, JsonRequestBehavior.AllowGet);

        }


    }
}