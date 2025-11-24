using AcademiaDoZe.Application.Enums;
using AcademiaDoZe.Presentation.AppMaui.Message;
using CommunityToolkit.Mvvm.Messaging;
namespace AcademiaDoZe.Presentation.AppMaui.Views;
using AcademiaDoZe.Presentation.AppMaui.Resources.Strings;
public partial class ConfigPage : ContentPage
{
    private VisualElement[] _focusOrder = [];
    public ConfigPage()
    {
        InitializeComponent();
        CarregarTema();
        CarregarBanco();
        CarregarCultura();

        TemaPicker.SelectedIndexChanged += OnSalvarTemaClicked;

        CulturaPicker.SelectedIndexChanged += OnSalvarCulturaClicked;
        // inicializa a ordem de foco dos controles
        _focusOrder = [
        DatabaseTypePicker, ServidorEntry, BancoEntry, UsuarioEntry, SenhaEntry, ComplementoEntry ];
    }
    private void CarregarTema()
    {
        // uso de expressão switch para carregar o índice selecionado
        TemaPicker.SelectedIndex = Preferences.Get("Tema", "system") switch { "light" => 0, "dark" => 1, _ => 2, };
    }
    private async void OnSalvarTemaClicked(object sender, EventArgs e)
    {
        string selectedTheme = TemaPicker.SelectedIndex switch { 0 => "light", 1 => "dark", _ => "system" };
        Preferences.Set("Tema", selectedTheme);
        // Disparar mensagem para uso na recarga dinâmica
        WeakReferenceMessenger.Default.Send(new TemaPreferencesUpdatedMessage("TemaAlterado"));
        await DisplayAlert("Sucesso", "Dados salvos com sucesso!", "OK");
    }


    private void CarregarCultura()
    {
        // uso de expressão switch para carregar o índice selecionado
        CulturaPicker.SelectedIndex = Preferences.Get("Cultura", "") switch { "en-US" => 0, "es-ES" => 1, _ => 2, };
        // se o valor não estiver definido, seleciona o index 2, que é o padrão pt-BR
    }
    private async void OnSalvarCulturaClicked(object? sender, EventArgs e)
    {
        string selected = CulturaPicker.SelectedIndex switch { 0 => "en-US", 1 => "es-ES", _ => "" };
        // Disparar mensagem com o idioma selecionado - App.xaml.cs irá capturar esta mensagem e salvar
        WeakReferenceMessenger.Default.Send(new CulturaPreferencesUpdatedMessage(selected));
    }

    // Banco de Dados
    private void CarregarBanco()
    {
        foreach (var tipo in Enum.GetValues<EAppDatabaseType>())
        {
            DatabaseTypePicker.Items.Add(tipo.ToString());
        }
        // Carregar os dados existentes, ou valores padrão, ao abrir a página
        ServidorEntry.Text = Preferences.Get("Servidor", "172.24.32.1");
        BancoEntry.Text = Preferences.Get("Banco", "db_academia_do_ze");
        UsuarioEntry.Text = Preferences.Get("Usuario", "sa");
        SenhaEntry.Text = Preferences.Get("Senha", "abcBolinhas12345");
        ComplementoEntry.Text = Preferences.Get("Complemento", "TrustServerCertificate=True;Encrypt=True;");
        DatabaseTypePicker.SelectedItem = Preferences.Get("DatabaseType", EAppDatabaseType.SqlServer.ToString());
    }
    private async void OnSalvarBdClicked(object sender, EventArgs e)
    {
        Preferences.Set("Servidor", ServidorEntry.Text);
        Preferences.Set("Banco", BancoEntry.Text);
        Preferences.Set("Usuario", UsuarioEntry.Text);
        Preferences.Set("Senha", SenhaEntry.Text);
        Preferences.Set("Complemento", ComplementoEntry.Text);
        Preferences.Set("DatabaseType", DatabaseTypePicker.SelectedItem.ToString());
        // Disparar a mensagem para recarga dinâmica
        WeakReferenceMessenger.Default.Send(new BancoPreferencesUpdatedMessage("BancoAlterado"));
        await DisplayAlert("Sucesso", "Dados salvos com sucesso!", "OK");
        // Navegar para dashboard
        await Shell.Current.GoToAsync("//dashboard");
    }


    private async void OnCancelarClicked(object sender, EventArgs e)
    {
        // retornar para dashboard
        await Shell.Current.GoToAsync("//dashboard");
    }
    // Ao fechar a página, chama WeakReferenceMessenger.Default.UnregisterAll(this); para evitar vazamentos de memória - memory leaks
    protected override void OnDisappearing()
    {
        base.OnDisappearing();
        // Desinscreve o mensageiro para evitar memory leaks
        WeakReferenceMessenger.Default.UnregisterAll(this);
    }


    private void OnEntryCompleted(object sender, EventArgs e)
    {
        if (sender is not VisualElement current)
            return;
        int idx = Array.IndexOf(_focusOrder, current);
        if (idx >= 0)
        {
            if (idx < _focusOrder.Length - 1)
            {
                // foca o próximo controle
                _focusOrder[idx + 1].Focus();
            }
            else
            {
                // último item -> submete
                OnSalvarBdClicked(sender, e);
            }
        }
        else
        {
            // fallback simples: avançar para o primeiro focável se não estiver na lista
            _focusOrder.FirstOrDefault()?.Focus();
        }
    }


}