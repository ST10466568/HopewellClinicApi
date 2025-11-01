using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Filters;
using System.Security.Claims;

namespace HopewellClinicApi.Attributes
{
    [AttributeUsage(AttributeTargets.Class | AttributeTargets.Method, AllowMultiple = true)]
    public class AuthorizeRoleAttribute : Attribute, IAuthorizationFilter
    {
        private readonly string[] _allowedRoles;

        public AuthorizeRoleAttribute(params string[] allowedRoles)
        {
            _allowedRoles = allowedRoles;
        }

        public void OnAuthorization(AuthorizationFilterContext context)
        {
            var user = context.HttpContext.User;
            
            // Check if user has claims (JWT token validation)
            if (!user.Claims.Any())
            {
                context.Result = new UnauthorizedObjectResult(new { error = "Authentication required" });
                return;
            }

            var userRoles = user.Claims
                .Where(c => c.Type == ClaimTypes.Role)
                .Select(c => c.Value.ToLower())
                .ToList();

            if (!_allowedRoles.Any(role => userRoles.Contains(role.ToLower())))
            {
                context.Result = new ForbidResult();
                return;
            }
        }
    }

    // Specific role attributes for convenience
    public class AuthorizeAdminAttribute : AuthorizeRoleAttribute
    {
        public AuthorizeAdminAttribute() : base("admin") { }
    }

    public class AuthorizeDoctorAttribute : AuthorizeRoleAttribute
    {
        public AuthorizeDoctorAttribute() : base("doctor") { }
    }

    public class AuthorizeStaffAttribute : AuthorizeRoleAttribute
    {
        public AuthorizeStaffAttribute() : base("doctor", "admin") { }
    }

    public class AuthorizePatientAttribute : AuthorizeRoleAttribute
    {
        public AuthorizePatientAttribute() : base("patient") { }
    }

    public class AuthorizePatientOrStaffAttribute : AuthorizeRoleAttribute
    {
        public AuthorizePatientOrStaffAttribute() : base("patient", "doctor", "admin") { }
    }
}

