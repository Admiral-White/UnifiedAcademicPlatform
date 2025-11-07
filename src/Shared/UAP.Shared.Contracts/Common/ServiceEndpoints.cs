using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

/// <summary>
/// Centralized configuration for service endpoints
/// Used by API Gateway and service discovery
/// </summary>
namespace UAP.Shared.Contracts.Common
{
    public static class ServiceEndpoints
    {
        public const string Authentication = "authentication-service";
        public const string CourseCatalog = "course-catalog-service";
        public const string StudentAcademic = "student-academic-service";
        public const string Registration = "registration-service";
        public const string Notification = "notification-service";
        public const string Reporting = "reporting-service";
    }
}
