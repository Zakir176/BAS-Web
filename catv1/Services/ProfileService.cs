using catv1.Models;
using Supabase;

namespace catv1.Services;

public interface IProfileService
{
    Task<Student?> GetStudentByIdAsync(string id);
    Task<Student?> GetStudentByEmailAsync(string email);
    Task<Student?> GetStudentByStudentNumberAsync(string studentNumber);
    Task<LecturerProfile?> GetLecturerByIdAsync(string id);
    Task<LecturerProfile?> GetLecturerByEmailAsync(string email);
    Task<Department?> GetDepartmentByIdAsync(string id);
    Task<List<Department>> GetAllDepartmentsAsync();
    Task<List<Student>> GetStudentsByIdsAsync(List<string> ids);
    Task InsertStudentAsync(Student student);
    Task InsertLecturerAsync(LecturerProfile lecturer);
}

public class ProfileService : IProfileService
{
    private readonly Client _supabase;

    public ProfileService(Client supabase)
    {
        _supabase = supabase;
    }

    public async Task<Student?> GetStudentByIdAsync(string id)
    {
        var response = await _supabase.From<Student>().Where(s => s.Id == id).Get();
        return response.Models.FirstOrDefault();
    }

    public async Task<Student?> GetStudentByEmailAsync(string email)
    {
        var response = await _supabase.From<Student>()
            .Filter("email", Supabase.Postgrest.Constants.Operator.ILike, email)
            .Get();
        return response.Models.FirstOrDefault();
    }

    public async Task<Student?> GetStudentByStudentNumberAsync(string studentNumber)
    {
        var response = await _supabase.From<Student>()
            .Where(x => x.StudentNumber == studentNumber)
            .Get();
        return response.Models.FirstOrDefault();
    }

    public async Task<LecturerProfile?> GetLecturerByIdAsync(string id)
    {
        var response = await _supabase.From<LecturerProfile>().Where(l => l.Id == id).Get();
        return response.Models.FirstOrDefault();
    }

    public async Task<LecturerProfile?> GetLecturerByEmailAsync(string email)
    {
        var response = await _supabase.From<LecturerProfile>()
            .Filter("email", Supabase.Postgrest.Constants.Operator.ILike, email)
            .Get();
        return response.Models.FirstOrDefault();
    }

    public async Task<Department?> GetDepartmentByIdAsync(string id)
    {
        var response = await _supabase.From<Department>().Where(d => d.Id == id).Get();
        return response.Models.FirstOrDefault();
    }

    public async Task<List<Department>> GetAllDepartmentsAsync()
    {
        var response = await _supabase.From<Department>().Get();
        return response.Models;
    }

    public async Task<List<Student>> GetStudentsByIdsAsync(List<string> ids)
    {
        if (ids == null || ids.Count == 0) return [];
        var response = await _supabase.From<Student>()
            .Filter("id", Supabase.Postgrest.Constants.Operator.In, ids.Cast<object>().ToList())
            .Get();
        return response.Models;
    }

    public async Task InsertStudentAsync(Student student)
    {
        await _supabase.From<Student>().Insert(student);
    }

    public async Task InsertLecturerAsync(LecturerProfile lecturer)
    {
        await _supabase.From<LecturerProfile>().Insert(lecturer);
    }
}
