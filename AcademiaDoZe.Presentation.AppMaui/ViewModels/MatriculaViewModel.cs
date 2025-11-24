using AcademiaDoZe.Application.DTOs;
using AcademiaDoZe.Application.Enums;
using AcademiaDoZe.Application.Interfaces;
using AcademiaDoZe.Domain.Entities;
using AcademiaDoZe.Domain.Enums;
using AcademiaDoZe.Presentation.AppMaui.Helpers;
using CommunityToolkit.Mvvm.Input;
using System.Collections.ObjectModel;
using System.ComponentModel;
using System.ComponentModel.DataAnnotations;
using System.Reflection;


namespace AcademiaDoZe.Presentation.AppMaui.ViewModels
{
    [QueryProperty(nameof(MatriculaId), "Id")]
    public partial class MatriculaViewModel : BaseViewModel
    {
        public IEnumerable<EAppMatriculaRestricoes> MatriculaRestricoes { get; } = Enum.GetValues(typeof(EAppMatriculaRestricoes)).Cast<EAppMatriculaRestricoes>();
        public ObservableCollection<SelectableRestricaoItem> SelectableRestricoes { get; } = new();
        private readonly IMatriculaService _matriculaService;
        private readonly IAlunoService _alunoService;
        private readonly IPlanoService _planoService;

        private MatriculaDTO _matricula = new()
        {
            AlunoMatricula = new AlunoDTO { Nome = string.Empty, Cpf = string.Empty, DataNascimento = DateOnly.FromDateTime(DateTime.Today.AddYears(-18)), Telefone = string.Empty, Email = string.Empty, Endereco = new LogradouroDTO { Cep = string.Empty, Nome = string.Empty, Bairro = string.Empty, Cidade = string.Empty, Estado = string.Empty, Pais = string.Empty }, Numero = string.Empty, Complemento = string.Empty },
            Plano = new PlanoDTO { Tipo = string.Empty, Descricao = string.Empty, Preco = Decimal.Zero, DuracaoEmDias = 0, Ativo = false },
            DataInicio = DateOnly.FromDateTime(DateTime.UtcNow),
            DataFim = DateOnly.FromDateTime(DateTime.UtcNow),
            Objetivo = string.Empty,
            RestricoesMedicas = new List<EAppMatriculaRestricoes>(),
            ObservacoesRestricoes = null,
            LaudoMedico = null,
        };
        public MatriculaDTO Matricula
        {
            get => _matricula;
            set => SetProperty(ref _matricula, value);
        }

    
        public DateTime DataInicioSelecionada
        {
            get => Matricula.DataInicio.ToDateTime(TimeOnly.MinValue);
            set
            {
                if (Matricula.DataInicio != DateOnly.FromDateTime(value))
                {
                    Matricula.DataInicio = DateOnly.FromDateTime(value);
                    OnPropertyChanged();
                    CalcularDataFim();
                }
            }
        }
        private int _matriculaId;
        public int MatriculaId
        {
            get => _matriculaId;
            set => SetProperty(ref _matriculaId, value);
        }
        private bool _isEditMode;
        public bool IsEditMode
        {
            get => _isEditMode;
            set => SetProperty(ref _isEditMode, value);
        }
        private ObservableCollection<PlanoDTO> _planos = new();
        public ObservableCollection<PlanoDTO> Planos
        {
            get => _planos;
            set => SetProperty(ref _planos, value);
        }
        private PlanoDTO? _selectedPlano;
        public PlanoDTO? SelectedPlano
        {
            get => _selectedPlano;
            set
            {
                if (SetProperty(ref _selectedPlano, value))
                {
                    Matricula.Plano = value ?? new PlanoDTO { Tipo = string.Empty, Descricao = string.Empty, Preco = Decimal.Zero, DuracaoEmDias = 0, Ativo = false };
                    OnPropertyChanged(nameof(Matricula));
                    CalcularDataFim();
                }
            }
        }


        public MatriculaViewModel(IMatriculaService matriculaService, IAlunoService alunoService, IPlanoService planoService)
        {
            _matriculaService = matriculaService;
            _alunoService = alunoService;
            _planoService = planoService;
            Title = "Detalhes da Matrícula";
        }

        private void CalcularDataFim()
        {
            if (SelectedPlano != null && SelectedPlano.DuracaoEmDias > 0)
            {
                Matricula.DataFim = Matricula.DataInicio.AddDays(SelectedPlano.DuracaoEmDias);
            }
            else
            {
                Matricula.DataFim = Matricula.DataInicio;
            }
            OnPropertyChanged(nameof(Matricula));
        }

        [RelayCommand]
        public async Task InitializeAsync()
        {
            IsBusy = true;
            await LoadPlanosAsync();
            if (MatriculaId > 0)
            {
                IsEditMode = true;
                Title = "Editar matrícula";
                await LoadMatriculaAsync();
            }
            else
            {
                IsEditMode = false;
                Title = "Nova matrícula";
                Matricula.Plano = new PlanoDTO { Tipo = string.Empty, Descricao = string.Empty, Preco = Decimal.Zero, DuracaoEmDias = 0, Ativo = false };
                SelectedPlano = null;
                CalcularDataFim();
            }

            PopulateSelectableRestricoes();
            IsBusy = false;
        }

        private void PopulateSelectableRestricoes()
        {
            // Remove listeners antigos para evitar memory leaks
            foreach (var item in SelectableRestricoes)
            {
                item.PropertyChanged -= OnRestricaoSelectionChanged;
            }

            SelectableRestricoes.Clear();

            foreach (var restricaoValue in MatriculaRestricoes)
            {
                if (restricaoValue == EAppMatriculaRestricoes.None) continue;

                var selectableItem = new SelectableRestricaoItem
                {
                    Item = restricaoValue,
                    DisplayName = restricaoValue.GetDisplayName(),
                    IsSelected = Matricula.RestricoesMedicas.Contains(restricaoValue)
                };

                selectableItem.PropertyChanged += OnRestricaoSelectionChanged;
                SelectableRestricoes.Add(selectableItem);
            }
        }

        private void OnRestricaoSelectionChanged(object? sender, PropertyChangedEventArgs e)
        {
            if (e.PropertyName != nameof(SelectableItem<EAppMatriculaRestricoes>.IsSelected))
                return;

            Matricula.RestricoesMedicas.Clear();
            Matricula.RestricoesMedicas.AddRange(SelectableRestricoes.Where(x => x.IsSelected).Select(x => x.Item));

            OnPropertyChanged(nameof(Matricula));
        }


        [RelayCommand]
        public async Task SaveMatriculaAsync()
        {
            if (IsBusy)
                return;

            Matricula.RestricoesMedicas.Clear();
            Matricula.RestricoesMedicas.AddRange(SelectableRestricoes.Where(x => x.IsSelected).Select(x => x.Item));

            if (!await ValidateMatricula(Matricula))
                return;

            try
            {
                IsBusy = true;

                if (IsEditMode)
                {
                    await _matriculaService.AtualizarAsync(Matricula);
                    await Shell.Current.DisplayAlert("Sucesso", "Matrícula atualizada com sucesso!", "OK");
                }
                else
                {
                    await _matriculaService.AdicionarAsync(Matricula);
                    await Shell.Current.DisplayAlert("Sucesso", "Matrícula criada com sucesso!", "OK");
                }
                await Shell.Current.GoToAsync("..");
            }
            catch (Exception ex)
            {
                await Shell.Current.DisplayAlert("Erro", $"Erro ao salvar matrícula: {ex.Message}", "OK");
            }
            finally
            {
                IsBusy = false;
            }
        }

        private static async Task<bool> ValidateMatricula(MatriculaDTO matricula)
        {
            const string validationTitle = "Validação";

            if (matricula.AlunoMatricula == null || matricula.AlunoMatricula.Id <= 0)
            {
                await Shell.Current.DisplayAlert(validationTitle, "O Aluno é obrigatório. Utilize a busca por CPF.", "OK");
                return false;
            }

            if (matricula.Plano == null || matricula.Plano.Id <= 0)
            {
                await Shell.Current.DisplayAlert(validationTitle, "Um plano deve ser selecionado para a matrícula.", "OK");
                return false;
            }

            if (matricula.DataInicio == default)
            {
                await Shell.Current.DisplayAlert(validationTitle, "A Data de Início é obrigatória.", "OK");
                return false;
            }

            if (string.IsNullOrWhiteSpace(matricula.Objetivo))
            {
                await Shell.Current.DisplayAlert(validationTitle, "O campo Objetivo é obrigatório.", "OK");
                return false;
            }

            if (matricula.RestricoesMedicas.Any() && matricula.LaudoMedico == null)
            {
                if (matricula.RestricoesMedicas.All(r => r == EAppMatriculaRestricoes.None) && matricula.RestricoesMedicas.Count == 1)
                {

                }
                else
                {
                    await Shell.Current.DisplayAlert(validationTitle, "Para alunos com restrições, o Laudo Médico é obrigatório.", "OK");
                    return false;
                }
            }

            if (matricula.AlunoMatricula.DataNascimento != default && matricula.AlunoMatricula.DataNascimento > DateOnly.FromDateTime(DateTime.Today.AddYears(-16)) && matricula.LaudoMedico == null)
            {
                await Shell.Current.DisplayAlert(validationTitle, "Para alunos menores de 16 anos, o Laudo Médico é obrigatório.", "OK");
                return false;
            }

            return true;
        }

        [RelayCommand]
        private async Task SearchAlunoByCpfAsync()
        {
            if (string.IsNullOrWhiteSpace(Matricula.AlunoMatricula.Cpf))
            {
                await Shell.Current.DisplayAlert("Busca de Aluno", "Por favor, informe um CPF para buscar.", "OK");
                return;
            }

            if (IsBusy) return;

            try
            {
                IsBusy = true;
                var alunoData = await _alunoService.ObterPorCpfAsync(Matricula.AlunoMatricula.Cpf);

                if (alunoData != null && alunoData.Any())
                {
                    var alunoEncontrado = alunoData.First();
                    Matricula.AlunoMatricula = alunoEncontrado;
                    OnPropertyChanged(nameof(Matricula));
                }
                else
                {
                    await Shell.Current.DisplayAlert("Busca de Aluno", "Nenhum aluno encontrado com o CPF informado.", "OK");
                    var cpfDigitado = Matricula.AlunoMatricula.Cpf;

                    Matricula.AlunoMatricula = new AlunoDTO
                    {
                        Cpf = cpfDigitado,
                        Nome = string.Empty,
                        DataNascimento = default,
                        Telefone = string.Empty,
                        Numero = string.Empty,
                        Endereco = new LogradouroDTO
                        {
                            Cep = string.Empty,
                            Nome = string.Empty,
                            Bairro = string.Empty,
                            Cidade = string.Empty,
                            Estado = string.Empty,
                            Pais = string.Empty
                        }
                    };

                    OnPropertyChanged(nameof(Matricula));
                }
            }
            catch (Exception ex)
            {
                await Shell.Current.DisplayAlert("Erro", $"Erro ao buscar aluno: {ex.Message}", "OK");
            }
            finally
            {
                IsBusy = false;
            }
        }

        [RelayCommand]
        public void ClearLaudo()
        {
            Matricula.LaudoMedico = null;
            OnPropertyChanged(nameof(Matricula));
        }

        [RelayCommand]
        private async Task CancelAsync()
        {
            await Shell.Current.GoToAsync("..");
        }

        [RelayCommand]
        public async Task LoadMatriculaAsync()
        {
            if (MatriculaId <= 0)
                return;
            try
            {
                IsBusy = true;
                var matriculaData = await _matriculaService.ObterPorIdAsync(MatriculaId);

                if (matriculaData != null)
                {
                    Matricula = matriculaData;
                    OnPropertyChanged(nameof(DataInicioSelecionada));
                    SelectedPlano = Planos.FirstOrDefault(p => p.Id == Matricula.Plano.Id);
                    CalcularDataFim();
                }
            }
            catch (Exception ex)
            {
                await Shell.Current.DisplayAlert("Erro", $"Erro ao carregar matrícula: {ex.Message}", "OK");
            }
            finally
            {
                IsBusy = false;
            }
        }

        [RelayCommand]
        public async Task LoadPlanosAsync()
        {
            try
            {
                IsBusy = true;
                var planosList = await _planoService.ObterTodosAsync();
                Planos.Clear();
                foreach (var plano in planosList)
                {
                    Planos.Add(plano);
                }
            }
            catch (Exception ex)
            {
                await Shell.Current.DisplayAlert("Erro", $"Erro ao carregar planos: {ex.Message}", "OK");
            }
            finally
            {
                IsBusy = false;
            }
        }

        [RelayCommand]
        public async Task AddPlanoAsync()
        {
            var newPlano = new PlanoDTO { Tipo = string.Empty, Descricao = string.Empty, Preco = 0, DuracaoEmDias = 0, Ativo = true };
            bool result = await ShowPlanoEditPopup(newPlano, true);
            if (result)
            {
                await LoadPlanosAsync();
                SelectedPlano = Planos.LastOrDefault();
            }
        }

        [RelayCommand]
        public async Task EditPlanoAsync()
        {
            if (SelectedPlano == null)
            {
                await Shell.Current.DisplayAlert("Edição de Plano", "Nenhum plano selecionado para editar.", "OK");
                return;
            }

            var planoToEdit = new PlanoDTO
            {
                Id = SelectedPlano.Id,
                Tipo = SelectedPlano.Tipo,
                Descricao = SelectedPlano.Descricao,
                Preco = SelectedPlano.Preco,
                DuracaoEmDias = SelectedPlano.DuracaoEmDias,
                Ativo = SelectedPlano.Ativo
            };

            bool result = await ShowPlanoEditPopup(planoToEdit, false);
            if (result)
            {
                await LoadPlanosAsync();
                SelectedPlano = Planos.FirstOrDefault(p => p.Id == planoToEdit.Id);
            }
        }

        [RelayCommand]
        public async Task DeletePlanoAsync()
        {
            if (SelectedPlano == null)
            {
                await Shell.Current.DisplayAlert("Exclusão de Plano", "Nenhum plano selecionado para excluir.", "OK");
                return;
            }

            bool confirm = await Shell.Current.DisplayAlert("Confirmar Exclusão", $"Tem certeza que deseja excluir o plano '{SelectedPlano.Tipo}'?", "Sim", "Não");
            if (confirm)
            {
                try
                {
                    IsBusy = true;
                    bool removed = await _planoService.RemoverAsync(SelectedPlano.Id);
                    if (removed)
                    {
                        await Shell.Current.DisplayAlert("Sucesso", "Plano removido com sucesso!", "OK");
                        await LoadPlanosAsync();
                        SelectedPlano = null;
                    }
                    else
                    {
                        await Shell.Current.DisplayAlert("Erro", "Não foi possível remover o plano.", "OK");
                    }
                }
                catch (Exception ex)
                {
                    await Shell.Current.DisplayAlert("Erro", $"Erro ao excluir plano: {ex.Message}", "OK");
                }
                finally
                {
                    IsBusy = false;
                }
            }
        }

        private async Task<bool> ShowPlanoEditPopup(PlanoDTO plano, bool isNew)
        {
            string title = isNew ? "Novo Plano" : "Editar Plano";

            string? tipo = await Shell.Current.DisplayPromptAsync(title, "Tipo do Plano:", "OK", "Cancelar", "Mensal", -1, Keyboard.Text, plano.Tipo);
            if (tipo == null) return false;
            plano.Tipo = tipo;

            string? descricao = await Shell.Current.DisplayPromptAsync(title, "Descrição do Plano:", "OK", "Cancelar", "Plano básico com acesso total", -1, Keyboard.Text, plano.Descricao);
            if (descricao == null) return false;
            plano.Descricao = descricao;

            string? precoStr = await Shell.Current.DisplayPromptAsync(title, "Preço:", "OK", "Cancelar", "50.00", -1, Keyboard.Numeric, plano.Preco.ToString());
            if (!decimal.TryParse(precoStr, out decimal preco))
            {
                await Shell.Current.DisplayAlert("Erro", "Preço inválido.", "OK");
                return false;
            }
            plano.Preco = preco;

            string? duracaoStr = await Shell.Current.DisplayPromptAsync(title, "Duração em Dias:", "OK", "Cancelar", "30", -1, Keyboard.Numeric, plano.DuracaoEmDias.ToString());
            if (!int.TryParse(duracaoStr, out int duracao))
            {
                await Shell.Current.DisplayAlert("Erro", "Duração inválida.", "OK");
                return false;
            }
            plano.DuracaoEmDias = duracao;

            bool? ativoChoice = await Shell.Current.DisplayAlert(title, "Plano Ativo?", "Sim", "Não");
            if (ativoChoice == null) return false;
            plano.Ativo = ativoChoice.Value;


            try
            {
                IsBusy = true;
                if (isNew)
                {
                    await _planoService.AdicionarAsync(plano);
                    await Shell.Current.DisplayAlert("Sucesso", "Plano adicionado com sucesso!", "OK");
                }
                else
                {
                    await _planoService.AtualizarAsync(plano);
                    await Shell.Current.DisplayAlert("Sucesso", "Plano atualizado com sucesso!", "OK");
                }
                return true;
            }
            catch (Exception ex)
            {
                await Shell.Current.DisplayAlert("Erro", $"Erro ao salvar plano: {ex.Message}", "OK");
                return false;
            }
            finally
            {
                IsBusy = false;
            }
        }

        [RelayCommand]
        public async Task SelecionarFotoLaudoAsync()
        {
            try
            {
                string escolha = await Shell.Current.DisplayActionSheet("Origem da Imagem", "Cancelar", null, "Galeria", "Câmera");
                FileResult? result = null;
                if (escolha == "Galeria")
                {
                    result = await FilePicker.Default.PickAsync(new PickOptions
                    {
                        PickerTitle = "Selecione uma imagem",
                        FileTypes = FilePickerFileType.Images
                    });
                }
                else if (escolha == "Câmera")
                {
                    if (MediaPicker.Default.IsCaptureSupported)
                    {
                        result = await MediaPicker.Default.CapturePhotoAsync();
                    }
                    else
                    {
                        await Shell.Current.DisplayAlert("Erro", "Captura de foto não suportada neste dispositivo.", "OK");
                        return;
                    }
                }
                if (result != null)
                {
                    using var stream = await result.OpenReadAsync();
                    using var ms = new MemoryStream();
                    await stream.CopyToAsync(ms);
                    Matricula.LaudoMedico = new ArquivoDTO { Conteudo = ms.ToArray(), ContentType = result.FileName };
                    OnPropertyChanged(nameof(Matricula));
                }
            }
            catch (Exception ex)
            {
                await Shell.Current.DisplayAlert("Erro", $"Erro ao selecionar imagem: {ex.Message}", "OK");
            }
        }
    }
}