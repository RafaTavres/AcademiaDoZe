using AcademiaDoZe.Presentation.AppMaui.ViewModels;
using System.Text.RegularExpressions;

namespace AcademiaDoZe.Presentation.AppMaui.Views;

public partial class AlunoPage : ContentPage
{
    public AlunoPage(AlunoViewModel viewModel)
    {
        InitializeComponent();
        BindingContext = viewModel;
    }

    protected override async void OnAppearing()
    {
        base.OnAppearing();

        if (BindingContext is AlunoViewModel viewModel)
        {
            await viewModel.InitializeAsync();
        }
    }

    private void OnEmailUnfocused(object sender, FocusEventArgs e)
    {
        var entry = sender as Entry;
        if (string.IsNullOrEmpty(entry?.Text))
        {
            EmailErrorLabel.IsVisible = false;
            return;
        }

        bool isEmailValid = Regex.IsMatch(entry.Text, @"^[^@\s]+@[^@\s]+\.[^@\s]+$");
        EmailErrorLabel.IsVisible = !isEmailValid;
    }
}