using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace eNotaryWebRole.ViewModel
{
    public class DivorceCommonDetailsViewModel
    {
        public DateTime marriage_date { get; set; }
        public string downhall_city { get; set; }
        public string downhall_county { get; set; }
        public string marriage_certificate_serie { get; set; }
        public string marriage_certificate_no { get; set; }
        public string common_city { get; set; }
        public string common_street { get; set; }
        public string common_street_no { get; set; }
        public string common_street_bl { get; set; }
        public string common_bl { get; set; }
        public string common_et { get; set; }
        public string common_ap { get; set; }
        public string common_ex_husband_name { get; set; }
        public string common_ex_wife_name { get; set; }
    }
}