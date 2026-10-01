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
    Task<List<Section>> GetSectionsByIdsAsync(List<string> sectionIds); // DES-004: batch fetch
    Task InsertSectionAsync(Section section);
    Task<List<Enrollment>> GetEnrollmentsByStudentAsync(string studentId);
    Task<List<Enrollment>> GetEnrollmentsBySectionsAsync(List<string> sectionIds);
    Task<Course?> GetCourseByIdAsync(string courseId);
    Task<List<Course>> GetCoursesByIdsAsync(List<string> courseIds); // DES-004: batch fetch
}
