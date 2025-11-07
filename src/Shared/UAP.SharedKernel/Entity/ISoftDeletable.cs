using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace UAP.SharedKernel.Entity
{
    public interface ISoftDeletable
    {
        // The property used by the global query filter
        bool IsDeleted { get; set; }
    }
}
