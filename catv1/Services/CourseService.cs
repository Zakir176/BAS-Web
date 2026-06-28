using catv1.Models;
using Supabase;

namespace catv1.Services;

public class CourseService : ICourseService
{
    private readonly Client _supabase;

    public CourseService(Client supabase)
    {
        _supabase = supabase;
    }

    public async Task<List<Course>> GetCoursesByLecturerAsync(string lecturerId)
    {
        var response = await _supabase.From<Course>().Where(c => c.LecturerId == lecturerId).Get();
        return response.Models;
    }

    public async Task<Course?> GetCourseByCodeAsync(string code, string lecturerId)
    {
        var response = await _supabase.From<Course>()
            .Where(c => c.LecturerId == lecturerId && c.Code == code)
            .Get();
        return response.Models.FirstOrDefault();
    }

    public async Task InsertCourseAsync(Course course)
    {
        await _supabase.From<Course>().Insert(course);
    }

    public async Task DeleteCourseAsync(string courseId)
    {
        await _supabase.From<Course>().Where(c => c.Id == courseId).Delete();
    }

    public async Task<List<Section>> GetSectionsByLecturerAsync(string lecturerId)
    {
        var response = await _supabase.From<Section>().Where(s => s.LecturerId == lecturerId).Get();
        return response.Models;
    }

    public async Task<Section?> GetSectionByIdAsync(string sectionId)
    {
        var response = await _supabase.From<Section>().Where(s => s.Id == sectionId).Get();
        return response.Models.FirstOrDefault();
    }

    public async Task InsertSectionAsync(Section section)
    {
        await _supabase.From<Section>().Insert(section);
    }

    public async Task<List<Enrollment>> GetEnrollmentsByStudentAsync(string studentId)
    {
        var response = await _supabase.From<Enrollment>().Where(e => e.StudentId == studentId).Get();
        return response.Models;
    }

    public async Task<List<Enrollment>> GetEnrollmentsBySectionsAsync(List<string> sectionIds)
    {
        if (sectionIds == null || sectionIds.Count == 0) return [];
        
        var response = await _supabase.From<Enrollment>()
            .Filter("section_id", Supabase.Postgrest.Constants.Operator.In, sectionIds.Cast<object>().ToList())
            .Get();
        return response.Models;
    }

    public async Task<Course?> GetCourseByIdAsync(string courseId)
    {
        var response = await _supabase.From<Course>().Where(c => c.Id == courseId).Get();
        return response.Models.FirstOrDefault();
    }
}
