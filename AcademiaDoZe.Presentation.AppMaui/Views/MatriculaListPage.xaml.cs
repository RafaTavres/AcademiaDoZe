using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using AcademiaDoZe.Application.DTOs;
using AcademiaDoZe.Presentation.AppMaui.ViewModels;
using Microsoft.Maui.Controls;

namespace AcademiaDoZe.Presentation.AppMaui.Views;

public partial class MatriculaListPage : ContentPage
{
    private CancellationTokenSource? _searchCts;

    public MatriculaListPage(MatriculaListViewModel viewModel)
    {
        InitializeComponent();
        BindingContext = viewModel;
    }

    protected override async void OnAppearing()
    {
        base.OnAppearing();
        if (BindingContext is MatriculaListViewModel viewModel)
        {
            // O comando LoadMatriculaesAsync no ViewModel já lida com o estado IsBusy e IsRefreshing
            await viewModel.LoadMatriculaesCommand.ExecuteAsync(null);
        }
    }

    private async void OnEditButtonClicked(object sender, EventArgs e)
    {
        try
        {
            if (sender is Button button && button.BindingContext is MatriculaDTO matricula && BindingContext is MatriculaListViewModel viewModel)
            {
                // Chama o comando EditMatriculaAsync do ViewModel
                await viewModel.EditMatriculaCommand.ExecuteAsync(matricula);
            }
        }
        catch (Exception ex)
        {
            await DisplayAlert("Erro", $"Erro ao editar matrícula: {ex.Message}", "OK");
        }
    }

    private async void OnDeleteButtonClicked(object sender, EventArgs e)
    {
        try
        {
            if (sender is Button button && button.BindingContext is MatriculaDTO matricula && BindingContext is MatriculaListViewModel viewModel)
            {
                // Chama o comando DeleteMatriculaAsync do ViewModel
                await viewModel.DeleteMatriculaCommand.ExecuteAsync(matricula);
            }
        }
        catch (Exception ex)
        {
            await DisplayAlert("Erro", $"Erro ao excluir matrícula: {ex.Message}", "OK");
        }
    }

    private async void OnSearchDebounceTextChanged(object? sender, TextChangedEventArgs e)
    {
        try
        {
            _searchCts?.Cancel();
            _searchCts = new CancellationTokenSource();
            var token = _searchCts.Token;
            // espera curta (300ms)
            await Task.Delay(300, token);
            if (token.IsCancellationRequested) return;

            if (BindingContext is MatriculaListViewModel vm)
            {
                // Chama o comando SearchMatriculaesAsync do ViewModel
                await vm.SearchMatriculaesCommand.ExecuteAsync(null);
            }
        }
        catch (TaskCanceledException) { /* ignorar - ocorre se a pesquisa for cancelada por nova digitação */ }
        catch (Exception ex)
        {
            await DisplayAlert("Erro", $"Erro na pesquisa: {ex.Message}", "OK");
        }
    }
}