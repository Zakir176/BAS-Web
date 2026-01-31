using catv1.ViewModels;

namespace catv1.Views;

public partial class SignUpPage : ContentPage
{
    public SignUpPage(SignUpViewModel viewModel)
    {
        InitializeComponent();
        BindingContext = viewModel;
    }
}
