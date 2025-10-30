using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Filters;
using Microsoft.IdentityModel.Tokens;
using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using System.Text;

namespace HopewellClinicApi.Attributes
{
    public class JwtAuthorizeAttribute : Attribute, IAuthorizationFilter
    {
        public void OnAuthorization(AuthorizationFilterContext context)
        {
            try
            {
                Console.WriteLine($"JwtAuthorize: Starting authorization for {context.HttpContext.Request.Path}");
                
                var authHeader = context.HttpContext.Request.Headers["Authorization"].FirstOrDefault();
                Console.WriteLine($"JwtAuthorize: Authorization header: {authHeader}");
                
                if (string.IsNullOrEmpty(authHeader) || !authHeader.StartsWith("Bearer "))
                {
                    Console.WriteLine($"JwtAuthorize: No valid authorization header found");
                    context.Result = new UnauthorizedObjectResult(new { message = "No valid authorization header found" });
                    return;
                }

                var token = authHeader.Substring("Bearer ".Length).Trim();
                Console.WriteLine($"JwtAuthorize: Token extracted (length: {token.Length})");
                
                // Get JWT settings from configuration
                var configuration = context.HttpContext.RequestServices.GetRequiredService<IConfiguration>();
                var jwtSettings = configuration.GetSection("JwtSettings");
                var secretKey = jwtSettings["SecretKey"];
                var issuer = jwtSettings["Issuer"];
                var audience = jwtSettings["Audience"];

                Console.WriteLine($"JwtAuthorize: JWT Settings - Issuer: {issuer}, Audience: {audience}, SecretKey length: {secretKey?.Length ?? 0}");

                if (string.IsNullOrEmpty(secretKey))
                {
                    Console.WriteLine($"JwtAuthorize: SecretKey is null or empty");
                    context.Result = new UnauthorizedObjectResult(new { message = "JWT configuration error" });
                    return;
                }

                var key = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(secretKey!));
                var tokenHandler = new JwtSecurityTokenHandler();

                try
                {
                    Console.WriteLine($"JwtAuthorize: Starting token validation");
                    
                    var validationParameters = new TokenValidationParameters
                    {
                        ValidateIssuerSigningKey = true,
                        IssuerSigningKey = key,
                        ValidateIssuer = true,
                        ValidIssuer = issuer,
                        ValidateAudience = true,
                        ValidAudience = audience,
                        ValidateLifetime = true,
                        ClockSkew = TimeSpan.Zero
                    };

                    var principal = tokenHandler.ValidateToken(token, validationParameters, out _);
                    
                    Console.WriteLine($"JwtAuthorize: Token validation successful for user: {principal.Identity?.Name}");
                    
                    // Set the user principal for the request
                    context.HttpContext.User = principal;
                }
                catch (SecurityTokenException ex)
                {
                    Console.WriteLine($"JwtAuthorize: SecurityTokenException: {ex.Message}");
                    Console.WriteLine($"JwtAuthorize: SecurityTokenException Type: {ex.GetType().Name}");
                    context.Result = new UnauthorizedObjectResult(new { message = "Token validation failed", error = ex.Message });
                    return;
                }
                catch (Exception ex)
                {
                    Console.WriteLine($"JwtAuthorize: Token validation exception: {ex.Message}");
                    Console.WriteLine($"JwtAuthorize: Token validation exception type: {ex.GetType().Name}");
                    context.Result = new UnauthorizedObjectResult(new { message = "Token validation error", error = ex.Message });
                    return;
                }
            }
            catch (Exception ex)
            {
                // Log the exception for debugging
                Console.WriteLine($"JwtAuthorize Error: {ex.Message}");
                Console.WriteLine($"JwtAuthorize StackTrace: {ex.StackTrace}");
                if (ex.InnerException != null)
                {
                    Console.WriteLine($"JwtAuthorize Inner Exception: {ex.InnerException.Message}");
                }
                
                // Return 401 Unauthorized instead of 500 for authentication errors
                context.Result = new UnauthorizedObjectResult(new { 
                    message = "Authentication failed", 
                    error = ex.Message 
                });
                return;
            }
        }
    }
}














