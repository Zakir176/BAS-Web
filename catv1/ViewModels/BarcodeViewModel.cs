using CommunityToolkit.Mvvm.ComponentModel;

namespace catv1.ViewModels;

[QueryProperty("Name", "name")]
[QueryProperty("StudentId", "id")]
[QueryProperty("QrCodeValue", "qrcode")]
public partial class BarcodeViewModel : BaseViewModel
{
    [ObservableProperty]
    private string name = string.Empty;

    [ObservableProperty]
    private string studentId = "00000000";

    [ObservableProperty]
    [NotifyPropertyChangedFor(nameof(HasBarcode))]
    [NotifyPropertyChangedFor(nameof(HasNoBarcode))]
    private string qrCodeValue = string.Empty;

    public bool HasBarcode => !string.IsNullOrEmpty(QrCodeValue);
    public bool HasNoBarcode => string.IsNullOrEmpty(QrCodeValue);

    public BarcodeViewModel()
    {
        Title = "Digital ID";
    }
}
