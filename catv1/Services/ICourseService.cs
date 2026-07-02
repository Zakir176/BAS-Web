using catv1.Models;

namespace catv1.Services;

public interface ICourseService
{
    Task<List<Course>> GetCoursesByLecturerAsync(string lecturerId);
    Task<Course?> GetCourseByCodeAsync(string code, string lecturerId);
    Task InsertCourseAsync(Course course);
    Task DeleteCourseAsync(string courseId);
    
    Task<List<Section>> GetSectionsByLecturerAsync(string lecturerId);
    Task<Section?> GetSectionByIdAsync(string sectionId);
    Task InsertSectionAsync(Section section);
    Task<List<Enrollment>> GetEnrollmentsByStudentAsync(string studentId);
    Task<List<Enrollment>> GetEnrollmentsBySectionsAsync(List<string> sectionIds);
    Task<Course?> GetCourseByIdAsync(string courseId);
}
