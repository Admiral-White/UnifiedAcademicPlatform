namespace UAP.SharedKernel.Common
{
    /// <summary>
    /// Base class for all domain events
    /// Provides common properties for event tracking
    /// </summary>
    public abstract class DomainEvent : IDomainEvent
    {
        public Guid EventId { get; private set; }
        public DateTime OccurredOn { get; private set; }

        protected DomainEvent()
        {
            EventId = Guid.NewGuid();
            OccurredOn = DateTime.UtcNow;
        }
    }
}
