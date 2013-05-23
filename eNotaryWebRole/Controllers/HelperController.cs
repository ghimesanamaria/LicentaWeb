using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using eNotaryWebRole.ViewModel;

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

        public ActionResult DivorcePersonDetails(string type, DivorcePersonDetailViewModel pd)
        {
            pd.type = type;
            
            return PartialView("DivorcePersonDetails",pd );
        }

        public ActionResult CommonDetails(DivorceCommonDetailsViewModel cd)
        {
            return PartialView("CommonDetails",cd);
        }

    }
}
