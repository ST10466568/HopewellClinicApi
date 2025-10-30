using HopewellClinicApi.DTOs;
using HopewellClinicApi.Models;

namespace HopewellClinicApi.Services
{
    public interface IDoctorAvailabilityService
    {
        /// <summary>
        /// Get availability summary for a specific date
        /// </summary>
        Task<DoctorAvailabilitySummary> GetAvailabilitySummaryAsync(DateTime date);

        /// <summary>
        /// Get doctors with availability information for a specific date
        /// </summary>
        Task<List<DoctorWithAvailability>> GetDoctorsWithAvailabilityAsync(DateTime date);

        /// <summary>
        /// Get availability information for a specific doctor on a specific date
        /// </summary>
        Task<DoctorAvailability?> GetDoctorAvailabilityAsync(Guid doctorId, DateTime date);

        /// <summary>
        /// Update availability data for a specific doctor on a specific date
        /// </summary>
        Task UpdateDoctorAvailabilityAsync(Guid doctorId, DateTime date);

        /// <summary>
        /// Get next available dates for a doctor within a specified range
        /// </summary>
        Task<List<DateTime>> GetNextAvailableDatesAsync(Guid doctorId, int maxDays = 7);

        /// <summary>
        /// Calculate total slots for a doctor based on their shift schedule
        /// </summary>
        Task<int> CalculateTotalSlotsAsync(Guid doctorId, DateTime date);

        /// <summary>
        /// Calculate booked slots for a doctor on a specific date
        /// </summary>
        Task<int> CalculateBookedSlotsAsync(Guid doctorId, DateTime date);

        /// <summary>
        /// Update availability for all doctors for a date range
        /// </summary>
        Task UpdateAvailabilityForDateRangeAsync(DateTime startDate, DateTime endDate);

        /// <summary>
        /// Get availability data for multiple doctors on a specific date
        /// </summary>
        Task<Dictionary<Guid, DoctorAvailability>> GetDoctorsAvailabilityAsync(List<Guid> doctorIds, DateTime date);

        /// <summary>
        /// Check doctor availability with shift schedule validation
        /// </summary>
        Task<DoctorAvailabilityResult> CheckDoctorAvailabilityAsync(Guid doctorId, DateTime date);

        /// <summary>
        /// Check if doctor is on duty for a specific date
        /// </summary>
        Task<bool> IsDoctorOnDutyAsync(Guid doctorId, DateTime date);

        /// <summary>
        /// Get doctor shift information for a specific date
        /// </summary>
        Task<DoctorShiftInfo?> GetDoctorShiftInfoAsync(Guid doctorId, DateTime date);
    }
}
