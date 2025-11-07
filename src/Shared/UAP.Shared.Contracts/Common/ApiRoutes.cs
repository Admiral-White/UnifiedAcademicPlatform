using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

/// <summary>
/// Centralized API route definitions
/// Ensures consistency across all services
/// </summary>
namespace UAP.Shared.Contracts.Common
{
    public static class ApiRoutes
    {
        private const string Root = "api";
        private const string Version = "v1";
        private const string Base = $"{Root}/{Version}";

        public static class Authentication
        {
            public const string Login = $"{Base}/auth/login";
            public const string Register = $"{Base}/auth/register";
            public const string Refresh = $"{Base}/auth/refresh";
            public const string Validate = $"{Base}/auth/validate";
        }

        public static class Courses
        {
            public const string GetAll = $"{Base}/courses";
            public const string GetById = $"{Base}/courses/{{id}}";
            public const string Create = $"{Base}/courses";
            public const string Update = $"{Base}/courses/{{id}}";
            public const string Search = $"{Base}/courses/search";
        }
    }
}
