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
    
    public partial class EducationLevel
    {
        public EducationLevel()
        {
            this.PersonDetails = new HashSet<PersonDetail>();
        }
    
        public long ID { get; set; }
        public string EducationLevel1 { get; set; }
        public long Disabled { get; set; }
    
        public virtual ICollection<PersonDetail> PersonDetails { get; set; }
    }
}
