using SQLite;
using catv1.Models;
using System.IO;
using Microsoft.Maui.Storage;

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

    // SEC-L2: Key stored in hardware-backed SecureStorage (Android Keystore / iOS Keychain).
    // Generated once on first run and reused for the lifetime of the install.
    private const string DbEncryptionKeyName = "OfflineDbEncryptionKey";

    private static async Task<string> GetOrCreateDbKeyAsync()
    {
        var key = await SecureStorage.Default.GetAsync(DbEncryptionKeyName);
        if (string.IsNullOrEmpty(key))
        {
            // 32 random bytes → 64-char hex string → used as SQLCipher passphrase
            var bytes = new byte[32];
            System.Security.Cryptography.RandomNumberGenerator.Fill(bytes);
            key = Convert.ToHexString(bytes);
            await SecureStorage.Default.SetAsync(DbEncryptionKeyName, key);
        }
        return key;
    }

    public async Task InitAsync()
    {
        if (_db != null)
            return;

        var databasePath = Path.Combine(FileSystem.AppDataDirectory, "catv1_offline.db3");
        var encryptionKey = await GetOrCreateDbKeyAsync();

        // SEC-L2: Open the database with SQLCipher encryption.
        // If the connection fails (e.g. an existing unencrypted legacy file is present),
        // delete it and start fresh — the offline queue is a temporary sync buffer, not
        // a source of truth, so losing unsynced rows is acceptable vs. leaving data exposed.
        try
        {
            var connectionString = new SQLiteConnectionString(databasePath, true, key: encryptionKey);
            _db = new SQLiteAsyncConnection(connectionString);
            await _db.CreateTableAsync<OfflineActivityLog>();
        }
        catch (SQLiteException)
        {
            System.Diagnostics.Debug.WriteLine("[OfflineSyncService] InitAsync: failed to open encrypted DB — deleting legacy file and recreating.");
            if (File.Exists(databasePath))
                File.Delete(databasePath);

            var connectionString = new SQLiteConnectionString(databasePath, true, key: encryptionKey);
            _db = new SQLiteAsyncConnection(connectionString);
            await _db.CreateTableAsync<OfflineActivityLog>();
        }
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

