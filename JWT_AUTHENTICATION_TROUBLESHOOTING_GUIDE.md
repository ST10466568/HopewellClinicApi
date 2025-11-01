# JWT Authentication Troubleshooting Guide

## Overview

This document outlines the various JWT authentication issues encountered during the development of the Hopewell Clinic API and the solutions implemented to resolve them. The authentication system uses JWT tokens with ASP.NET Core Identity and custom authorization attributes.

## Table of Contents

1. [Initial Setup Issues](#initial-setup-issues)
2. [Middleware Pipeline Problems](#middleware-pipeline-problems)
3. [CORS Configuration Conflicts](#cors-configuration-conflicts)
4. [Route Conflicts](#route-conflicts)
5. [Token Validation Issues](#token-validation-issues)
6. [Authorization Attribute Problems](#authorization-attribute-problems)
7. [Frontend Integration Challenges](#frontend-integration-challenges)
8. [Solutions Summary](#solutions-summary)

---

## Initial Setup Issues

### Problem 1: JWT Configuration Missing

**Symptoms:**
- 401 Unauthorized errors on all protected endpoints
- No JWT token validation occurring
- Authentication middleware not running

**Root Cause:**
The JWT authentication was not properly configured in `Program.cs`.

**Solution:**
```csharp
// Program.cs
builder.Services.AddAuthentication(JwtBearerDefaults.AuthenticationScheme)
    .AddJwtBearer(options =>
    {
        options.TokenValidationParameters = new TokenValidationParameters
        {
            ValidateIssuer = true,
            ValidateAudience = true,
            ValidateLifetime = true,
            ValidateIssuerSigningKey = true,
            ValidIssuer = builder.Configuration["Jwt:Issuer"],
            ValidAudience = builder.Configuration["Jwt:Audience"],
            IssuerSigningKey = new SymmetricSecurityKey(
                Encoding.UTF8.GetBytes(builder.Configuration["Jwt:SecretKey"]))
        };
    });
```

---

## Middleware Pipeline Problems

### Problem 2: Middleware Order Issues

**Symptoms:**
- JWT authentication not triggering
- "Logon Method: Anonymous" in Azure logs
- Authentication middleware running after CORS

**Root Cause:**
The middleware pipeline order was incorrect. CORS middleware was running before authentication middleware.

**Solution:**
```csharp
// Program.cs - Correct middleware order
app.UseRouting();
app.UseCors("DebugCors");
app.UseAuthentication();  // Must come before UseAuthorization
app.UseAuthorization();   // Must come before custom middleware
app.UseMiddleware<CustomCorsMiddleware>(); // Custom middleware last
app.MapControllers();
```

**Key Learning:** Authentication middleware must run before authorization middleware, and both must run before any custom middleware that might intercept requests.

---

## CORS Configuration Conflicts

### Problem 3: Custom CORS Middleware Interference

**Symptoms:**
- OPTIONS requests being intercepted
- JWT authentication not reaching controllers
- CORS errors despite configuration

**Root Cause:**
Custom CORS middleware was intercepting OPTIONS requests before JWT authentication could process them.

**Solution:**
```csharp
// Removed conflicting custom CORS middleware
// app.UseMiddleware<CustomCorsMiddleware>(); // REMOVED

// Used only the configured CORS policy
app.UseCors("DebugCors");
```

**Alternative Solution (if custom middleware needed):**
```csharp
app.UseMiddleware<CustomCorsMiddleware>(async (context, next) =>
{
    // Only process CORS for non-authenticated requests
    if (!context.Request.Headers.ContainsKey("Authorization"))
    {
        // Handle CORS
    }
    await next();
});
```

---

## Route Conflicts

### Problem 4: Generic Routes Catching Specific Requests

**Symptoms:**
- 404 errors on specific endpoints
- Generic `{id}` routes intercepting other requests
- Authentication not reaching intended controllers

**Root Cause:**
Generic routes like `[HttpGet("{id}")]` were too broad and catching requests meant for other endpoints.

**Solution:**
```csharp
// Before (Problematic)
[HttpGet("{id}")]
public async Task<ActionResult> GetAppointment(Guid id)

[HttpGet("admin/{id}")]
public async Task<ActionResult> GetAdminAppointment(Guid id)

// After (Fixed)
[HttpGet("get/{id}")]
public async Task<ActionResult> GetAppointment(Guid id)

[HttpGet("admin/appointment/{id}")]
public async Task<ActionResult> GetAdminAppointment(Guid id)
```

**Route Ordering Strategy:**
1. Most specific routes first
2. Generic routes last
3. Test endpoints at the top

---

## Token Validation Issues

### Problem 5: JWT Token Claims Mismatch

**Symptoms:**
- Valid tokens being rejected
- Claims not found in user context
- Authorization failures despite valid tokens

**Root Cause:**
The frontend was sending tokens with different claim names than expected by the backend.

**Solution:**
```csharp
// Handle multiple claim types
var userIdClaim = User.FindFirst("sub")?.Value ?? 
                  User.FindFirst(ClaimTypes.NameIdentifier)?.Value;

var roleClaim = User.FindFirst(ClaimTypes.Role)?.Value ?? 
                User.FindFirst("role")?.Value;
```

**JWT Token Structure:**
```json
{
  "sub": "user-id",
  "http://schemas.xmlsoap.org/ws/2005/05/identity/claims/nameidentifier": "user-id",
  "http://schemas.microsoft.com/ws/2008/06/identity/claims/role": "admin",
  "role": "admin"
}
```

---

## Authorization Attribute Problems

### Problem 6: Custom Authorization Attribute Issues

**Symptoms:**
- `[JwtAuthorize]` attribute not working
- Authentication bypassing custom logic
- Inconsistent authorization behavior

**Root Cause:**
The custom `JwtAuthorizeAttribute` was not properly integrated with ASP.NET Core's authorization system.

**Solution:**
```csharp
public class JwtAuthorizeAttribute : Attribute, IAuthorizationFilter
{
    public void OnAuthorization(AuthorizationFilterContext context)
    {
        try
        {
            // Check if user is authenticated
            if (!context.HttpContext.User.Identity.IsAuthenticated)
            {
                context.Result = new UnauthorizedResult();
                return;
            }

            // Validate JWT token claims
            var userIdClaim = context.HttpContext.User.FindFirst("sub")?.Value ?? 
                             context.HttpContext.User.FindFirst(ClaimTypes.NameIdentifier)?.Value;
            
            if (string.IsNullOrEmpty(userIdClaim))
            {
                context.Result = new UnauthorizedResult();
                return;
            }

            // Additional validation logic...
        }
        catch (Exception ex)
        {
            Console.WriteLine($"JwtAuthorize Error: {ex.Message}");
            context.Result = new UnauthorizedResult();
        }
    }
}
```

---

## Frontend Integration Challenges

### Problem 7: Frontend Calling Wrong Endpoints

**Symptoms:**
- Frontend getting 404 errors
- Wrong data being displayed
- API calls failing

**Root Cause:**
The frontend was calling `/api/Staff` instead of `/api/Admin/users` for user management.

**Solution:**
```javascript
// Before (Wrong)
const response = await fetch('/api/Staff');

// After (Correct)
const response = await fetch('/api/Admin/users', {
  headers: {
    'Authorization': `Bearer ${token}`,
    'Content-Type': 'application/json'
  }
});
```

**API Endpoint Clarification:**
- `/api/Staff` - Returns only staff members (10 users)
- `/api/Admin/users` - Returns all users including patients (14 users)

---

## Solutions Summary

### 1. JWT Configuration
```csharp
// Complete JWT setup in Program.cs
builder.Services.AddAuthentication(JwtBearerDefaults.AuthenticationScheme)
    .AddJwtBearer(options =>
    {
        options.TokenValidationParameters = new TokenValidationParameters
        {
            ValidateIssuer = true,
            ValidateAudience = true,
            ValidateLifetime = true,
            ValidateIssuerSigningKey = true,
            ValidIssuer = builder.Configuration["Jwt:Issuer"],
            ValidAudience = builder.Configuration["Jwt:Audience"],
            IssuerSigningKey = new SymmetricSecurityKey(
                Encoding.UTF8.GetBytes(builder.Configuration["Jwt:SecretKey"]))
        };
        
        // Add event handlers for debugging
        options.Events = new JwtBearerEvents
        {
            OnAuthenticationFailed = context =>
            {
                Console.WriteLine($"Authentication failed: {context.Exception.Message}");
                return Task.CompletedTask;
            },
            OnTokenValidated = context =>
            {
                Console.WriteLine($"Token validated for user: {context.Principal.Identity.Name}");
                return Task.CompletedTask;
            }
        };
    });
```

### 2. Middleware Pipeline Order
```csharp
// Correct middleware order
app.UseRouting();
app.UseCors("DebugCors");
app.UseAuthentication();
app.UseAuthorization();
app.MapControllers();
```

### 3. Route Organization
```csharp
// Organize routes from most specific to least specific
[HttpGet("test/service")]           // Test endpoints first
[HttpGet("admin/appointment/{id}")] // Specific admin routes
[HttpGet("admin/search")]           // Admin search
[HttpGet("get/{id}")]              // Generic get by ID
[HttpGet]                          // Generic get all (last)
```

### 4. Error Handling and Logging
```csharp
// Comprehensive error handling
try
{
    // JWT validation logic
}
catch (Exception ex)
{
    Console.WriteLine($"JWT Error: {ex.Message}");
    Console.WriteLine($"Stack Trace: {ex.StackTrace}");
    if (ex.InnerException != null)
    {
        Console.WriteLine($"Inner Exception: {ex.InnerException.Message}");
    }
    return Unauthorized();
}
```

### 5. Frontend Integration
```javascript
// Proper API integration
const apiCall = async (endpoint, method = 'GET', data = null) => {
  const token = localStorage.getItem('token');
  const headers = {
    'Authorization': `Bearer ${token}`,
    'Content-Type': 'application/json'
  };
  
  const response = await fetch(`/api/${endpoint}`, {
    method,
    headers,
    body: data ? JSON.stringify(data) : null
  });
  
  if (!response.ok) {
    throw new Error(`API call failed: ${response.status}`);
  }
  
  return response.json();
};
```

---

## Debugging Tips

### 1. Enable Detailed Logging
```csharp
// Add to Program.cs for debugging
builder.Logging.AddConsole();
builder.Logging.SetMinimumLevel(LogLevel.Debug);
```

### 2. Check Azure Logs
```bash
# Monitor Azure logs in real-time
az webapp log tail --resource-group "RESOURCE_GROUP" --name "APP_NAME"
```

### 3. Test JWT Tokens
```bash
# Decode JWT token to verify claims
echo "JWT_TOKEN" | base64 -d
```

### 4. Verify Middleware Order
```csharp
// Add logging to verify middleware execution order
app.Use(async (context, next) =>
{
    Console.WriteLine($"Request: {context.Request.Method} {context.Request.Path}");
    await next();
    Console.WriteLine($"Response: {context.Response.StatusCode}");
});
```

---

## Common Pitfalls to Avoid

1. **Middleware Order**: Always place authentication before authorization
2. **CORS Conflicts**: Avoid custom CORS middleware that intercepts OPTIONS requests
3. **Route Specificity**: Use specific routes before generic ones
4. **Claim Names**: Handle multiple claim name formats
5. **Error Handling**: Always wrap JWT validation in try-catch blocks
6. **Frontend Integration**: Ensure frontend calls the correct endpoints
7. **Token Validation**: Verify token structure and claims before processing

---

## Final Working Configuration

The final working JWT authentication setup includes:

1. **Proper JWT configuration** with all validation parameters
2. **Correct middleware pipeline order** with authentication before authorization
3. **Specific route patterns** to avoid conflicts
4. **Comprehensive error handling** with detailed logging
5. **Flexible claim handling** for different token formats
6. **Frontend integration** with correct endpoint calls

This configuration ensures reliable JWT authentication across all protected endpoints while maintaining proper error handling and debugging capabilities.

---

*Last Updated: October 11, 2025*
*Version: 1.0*











