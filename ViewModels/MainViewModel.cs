using System;
using System.Collections.ObjectModel;
using System.ComponentModel;
using System.Drawing;
using System.Linq;
using System.Runtime.CompilerServices;
using System.Windows.Input;
using ColorPaletteApp.Models;
using Microsoft.Maui;

namespace ColorPaletteApp.ViewModels
{
    public class MainViewModel : INotifyPropertyChanged
    {
        public ObservableCollection<ColorItem> Colors { get; set; }
        public ICommand GenerateCommand { get; }
        public ICommand IncreaseCommand { get; }
        public ICommand DecreaseCommand { get; }
        private int _numberColors = 5;

        public int NumberColors
        {
            get => _numberColors;
            set
            {
                if (value < 1)
                    _numberColors = 1;
                else if (value > 100)
                    _numberColors = 100;
                else
                    _numberColors = value;

                OnPropertyChanged();
            }
        }


        private ColorHarmony _selectedHarmony = ColorHarmony.Complementary;
        public ColorHarmony SelectedHarmony
        {
            get => _selectedHarmony;
            set
            {
                _selectedHarmony = value;
                OnPropertyChanged();
                GenerateColors();
            }
        }
        public Array Harmonies => Enum.GetValues(typeof(ColorHarmony));

        public MainViewModel()
        {
            Colors = new ObservableCollection<ColorItem>();
            GenerateCommand = new Command(GenerateColors);
            IncreaseCommand = new Command(() => NumberColors++);
            DecreaseCommand = new Command(() => { if (NumberColors > 1) NumberColors--; });
            GenerateColors();
        }


        private void GenerateColors()
        {
            Random rand = new Random();
            Colors.Clear();

            // Color base aleatorio
            var baseColorHex = $"#{rand.Next(0x1000000):X6}";
            Colors.Add(new ColorItem { HexCode = baseColorHex });

            // Analogos o similares
            for (int i = 2; i < NumberColors; i++)
            {
                // Variamos tono/saturación para dar variedad
                System.Drawing.Color color = HEXtoRGB(baseColorHex);
                int r = Math.Min(255, Math.Max(0, color.R + rand.Next(-30, 30)));
                int g = Math.Min(255, Math.Max(0, color.G + rand.Next(-30, 30)));
                int b = Math.Min(255, Math.Max(0, color.B + rand.Next(-30, 30)));
                var hex = $"#{r:X2}{g:X2}{b:X2}";
                Colors.Add(new ColorItem { HexCode = hex });

            }
            // Color complementario
            var complementaryHex = GetComplementaryColor(baseColorHex);
            Colors.Add(new ColorItem { HexCode = complementaryHex });

            OnPropertyChanged(nameof(Colors));
        }

        private string GetComplementaryColor(string hex)
        {
            // Convertimos el color de HEX  RGB
            System.Drawing.Color color = HEXtoRGB(hex);
            int r = 255 - color.R;
            int g = 255 - color.G;
            int b = 255 - color.B;
            return $"#{r:X2}{g:X2}{b:X2}";
        }

        private System.Drawing.Color HEXtoRGB(string hex)
        {
            var color = System.Drawing.ColorTranslator.FromHtml(hex);
            return color;
        }

        // INotifyPropertyChanged implementation
        public event PropertyChangedEventHandler? PropertyChanged;
        protected void OnPropertyChanged([CallerMemberName] string? name = null) =>
            PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(name));
    }
}
