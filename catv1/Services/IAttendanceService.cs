using catv1.Models;

namespace catv1.Services;

public interface IAttendanceService
{
    Task LogAttendanceAsync(ActivityLog log);
    Task<List<ActivityLog>> GetStudentLogsAsync(string studentId);
    Task<List<ActivityLog>> GetSectionLogsAsync(List<string> sectionIds);
    Task SyncOfflineLogsAsync();
}
