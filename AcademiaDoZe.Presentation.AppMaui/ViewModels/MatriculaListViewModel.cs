using AcademiaDoZe.Application.DTOs;
using AcademiaDoZe.Application.Interfaces;
using AcademiaDoZe.Domain.Entities;
using CommunityToolkit.Mvvm.Input;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace AcademiaDoZe.Presentation.AppMaui.ViewModels
{
    public partial class MatriculaListViewModel : BaseViewModel
    {
        public ObservableCollection<string> FilterTypes { get; } = new() { "Id", "Id Aluno" };
        private readonly IMatriculaService _matriculaService;
        private string _searchText = string.Empty;
        public string SearchText
        {
            get => _searchText;
            set => SetProperty(ref _searchText, value);
        }
        private string _selectedFilterType = "Id";
        public string SelectedFilterType
        {
            get => _selectedFilterType;
            set => SetProperty(ref _selectedFilterType, value);
        }
        private ObservableCollection<MatriculaDTO> _matriculas = new();
        public ObservableCollection<MatriculaDTO> Matriculas
        {
            get => _matriculas;
            set => SetProperty(ref _matriculas, value);
        }
        private MatriculaDTO? _selectedMatricula;
        public MatriculaDTO? SelectedMatricula
        {
            get => _selectedMatricula;
            set => SetProperty(ref _selectedMatricula, value);
        }
        public MatriculaListViewModel(IMatriculaService matriculaService)
        {
            _matriculaService = matriculaService;
            Title = "Matrículas";
        }
        // métodos de comando

        [RelayCommand]
        private async Task AddMatriculaAsync()
        {
            try
            {
                await Shell.Current.GoToAsync("matricula");
            }
            catch (Exception ex)
            {
                await Shell.Current.DisplayAlert("Erro", $"Erro ao navegar para tela de cadastro: {ex.Message}", "OK");
            }
        }
        [RelayCommand]
        private async Task EditMatriculaAsync(MatriculaDTO matricula)
        {
            try
            {
                if (matricula == null)
                    return;
                await Shell.Current.GoToAsync($"matricula?Id={matricula.Id}");
            }
            catch (Exception ex)
            {
                await Shell.Current.DisplayAlert("Erro", $"Erro ao navegar para tela de edição: {ex.Message}", "OK");
            }
        }
        [RelayCommand]
        private async Task RefreshAsync()
        {
            IsRefreshing = true;
            await LoadMatriculaesAsync();
        }

        [RelayCommand]
        private async Task SearchMatriculaesAsync()
        {
            if (IsBusy)
                return;
            try
            {
                IsBusy = true;
                // Limpa a lista atual

                await MainThread.InvokeOnMainThreadAsync(() =>

                {
                    Matriculas.Clear();
                });
                IEnumerable<MatriculaDTO> resultados = Enumerable.Empty<MatriculaDTO>();
                // Busca os Matriculaes de acordo com o filtro
                if (string.IsNullOrWhiteSpace(SearchText))

                {
                    resultados = await _matriculaService.ObterAtivasAsync() ?? Enumerable.Empty<MatriculaDTO>();
                }
                else if (SelectedFilterType == "Id" && int.TryParse(SearchText, out int id))
                {
                    var matricula = await _matriculaService.ObterPorIdAsync(id);

                    if (matricula != null)

                        resultados = new[] { matricula };
                }
                else if (SelectedFilterType == "Id Aluno" && int.TryParse(SearchText, out int idAluno))
                {
                    var matricula = await _matriculaService.ObterPorAlunoIdAsync(idAluno);

                    if (matricula != null)

                        resultados = new[] { matricula };
                }
                // Atualiza a coleção na thread principal

                await MainThread.InvokeOnMainThreadAsync(() =>

                {
                    foreach (var item in resultados)
                    {
                        Matriculas.Add(item);
                    }
                    OnPropertyChanged(nameof(Matriculas));
                });
            }
            catch (Exception ex)
            {
                await Shell.Current.DisplayAlert("Erro", $"Erro ao buscar Matriculaes: {ex.Message}", "OK");
            }
            finally
            {
                IsBusy = false;
            }
        }

        [RelayCommand]
        private async Task LoadMatriculaesAsync()
        {
            if (IsBusy)
                return;
            try
            {
                IsBusy = true;
                // Limpa a lista atual antes de carregar novos dados
                await MainThread.InvokeOnMainThreadAsync(() =>

                {
                    Matriculas.Clear();
                    OnPropertyChanged(nameof(Matriculas));
                });
                var MatriculasList = await _matriculaService.ObterAtivasAsync();
                if (MatriculasList != null)
                {
                    // Garantir que a atualização da UI aconteça na thread principal

                    await MainThread.InvokeOnMainThreadAsync(() =>

                    {
                        foreach (var Matricula in MatriculasList)
                        {
                            Matriculas.Add(Matricula);
                        }
                        OnPropertyChanged(nameof(Matriculas));
                    });
                }
            }
            catch (Exception ex)
            {
                await Shell.Current.DisplayAlert("Erro", $"Erro ao carregar Matrículas: {ex.Message}", "OK");
            }
            finally
            {
                IsBusy = false;
                IsRefreshing = false;
            }
        }

        [RelayCommand]
        private async Task DeleteMatriculaAsync(MatriculaDTO Matricula)
        {
            if (Matricula == null)
                return;
            bool confirm = await Shell.Current.DisplayAlert(
            "Confirmar Exclusão",

            $"Deseja realmente excluir a matrícula do aluno {Matricula.AlunoMatricula.Nome}?",
            "Sim", "Não");
            if (!confirm)
                return;
            try
            {
                IsBusy = true;
                bool success = await _matriculaService.RemoverAsync(Matricula.Id);
                if (success)
                {
                    Matriculas.Remove(Matricula);
                    await Shell.Current.DisplayAlert("Sucesso", "Matricula excluído com sucesso!", "OK");
                }
                else
                {
                    await Shell.Current.DisplayAlert("Erro", "Não foi possível excluir o Matricula.", "OK");
                }
            }
            catch (Exception ex)
            {
                await Shell.Current.DisplayAlert("Erro", $"Erro ao excluir Matricula: {ex.Message}", "OK");
            }
            finally
            {
                IsBusy = false;
            }
        }
    }
}
