using System.Collections.ObjectModel;
using catv1.Models;
using catv1.Services;

namespace catv1.ViewModels;

public class CourseListViewModel : BaseViewModel
{
    private readonly ICourseService _courseService;
    private readonly IAuthService _authService;

    public ObservableCollection<Course> Courses { get; } = new();

    public CourseListViewModel(ICourseService courseService, IAuthService authService)
    {
        _courseService = courseService;
        _authService = authService;
        Title = "All Courses";
    }

    public async Task LoadCoursesAsync()
    {
        if (IsBusy) return;

        try
        {
            IsBusy = true;
            var user = _authService.CurrentUser;
            if (user == null) return;

            var courses = await _courseService.GetCoursesByLecturerAsync(user.Id ?? string.Empty);
            
            Courses.Clear();
            foreach (var course in courses)
            {
                Courses.Add(course);
            }
        }
        catch (Exception ex)
        {
            System.Diagnostics.Debug.WriteLine($"Error loading courses: {ex}");
            await Shell.Current.DisplayAlertAsync("Error", "Failed to load courses.", "OK");
        }
        finally
        {
            IsBusy = false;
        }
    }
}
