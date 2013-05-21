using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;

namespace eNotaryWebRole.Controllers
{
    public class HelperController : Controller
    {
        //
        // GET: /Helper/

        public ActionResult Index()
        {
            return View();
        }

        public ActionResult DivorcePersonDetails(string type)
        {
            return PartialView("DivorcePersonDetails", type);
        }

        public ActionResult CommonDetails()
        {
            return PartialView("CommonDetails");
        }

    }
}
