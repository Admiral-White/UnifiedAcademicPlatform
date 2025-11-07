using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace UAP.SharedKernel.Common
{
    /// <summary>
    /// Marker interface for domain events
    /// Domain events represent something that happened in the domain that domain experts care about
    /// </summary>
    public interface IDomainEvent
    {
        DateTime OccurredOn { get; }
        Guid EventId { get; }

    }
}
