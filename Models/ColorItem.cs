using System.ComponentModel;
using System.Runtime.CompilerServices;

namespace ColorPaletteApp.Models
{
    public class ColorItem : INotifyPropertyChanged
    {
        private string? _hexCode = "#FFFFFF";
        private bool _isLocked = false;

        public string? HexCode
        {
            get => _hexCode;
            set
            {
                if (_hexCode != value)
                {
                    _hexCode = value;
                    OnPropertyChanged();
                }
            }
        }

        public bool IsLocked
        {
            get => _isLocked;
            set
            {
                if (_isLocked != value)
                {
                    _isLocked = value;
                    OnPropertyChanged();
                }
            }
        }

        public event PropertyChangedEventHandler? PropertyChanged;

        protected void OnPropertyChanged([CallerMemberName] string? propertyName = null) =>
            PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(propertyName));
    }
}
