using CommunityToolkit.Mvvm.ComponentModel;

namespace AcademiaDoZe.Presentation.AppMaui.Helpers
{
    public partial class SelectableItem<T> : ObservableObject
    {
        [ObservableProperty]
        private bool _isSelected;

        public T Item { get; set; }

        public string DisplayName { get; set; }
    }
}