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
    
    public partial class PersonDetail
    {
        public PersonDetail()
        {
            this.Acts = new HashSet<Act>();
            this.Payments = new HashSet<Payment>();
            this.SignedActs = new HashSet<SignedAct>();
            this.Users = new HashSet<User>();
        }
    
        public long ID { get; set; }
        public string FirstName { get; set; }
        public string MiddleName { get; set; }
        public string LastName { get; set; }
        public System.DateTime Birthday { get; set; }
        public string Gender { get; set; }
        public string Nationality { get; set; }
        public Nullable<long> AddressID { get; set; }
        public string MobilePhoneNumber { get; set; }
        public string HomePhoneNumber { get; set; }
        public Nullable<long> JobTypeID { get; set; }
        public string JobPlace { get; set; }
        public Nullable<long> EducationLevelID { get; set; }
        public string Email { get; set; }
        public string FacebookID { get; set; }
        public string CommunicationMode { get; set; }
        public bool Disabled { get; set; }
        public System.DateTime CreateDate { get; set; }
        public long CreateContactID { get; set; }
        public Nullable<System.DateTime> EditDate { get; set; }
        public Nullable<long> EditContactID { get; set; }
    
        public virtual ICollection<Act> Acts { get; set; }
        public virtual Address Address { get; set; }
        public virtual EducationLevel EducationLevel { get; set; }
        public virtual JobType JobType { get; set; }
        public virtual ICollection<Payment> Payments { get; set; }
        public virtual ICollection<SignedAct> SignedActs { get; set; }
        public virtual ICollection<User> Users { get; set; }
    }
}
