using Microsoft.UI.Xaml.Media.Imaging;
using System;
using System.Diagnostics;
using System.Drawing;
using System.IO;
using System.Threading.Tasks;

namespace KeyboardShortcut.Services
{
    /// <summary>
    /// Serviço responsável por extrair ícones de arquivos executáveis.
    /// Substitui a necessidade de um IValueConverter no XAML.
    /// </summary>
    public static class IconLoader
    {
        /// <summary>
        /// Extrai o ícone de um caminho de arquivo e o converte para um BitmapImage.
        /// </summary>
        /// <param name="path">O caminho para o arquivo .exe ou outro que contenha um ícone.</param>
        /// <returns>Um BitmapImage do ícone ou null se não puder ser carregado.</returns>
        public static BitmapImage LoadIconFromPath(string path)
        {
            if (string.IsNullOrEmpty(path))
            {
                return null;
            }

            try
            {
                var expandedPath = Environment.ExpandEnvironmentVariables(path);

                using var icon = Icon.ExtractAssociatedIcon(expandedPath);
                if (icon != null)
                {
                    // Converte o System.Drawing.Icon para um Microsoft.UI.Xaml.Media.Imaging.BitmapImage
                    using var ms = new MemoryStream();
                    icon.ToBitmap().Save(ms, System.Drawing.Imaging.ImageFormat.Png);
                    ms.Position = 0;

                    var bitmapImage = new BitmapImage();

                    // Importante: O SetSource precisa ser feito na thread da UI.
                    // Como estamos chamando isso do construtor da MainWindow, que está na thread da UI,
                    // esta chamada síncrona é segura.
                    bitmapImage.SetSource(ms.AsRandomAccessStream());

                    return bitmapImage;
                }
            }
            catch (Exception ex)
            {
                Debug.WriteLine($"Could not load icon from path {path}: {ex.Message}");
            }

            return null;
        }
    }
}
