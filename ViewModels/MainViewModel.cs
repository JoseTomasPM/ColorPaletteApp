using ColorPaletteApp.Models;
using ColorPaletteApp.Views;
using Microsoft.Maui;
using System;
using System.Collections.ObjectModel;
using System.ComponentModel;
using System.Drawing;
using System.Linq;
using System.Runtime.CompilerServices;
using System.Windows.Input;

namespace ColorPaletteApp.ViewModels
{
    public class MainViewModel : INotifyPropertyChanged
    {
        public ObservableCollection<ColorItem> Colors { get; set; }

        public ICommand GenerateCommand { get; }
        public ICommand IncreaseCommand { get; }
        public ICommand DecreaseCommand { get; }
        public ICommand RandomColorCommand { get; }

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

        private string _baseHex;
        public string BaseHex
        {
            get => _baseHex;
            set
            {
                _baseHex = value;
                OnPropertyChanged();
                TryGeneratePaletteFromBase();
            }
        }

        public MainViewModel()
        {
            Colors = new ObservableCollection<ColorItem>();

            GenerateCommand = new Command(GenerateColors);
            IncreaseCommand = new Command(() => NumberColors++);
            DecreaseCommand = new Command(() => { if (NumberColors > 1) NumberColors--; });
            RandomColorCommand = new Command(() =>
            {
                BaseHex = $"#{new Random().Next(0x1000000):X6}";
            });
            

            // Inicializa con color aleatorio
            BaseHex = $"#{new Random().Next(0x1000000):X6}";
        }

        private void TryGeneratePaletteFromBase()
        {
            if (!string.IsNullOrWhiteSpace(BaseHex))
                GenerateColors();
        }

        private void GenerateColors()
        {
            if (string.IsNullOrWhiteSpace(BaseHex)) return;

            try
            {
                var baseRgb = HEXtoRGB(BaseHex);
                Colors.Clear();

                // Color base
                Colors.Add(new ColorItem { HexCode = BaseHex, Category = "Base" });

                // Complementario
                var compHex = GetComplementaryColor(BaseHex);
                Colors.Add(new ColorItem { HexCode = compHex, Category = "Complementario" });

                // Análogos
                float[] analogShifts = new float[] { -15, 15, -30, 30 };
                foreach (var shift in analogShifts)
                {
                    var analogHex = ShiftHue(baseRgb, shift);
                    Colors.Add(new ColorItem { HexCode = analogHex, Category = "Análogo" });
                }

                // Triádicos
                float[] triadicShifts = new float[] { -120, 120 };
                foreach (var shift in triadicShifts)
                {
                    var triadicHex = ShiftHue(baseRgb, shift);
                    Colors.Add(new ColorItem { HexCode = triadicHex, Category = "Triádico" });
                }

                OnPropertyChanged(nameof(Colors));
            }
            catch
            {
                // HEX inválido o error en conversión
                // Puedes mostrar alerta aquí si quieres
            }
        }

        private string GetComplementaryColor(string hex)
        {
            System.Drawing.Color color = HEXtoRGB(hex);
            int r = 255 - color.R;
            int g = 255 - color.G;
            int b = 255 - color.B;
            return $"#{r:X2}{g:X2}{b:X2}";
        }

        private System.Drawing.Color HEXtoRGB(string hex)
        {
            return System.Drawing.ColorTranslator.FromHtml(hex);
        }

        private string ShiftHue(System.Drawing.Color color, float shiftDegrees)
        {
            float hue = color.GetHue();
            float newHue = (hue + shiftDegrees + 360) % 360;

            float saturation = color.GetSaturation();
            float brightness = color.GetBrightness();

            System.Drawing.Color newColor = FromHSL(newHue, saturation, brightness);
            return $"#{newColor.R:X2}{newColor.G:X2}{newColor.B:X2}";
        }

        private System.Drawing.Color FromHSL(float h, float s, float l)
        {
            double c = (1 - Math.Abs(2 * l - 1)) * s;
            double x = c * (1 - Math.Abs((h / 60) % 2 - 1));
            double m = l - c / 2;

            double r = 0, g = 0, b = 0;

            if (h < 60) { r = c; g = x; }
            else if (h < 120) { r = x; g = c; }
            else if (h < 180) { g = c; b = x; }
            else if (h < 240) { g = x; b = c; }
            else if (h < 300) { r = x; b = c; }
            else { r = c; b = x; }

            return System.Drawing.Color.FromArgb(
                (int)((r + m) * 255),
                (int)((g + m) * 255),
                (int)((b + m) * 255)
            );
        }
        


        // INotifyPropertyChanged
        public event PropertyChangedEventHandler? PropertyChanged;
        protected void OnPropertyChanged([CallerMemberName] string? name = null) =>
            PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(name));
    }
}
