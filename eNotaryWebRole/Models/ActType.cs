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
    
    public partial class ActType
    {
        public ActType()
        {
            this.Acts = new HashSet<Act>();
        }
    
        public long ID { get; set; }
        public string ActTypeName { get; set; }
        public bool Disabled { get; set; }
    
        public virtual ICollection<Act> Acts { get; set; }
    }
}
