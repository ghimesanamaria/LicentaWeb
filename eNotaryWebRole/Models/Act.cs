//------------------------------------------------------------------------------
// <auto-generated>
//    This code was generated from a template.
//
//    Manual changes to this file may cause unexpected behavior in your application.
//    Manual changes to this file will be overwritten if the code is regenerated.
// </auto-generated>
//------------------------------------------------------------------------------

namespace eNotaryWebRole.Models
{
    using System;
    using System.Collections.Generic;
    
    public partial class Act
    {
        public Act()
        {
            this.SignedActs = new HashSet<SignedAct>();
        }
    
        public long ID { get; set; }
        public long ActTypeID { get; set; }
        public long PersonDetailsID { get; set; }
        public string Name { get; set; }
        public System.DateTime CreationDate { get; set; }
        public long CreateContactID { get; set; }
        public string Reason { get; set; }
        public string State { get; set; }
        public string ReasonState { get; set; }
        public bool Signed { get; set; }
        public bool Disabled { get; set; }
        public string ExternalUniqueReference { get; set; }
        public string ExtraDetails { get; set; }
        public Nullable<System.DateTime> EditDate { get; set; }
        public Nullable<long> EditContactID { get; set; }
    
        public virtual ActType ActType { get; set; }
        public virtual PersonDetail PersonDetail { get; set; }
        public virtual ICollection<SignedAct> SignedActs { get; set; }
    }
}
