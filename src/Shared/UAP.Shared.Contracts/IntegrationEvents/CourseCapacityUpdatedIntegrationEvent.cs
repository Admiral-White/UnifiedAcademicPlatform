using System;

namespace UAP.Shared.Contracts.IntegrationEvents
{
    public class CourseCapacityUpdatedIntegrationEvent : BaseIntegrationEvent
    {
        public Guid CourseId { get; private set; }
        public string CourseCode { get; private set; }
        public int MaxCapacity { get; private set; }
        public int CurrentEnrollment { get; private set; }

        public CourseCapacityUpdatedIntegrationEvent(
            Guid courseId,
            string courseCode,
            int maxCapacity,
            int currentEnrollment,
            DateTime occurredOn) : base()
        {
            CourseId = courseId;
            CourseCode = courseCode;
            MaxCapacity = maxCapacity;
            CurrentEnrollment = currentEnrollment;
        }
    }
}