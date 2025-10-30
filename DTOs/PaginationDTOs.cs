using System.ComponentModel.DataAnnotations;

namespace HopewellClinicApi.DTOs
{
    /// <summary>
    /// Request model for paginated user search
    /// </summary>
    public class UserSearchRequest
    {
        [Range(1, int.MaxValue, ErrorMessage = "Page must be greater than 0")]
        public int Page { get; set; } = 1;

        [Range(1, 100, ErrorMessage = "Limit must be between 1 and 100")]
        public int Limit { get; set; } = 10;

        [StringLength(100, ErrorMessage = "Search term cannot exceed 100 characters")]
        public string? Search { get; set; }

        [StringLength(20, ErrorMessage = "Role cannot exceed 20 characters")]
        public string? Role { get; set; }

        [StringLength(20, ErrorMessage = "Status cannot exceed 20 characters")]
        public string? Status { get; set; }

        [StringLength(20, ErrorMessage = "SortBy cannot exceed 20 characters")]
        public string? SortBy { get; set; } = "createdAt";

        [StringLength(10, ErrorMessage = "SortOrder cannot exceed 10 characters")]
        public string? SortOrder { get; set; } = "desc";
    }

    /// <summary>
    /// Generic paginated result model
    /// </summary>
    public class PagedResult<T>
    {
        public List<T> Items { get; set; } = new();
        public int TotalCount { get; set; }
        public int Page { get; set; }
        public int Limit { get; set; }
        public int TotalPages { get; set; }
        public bool HasNextPage => Page < TotalPages;
        public bool HasPreviousPage => Page > 1;
    }

    /// <summary>
    /// Pagination information
    /// </summary>
    public class PaginationInfo
    {
        public int CurrentPage { get; set; }
        public int TotalPages { get; set; }
        public int TotalItems { get; set; }
        public int ItemsPerPage { get; set; }
        public bool HasNextPage { get; set; }
        public bool HasPreviousPage { get; set; }
    }

    /// <summary>
    /// Filter information
    /// </summary>
    public class FilterInfo
    {
        public string? Search { get; set; }
        public string? Role { get; set; }
        public string? Status { get; set; }
    }

    /// <summary>
    /// Enhanced user response model with pagination
    /// </summary>
    public class UserListResponse
    {
        public List<AdminUserResponse> Users { get; set; } = new();
        public PaginationInfo Pagination { get; set; } = new();
        public FilterInfo Filters { get; set; } = new();
        public bool Success { get; set; } = true;
        public string? Error { get; set; }
    }

    /// <summary>
    /// Individual user response model for admin management
    /// </summary>
    public class AdminUserResponse
    {
        public Guid Id { get; set; }
        public string Email { get; set; } = string.Empty;
        public string FirstName { get; set; } = string.Empty;
        public string LastName { get; set; } = string.Empty;
        public string? Phone { get; set; }
        public string? DateOfBirth { get; set; } // YYYY-MM-DD format
        public string? Address { get; set; }
        public string? EmergencyContact { get; set; }
        public string? EmergencyPhone { get; set; }
        public string Role { get; set; } = string.Empty;
        public List<string> ActualRoles { get; set; } = new();
        public bool IsActive { get; set; }
        public DateTime CreatedAt { get; set; }
        public DateTime UpdatedAt { get; set; }
        public PatientInfo? PatientInfo { get; set; }
        public StaffInfo? StaffInfo { get; set; }
    }

    /// <summary>
    /// Patient-specific information
    /// </summary>
    public class PatientInfo
    {
        public Guid PatientId { get; set; }
        public string PatientNumber { get; set; } = string.Empty;
        public DateTime? DateOfBirth { get; set; }
        public string? Address { get; set; }
        public string? EmergencyContact { get; set; }
        public string? EmergencyPhone { get; set; }
    }

    /// <summary>
    /// Staff-specific information
    /// </summary>
    public class StaffInfo
    {
        public Guid StaffId { get; set; }
        public string StaffNumber { get; set; } = string.Empty;
    }
}
