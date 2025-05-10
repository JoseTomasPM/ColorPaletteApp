using Microsoft.Maui.Controls.Shapes;

namespace ColorPaletteApp.Views;

public partial class MainPage : ContentPage
{
    public MainPage()
    {
        InitializeComponent();
        BindingContext = new ViewModels.MainViewModel();
    }

    public void OnBorderTapped(object sender, TappedEventArgs e)
    {
        // Convertimos el sender a un Border (asumimos que siempre será un Border)
        var border = (Border)sender;

        // Obtenemos el color de fondo del Border (BackgroundColor)
        var selectedColor = border.BackgroundColor;

        // Convertimos el color a formato hexadecimal
        string hexColor = selectedColor.ToHex();

        // Actualizamos el BaseHex en el ViewModel
        // Esto automáticamente generará la nueva paleta de colores
        ((ViewModels.MainViewModel)BindingContext).BaseHex = hexColor;

    }
    private async void OnLabelTapped(object sender, TappedEventArgs e)
    {
        // Convertimos el sender a un Label (asumimos que siempre será un Label)
        var label = (Label)sender;
        // Obtenemos el texto del Label (el código hexadecimal)
        string hexColor = label.Text;
        // Copiamos el color al portapapeles
        await Clipboard.SetTextAsync(hexColor);

        // Crear un Border para la notificación con esquinas redondeadas
        var notificationBorder = new Border
        {
            StrokeShape = new RoundRectangle
            {
                CornerRadius = new CornerRadius(10) // Esquinas redondeadas
            },
            BackgroundColor = Color.FromArgb("#888"),
            Padding = new Thickness(10),
            HorizontalOptions = LayoutOptions.Center,
            VerticalOptions = LayoutOptions.End,
            Opacity = 0, // Comienza invisible
            Content = new Label
            {
                Text = $"Color {hexColor} copied to clipboard!",
                TextColor = Color.FromArgb("#222"),
                FontAttributes = FontAttributes.Bold,
                HorizontalTextAlignment = TextAlignment.Center,
                VerticalTextAlignment = TextAlignment.Center
            }
        };

        // Agregar el Border al Grid principal
        if (this.Content is Grid grid)
        {
            grid.Children.Add(notificationBorder);
            Grid.SetRow(notificationBorder, 1); // Posiciona en la fila deseada
            Grid.SetColumnSpan(notificationBorder, grid.ColumnDefinitions.Count); // Ocupa todo el ancho

            // Animar la aparición de la notificación
            await notificationBorder.FadeTo(1, 250); // Aparece
            await Task.Delay(2000); // Espera 2 segundos
            await notificationBorder.FadeTo(0, 250); // Desaparece

            // Eliminar el Border del Grid
            grid.Children.Remove(notificationBorder);
        }
    }
}
    