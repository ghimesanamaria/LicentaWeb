using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace eNotaryWebRole.ViewModel
{
    public class SignatureDetailsViewModel
    {
        public string typeOfSignature { get; set; }
        public string issuerName { get; set; }
        public string timeStampDetails { get; set; }
        public string signatureDetails { get; set; }
        public string errorDoc { get; set; }
        public string reason { get; set; }
    }
}