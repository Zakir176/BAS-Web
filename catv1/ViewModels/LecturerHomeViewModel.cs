using catv1.Models;
namespace catv1.ViewModels;

public class LecturerHomeViewModel : BaseViewModel
{
    private readonly Supabase.Client _supabase;

    private string _name = "Lecturer";
    public string Name
    {
        get => _name;
        set => SetProperty(ref _name, value);
    }

    private string _department = "---";
    public string Department
    {
        get => _department;
        set => SetProperty(ref _department, value);
    }

    public LecturerHomeViewModel(Supabase.Client supabase)
    {
        _supabase = supabase;
        Title = "Lecturer Dashboard";
    }

    public async Task LoadDataAsync()
    {
        if (IsBusy) return;

        try
        {
            IsBusy = true;

            var user = _supabase.Auth.CurrentUser;
            if (user == null) return;

            var profileResponse = await _supabase.From<LecturerProfile>()
                .Where(l => l.Id == user.Id)
                .Single();

            if (profileResponse != null)
            {
                Name = profileResponse.FullName;
                
                // Fetch Department Name
                if (!string.IsNullOrEmpty(profileResponse.DepartmentId))
                {
                    var deptResponse = await _supabase.From<Department>()
                        .Where(d => d.Id == profileResponse.DepartmentId)
                        .Single();
                    
                    if (deptResponse != null)
                    {
                        Department = deptResponse.Name;
                    }
                    else
                    {
                        Department = "Unknown Department";
                    }
                }
            }
        }
        catch (Exception ex)
        {
            System.Diagnostics.Debug.WriteLine($"LoadData Error: {ex}");
        }
        finally
        {
            IsBusy = false;
        }
    }
}
