//------------------------------------------------------------------------------
// <auto-generated>
//     This code was generated from a template.
//
//     Manual changes to this file may cause unexpected behavior in your application.
//     Manual changes to this file will be overwritten if the code is regenerated.
// </auto-generated>
//------------------------------------------------------------------------------

namespace Rooms.Models
{
    using System;
    using System.Collections.Generic;
    
    public partial class TreatmentRoomMaster
    {
        [System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Usage", "CA2214:DoNotCallOverridableMethodsInConstructors")]
        public TreatmentRoomMaster()
        {
            this.TreatmentAvaileds = new HashSet<TreatmentAvailed>();
        }
    
        public int ID { get; set; }
        public string RoomName { get; set; }
        public Nullable<int> TreatmentGroupID { get; set; }
    
        [System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Usage", "CA2227:CollectionPropertiesShouldBeReadOnly")]
        public virtual ICollection<TreatmentAvailed> TreatmentAvaileds { get; set; }
        public virtual TreatmentRoomGroup TreatmentRoomGroup { get; set; }
    }
}
