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
    
    public partial class UserRole
    {
        public UserRole()
        {
            this.Users = new HashSet<User>();
        }
    
        public long ID { get; set; }
        public string RoleName { get; set; }
        public string Description { get; set; }
        public System.DateTime CreationDate { get; set; }
        public long CreateContactID { get; set; }
        public Nullable<long> EditDate { get; set; }
        public Nullable<long> EditContactID { get; set; }
    
        public virtual ICollection<User> Users { get; set; }
    }
}
