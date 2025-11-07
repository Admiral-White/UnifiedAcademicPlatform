using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace UAP.SharedKernel.Entity
{
    public interface IAuditable
    {
        // Required by the UpdateAuditFields method
        DateTime CreatedOn { get; set; }
        string? CreatedBy { get; set; } // Nullable based on your implementation need
        DateTime? ModifiedOn { get; set; }
        string? ModifiedBy { get; set; }
    }
}
