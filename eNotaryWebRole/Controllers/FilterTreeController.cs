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

        private eNotaryDBEntities1 _db = new eNotaryDBEntities1();

        private string username = "";





        //
        // GET: /FilterTree/

        public ActionResult Index()
        {
            string model = "FilterTree/GetUnitsForFilterTree";


            return PartialView("Index", model);
        }


        public JsTreeModel getNode(string data_title, string data_icon, string attr_id, bool attr_selected, string attr_style, string attr_description, string node_state, List<JsTreeModel> children)
        {

            JsTreeData dataObj = new JsTreeData(data_title, data_icon);
            JsTreeAttribute attrOBj = new JsTreeAttribute(attr_id, attr_selected, attr_style, attr_description);
            JsTreeModel objNode = new JsTreeModel(dataObj, node_state, attr_id, attrOBj, children);


            return objNode;
        }

        [HttpPost]
        public ActionResult GetDataFilterTree()
        {

            List<object> list_nodes = new List<object>();

            //JsTreeModel node = getNode("Data crearii", "", "date", false, "", "date", "", null);
            //list_nodes.Add(node);

            //node = getNode("Doar documentele semnate", "", "signed", false, "", "signed", "", null);
            //list_nodes.Add(node);

            //node = getNode("Doar documentele nesemnate", "", "unsigned", false, "", "unsigned", "", null);
            //list_nodes.Add(node);

            //node = getNode("Ordonare document initial-document nesemnat", "", "orderd", false, "", "ordered", "", null);
            //list_nodes.Add(node);
            var list = (from at in _db.ActTypes
                           select
                                  new
                                  {

                                      data = new { title = at.ActTypeName, icon = ""},
                                      id = at.ActTypeName ,
                                      attr = new { id = at.ActTypeName , description = at.ActTypeName },
                                      state = "opened"

                                  }).Distinct().ToList();
            var union_list = list.Union(list_nodes);

           
            return this.Json(union_list, JsonRequestBehavior.AllowGet);

        }


    }
}