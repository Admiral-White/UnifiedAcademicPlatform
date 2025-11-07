using MassTransit;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace UAP.Shared.Contracts.IntegrationEvents
{
    /// <summary>
    /// Marker interface for integration events
    /// Integration events are used for communication between microservices
    /// </summary>
    public interface IIntegrationEvent : CorrelatedBy<Guid>
    {
        DateTime OccurredOn { get; }
        string EventType { get; }

    }
}
