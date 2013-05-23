using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace eNotaryWebRole.ViewModel
{
    public class DivorcePersonDetailViewModel
    {
        public string firstlast_name { get; set; }
        public string father_name { get; set; }
        public string mothere_name { get; set; }
        public string city { get; set; }
        public string county { get; set; }
        public string serieAct { get; set; }
        public string noAct { get; set; }
        public string SNS { get; set; } //CNP
        public string Address { get; set; }
        public DateTime BirthDay { get; set; }


        public string type { get; set; }
    }
}