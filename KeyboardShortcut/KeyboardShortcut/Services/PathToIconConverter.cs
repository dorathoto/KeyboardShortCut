using Microsoft.UI.Xaml.Data;
using Microsoft.UI.Xaml.Media.Imaging;
using System;
using System.Diagnostics;
using System.Drawing;
using System.IO;

namespace KeyboardShortcut
{
    public class PathToIconConverter : IValueConverter
    {
        public object Convert(object value, Type targetType, object parameter, string language)
        {
            if (value is not string path || string.IsNullOrEmpty(path))
            {
                return null;
            }

            try
            {
                // Tenta expandir variáveis de ambiente, como %windir%
                var expandedPath = Environment.ExpandEnvironmentVariables(path);

                using var icon = Icon.ExtractAssociatedIcon(expandedPath);

                if (icon != null)
                {
                    // Converte o ícone para um formato que o WinUI possa exibir.
                    using var ms = new MemoryStream();
                    icon.ToBitmap().Save(ms, System.Drawing.Imaging.ImageFormat.Png);
                    ms.Position = 0;

                    var bitmapImage = new BitmapImage();
                    bitmapImage.SetSource(ms.AsRandomAccessStream());
                    return bitmapImage;
                }
            }
            catch (Exception ex)
            {
                // Se ocorrer um erro (ex: arquivo não encontrado), registra no console.
                Debug.WriteLine($"Could not load icon from path {path}: {ex.Message}");
            }

            // Retorna nulo se o ícone não puder ser carregado.
            // A UI pode ter um ícone padrão como fallback.
            return null;
        }

        public object ConvertBack(object value, Type targetType, object parameter, string language)
        {
            // A conversão de volta não é necessária para este cenário.
            throw new NotImplementedException();
        }
    }
}