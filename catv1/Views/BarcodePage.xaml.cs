using catv1.ViewModels;

namespace catv1.Views;

public partial class BarcodePage : ContentPage
{
	public BarcodePage(BarcodeViewModel viewModel)
	{
		InitializeComponent();
		BindingContext = viewModel;
	}

	private async void OnBackClicked(object sender, EventArgs e)
	{
		await Shell.Current.GoToAsync("..");
	}
}
