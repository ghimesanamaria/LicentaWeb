using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using eNotaryWebRole.ViewModel;

namespace eNotaryWebRole.ViewModel
{
    public class DivorceViewModel
    {

        public DivorcePersonDetailViewModel husband { get; set; }
        public DivorcePersonDetailViewModel wife { get; set; }
        public DivorceCommonDetailsViewModel common { get; set; }
       
    }
}