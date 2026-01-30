using catv1.ViewModels;
using Microsoft.Maui.Controls;

namespace catv1.Views;

public partial class LoginPage : ContentPage
{
    public LoginPage(LoginViewModel viewModel)
    {
        InitializeComponent();
        BindingContext = viewModel;
    }
}