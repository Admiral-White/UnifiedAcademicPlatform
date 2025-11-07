using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace UAP.Shared.Contracts.IntegrationEvents
{
    public abstract class BaseIntegrationEvent : IIntegrationEvent
    {
        public Guid CorrelationId { get; private set; }
        public DateTime OccurredOn { get; private set; }
        public string EventType => GetType().Name;

        protected BaseIntegrationEvent()
        {
            CorrelationId = Guid.NewGuid();
            OccurredOn = DateTime.UtcNow;
        }

        protected BaseIntegrationEvent(Guid correlationId)
        {
            CorrelationId = correlationId;
            OccurredOn = DateTime.UtcNow;
        }
    }
}
