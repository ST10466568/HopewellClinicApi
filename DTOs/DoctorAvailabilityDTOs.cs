using System.ComponentModel.DataAnnotations;

namespace HopewellClinicApi.DTOs
{
    // Enhanced Doctor On Duty DTO with availability information
    public class DoctorOnDutyWithAvailabilityDto
    {
        public Guid Id { get; set; }
        public string FirstName { get; set; } = string.Empty;
        public string LastName { get; set; } = string.Empty;
        public string Email { get; set; } = string.Empty;
        public string Specialty { get; set; } = string.Empty;
        public double Rating { get; set; }
        public TimeSpan ShiftStart { get; set; }
        public TimeSpan ShiftEnd { get; set; }
        public bool IsAvailable { get; set; }
        public bool IsFullyBooked { get; set; }
        public int AvailableSlots { get; set; }
        public int TotalSlots { get; set; }
        public DateTime? NextAvailableDate { get; set; }
        public List<ServiceDto> Services { get; set; } = new();
    }

    // Enhanced Doctor On Duty Response with summary
    public class DoctorOnDutyWithAvailabilityResponse
    {
        public List<DoctorOnDutyWithAvailabilityDto> Doctors { get; set; } = new();
        public DateTime RequestedDate { get; set; }
        public int TotalAvailableDoctors { get; set; }
        public int FullyBookedDoctors { get; set; }
        public int TotalAvailableSlots { get; set; }
        public int TotalBookedSlots { get; set; }
    }

    // Doctor Availability Summary
    public class DoctorAvailabilitySummary
    {
        public DateTime Date { get; set; }
        public int TotalDoctors { get; set; }
        public int AvailableDoctors { get; set; }
        public int FullyBookedDoctors { get; set; }
        public int TotalAvailableSlots { get; set; }
        public int TotalBookedSlots { get; set; }
    }

    // Doctor with availability information
    public class DoctorWithAvailability
    {
        public Guid Id { get; set; }
        public string Name { get; set; } = string.Empty;
        public string Specialty { get; set; } = string.Empty;
        public bool IsFullyBooked { get; set; }
        public int AvailableSlots { get; set; }
        public int TotalSlots { get; set; }
        public DateTime? NextAvailableDate { get; set; }
    }

    // Availability Summary Response
    public class AvailabilitySummaryResponse
    {
        public DateTime Date { get; set; }
        public DoctorAvailabilitySummary Summary { get; set; } = new();
        public List<DoctorWithAvailability> Doctors { get; set; } = new();
    }

    // Next Available Dates Response
    public class NextAvailableDatesResponse
    {
        public Guid DoctorId { get; set; }
        public string DoctorName { get; set; } = string.Empty;
        public List<DateTime> NextAvailableDates { get; set; } = new();
        public SearchPeriod SearchPeriod { get; set; } = new();
    }

    public class SearchPeriod
    {
        public DateTime StartDate { get; set; }
        public DateTime EndDate { get; set; }
        public int DaysSearched { get; set; }
    }

    // Enhanced Available Slots Response with doctor availability
    public class AvailableSlotsWithAvailabilityResponse
    {
        public Guid DoctorId { get; set; }
        public DateTime Date { get; set; }
        public List<TimeSlotDto> AvailableSlots { get; set; } = new();
        public DoctorAvailabilityInfo DoctorAvailability { get; set; } = new();
    }

    public class DoctorAvailabilityInfo
    {
        public bool IsFullyBooked { get; set; }
        public int AvailableSlots { get; set; }
        public int TotalSlots { get; set; }
        public DateTime? NextAvailableDate { get; set; }
    }

    // API Response wrapper
    public class ApiResponse<T>
    {
        public bool Success { get; set; }
        public T? Data { get; set; }
        public string? Error { get; set; }
        public string? Message { get; set; }
    }

    // Error response for availability endpoints
    public class AvailabilityErrorResponse
    {
        public string Error { get; set; } = string.Empty;
        public string Message { get; set; } = string.Empty;
        public object? Details { get; set; }
    }
}





