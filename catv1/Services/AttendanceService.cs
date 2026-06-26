using catv1.Models;
using Supabase;

namespace catv1.Services;

public class AttendanceService : IAttendanceService
{
    private readonly Client _supabase;
    private readonly IOfflineSyncService _offlineSync;

    public AttendanceService(Client supabase, IOfflineSyncService offlineSync)
    {
        _supabase = supabase;
        _offlineSync = offlineSync;
    }

    public async Task LogAttendanceAsync(ActivityLog log)
    {
        try
        {
            // If offline, this will throw an exception
            await _supabase.From<ActivityLog>().Insert(log);
        }
        catch (Exception)
        {
            // Network failure or API error, queue it offline
            await _offlineSync.QueueLogAsync(log);
        }
    }

    public async Task<List<ActivityLog>> GetStudentLogsAsync(string studentId)
    {
        var response = await _supabase.From<ActivityLog>()
            .Where(l => l.StudentId == studentId)
            .Order("session_date", Supabase.Postgrest.Constants.Ordering.Descending)
            .Get();
        return response.Models;
    }

    public async Task<List<ActivityLog>> GetSectionLogsAsync(List<string> sectionIds)
    {
        if (sectionIds == null || sectionIds.Count == 0) return [];

        var response = await _supabase.From<ActivityLog>()
            .Filter("section_id", Supabase.Postgrest.Constants.Operator.In, sectionIds.Cast<object>().ToList())
            .Get();
        return response.Models;
    }

    public async Task SyncOfflineLogsAsync()
    {
        var unsynced = await _offlineSync.GetUnsyncedLogsAsync();
        foreach (var offlineLog in unsynced)
        {
            try
            {
                var log = new ActivityLog
                {
                    Id = offlineLog.ActivityLogId,
                    StudentId = offlineLog.StudentId,
                    SectionId = offlineLog.SectionId,
                    Status = offlineLog.Status,
                    DateTime = offlineLog.DateTime
                };

                await _supabase.From<ActivityLog>().Insert(log);
                await _offlineSync.MarkAsSyncedAsync(offlineLog.Id);
            }
            catch (Exception ex)
            {
                System.Diagnostics.Debug.WriteLine($"Failed to sync log {offlineLog.Id}: {ex.Message}");
                // Break out of the loop if we still don't have connection
                break;
            }
        }
        
        await _offlineSync.ClearSyncedLogsAsync();
    }
}
