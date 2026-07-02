using SQLite;
using catv1.Models;
using System.IO;

namespace catv1.Services;

public class OfflineActivityLog
{
    [PrimaryKey, AutoIncrement]
    public int Id { get; set; }
    public string ActivityLogId { get; set; } = string.Empty;
    public string StudentId { get; set; } = string.Empty;
    public string SectionId { get; set; } = string.Empty;
    public string Status { get; set; } = string.Empty;
    public DateTime DateTime { get; set; }
    public bool IsSynced { get; set; } = false;
}

public interface IOfflineSyncService
{
    Task InitAsync();
    Task QueueLogAsync(ActivityLog log);
    Task<List<OfflineActivityLog>> GetUnsyncedLogsAsync();
    Task MarkAsSyncedAsync(int localId);
    Task ClearSyncedLogsAsync();
}

public class OfflineSyncService : IOfflineSyncService
{
    private SQLiteAsyncConnection? _db;

    public async Task InitAsync()
    {
        if (_db != null)
            return;

        var databasePath = Path.Combine(FileSystem.AppDataDirectory, "catv1_offline.db3");
        _db = new SQLiteAsyncConnection(databasePath);
        await _db.CreateTableAsync<OfflineActivityLog>();
    }

    public async Task QueueLogAsync(ActivityLog log)
    {
        await InitAsync();
        var offlineLog = new OfflineActivityLog
        {
            ActivityLogId = log.Id,
            StudentId = log.StudentId,
            SectionId = log.SectionId,
            Status = log.Status ?? "Present",
            DateTime = log.DateTime,
            IsSynced = false
        };
        await _db!.InsertAsync(offlineLog);
    }

    public async Task<List<OfflineActivityLog>> GetUnsyncedLogsAsync()
    {
        await InitAsync();
        return await _db!.Table<OfflineActivityLog>().Where(x => !x.IsSynced).ToListAsync();
    }

    public async Task MarkAsSyncedAsync(int localId)
    {
        await InitAsync();
        var log = await _db!.Table<OfflineActivityLog>().Where(x => x.Id == localId).FirstOrDefaultAsync();
        if (log != null)
        {
            log.IsSynced = true;
            await _db.UpdateAsync(log);
        }
    }

    public async Task ClearSyncedLogsAsync()
    {
        await InitAsync();
        var syncedLogs = await _db!.Table<OfflineActivityLog>().Where(x => x.IsSynced).ToListAsync();
        foreach (var log in syncedLogs)
        {
            await _db.DeleteAsync(log);
        }
    }
}
