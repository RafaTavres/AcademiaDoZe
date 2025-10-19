using AcademiaDoZe.Application.DTOs;
using AcademiaDoZe.Presentation.AppMaui.ViewModels;

namespace AcademiaDoZe.Presentation.AppMaui.Views;

public partial class AlunoListPage : ContentPage
{
    private CancellationTokenSource? _searchCts;

    public AlunoListPage(AlunoListViewModel viewModel)
    {
        InitializeComponent();
        BindingContext = viewModel;
    }

    protected override async void OnAppearing()
    {
        base.OnAppearing();
        if (BindingContext is AlunoListViewModel viewModel)
        {
            // Usa o comando LoadAlunosCommand para carregar os dados
            await viewModel.LoadAlunosCommand.ExecuteAsync(null);
        }
    }

    private async void OnEditButtonClicked(object sender, EventArgs e)
    {
        try
        {
            if (sender is Button button &&
                button.BindingContext is AlunoDTO aluno &&
                BindingContext is AlunoListViewModel viewModel)
            {
                await viewModel.EditAlunoCommand.ExecuteAsync(aluno);
            }
        }
        catch (Exception ex)
        {
            await DisplayAlert("Erro", $"Erro ao editar aluno: {ex.Message}", "OK");
        }
    }

    private async void OnDeleteButtonClicked(object sender, EventArgs e)
    {
        try
        {
            if (sender is Button button &&
                button.BindingContext is AlunoDTO aluno &&
                BindingContext is AlunoListViewModel viewModel)
            {
                await viewModel.DeleteAlunoCommand.ExecuteAsync(aluno);
            }
        }
        catch (Exception ex)
        {
            await DisplayAlert("Erro", $"Erro ao excluir aluno: {ex.Message}", "OK");
        }
    }

    // L�gica de "debounce" para a busca, evitando chamadas excessivas ao servi�o
    private async void OnSearchDebounceTextChanged(object? sender, TextChangedEventArgs e)
    {
        try
        {
            _searchCts?.Cancel();
            _searchCts = new CancellationTokenSource();
            var token = _searchCts.Token;

            // Espera um curto per�odo (300ms) antes de executar a busca
            await Task.Delay(300, token);
            if (token.IsCancellationRequested) return;

            if (BindingContext is AlunoListViewModel vm)
            {
                await vm.SearchAlunosCommand.ExecuteAsync(null);
            }
        }
        catch (TaskCanceledException)
        {
            // Ignora a exce��o que ocorre quando uma busca � cancelada por uma nova digita��o
        }
    }
} 