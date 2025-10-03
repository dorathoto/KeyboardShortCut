using KeyboardShortcut.Models;
using KeyboardShortcut.Services;
using Microsoft.UI;
using Microsoft.UI.Windowing;
using Microsoft.UI.Xaml;
using Microsoft.UI.Xaml.Controls;
using System;
using System.Collections.ObjectModel;
using System.Diagnostics;
using System.Linq;
using System.Threading.Tasks;
using WinRT.Interop;

namespace KeyboardShortcut
{
    /// <summary>
    /// Janela principal da aplicação que exibe a barra de atalhos.
    /// </summary>
    public sealed partial class MainWindow : Window
    {
        private AppWindow _appWindow;

        public ObservableCollection<Shortcut> Shortcuts { get; } = new ObservableCollection<Shortcut>();

        public MainWindow()
        {
            this.InitializeComponent();
            this.Activated += Window_Activated;

            SetupWindow();

            _ = LoadShortcutsAsync();

        }

        /// <summary>
        /// Carrega os atalhos do arquivo menu.json e popula a coleção.
        /// </summary>
        private async Task LoadShortcutsAsync()
        {
            var settingsService = new SettingsService();
            var shortcutsList = await settingsService.LoadShortcutsAsync();

            Shortcuts.Clear();
            foreach (var shortcut in shortcutsList.OrderBy(s => s.Slot))
            {
                shortcut.Icon = IconLoader.LoadIconFromPath(shortcut.Path);
                Shortcuts.Add(shortcut);
            }
        }
        /// <summary>
        /// Manipulador de evento para o clique em um botão de atalho.
        /// Inicia o processo correspondente e fecha a barra.
        /// </summary>
        private void ShortcutButton_Click(object sender, RoutedEventArgs e)
        {
            if ((sender as Button)?.Tag is Shortcut shortcut)
            {
                try
                {
                    // Usa Process.Start para executar o arquivo definido no Path.
                    Process.Start(new ProcessStartInfo(shortcut.Path) { UseShellExecute = true });
                }
                catch (Exception ex)
                {
                    // Log de erro caso o processo não possa ser iniciado.
                    Debug.WriteLine($"Failed to start process for {shortcut.Name}: {ex.Message}");
                    // Poderíamos exibir um diálogo de erro aqui no futuro.
                }
                finally
                {
                    // Fecha a janela após a tentativa de execução.
                    this.Close();
                }
            }
        }
        /// <summary>
        /// Configura a aparência e o posicionamento da janela principal.
        /// </summary>
        private void SetupWindow()
        {
            // --- 1. Obter o AppWindow ---
            var hWnd = WindowNative.GetWindowHandle(this);
            var windowId = Win32Interop.GetWindowIdFromWindow(hWnd);
            _appWindow = AppWindow.GetFromWindowId(windowId);

            // --- 2. Remover bordas e barra de título ---
            if (_appWindow.Presenter is OverlappedPresenter presenter)
            {
                presenter.SetBorderAndTitleBar(false, false);
                presenter.IsResizable = false;
                presenter.IsMaximizable = false;
                presenter.IsMinimizable = false;
            }

            // --- 3. Definir o tamanho da janela ---
            const int windowWidth = 500; // Aumentado para acomodar mais ícones
            const int windowHeight = 110;

            // --- 4. Posicionar a janela na parte inferior central ---
            var displayArea = DisplayArea.GetFromWindowId(windowId, DisplayAreaFallback.Primary);
            var screenWidth = displayArea.WorkArea.Width;
            var screenHeight = displayArea.WorkArea.Height;

            var newX = (screenWidth - windowWidth) / 2;
            var newY = screenHeight - windowHeight - 40; // 40 pixels de margem da parte inferior

            _appWindow.MoveAndResize(new Windows.Graphics.RectInt32(newX, newY, windowWidth, windowHeight));

            // --- 5. Aplicar fundo translúcido (Acrílico) ---
            SystemBackdrop = new Microsoft.UI.Xaml.Media.DesktopAcrylicBackdrop();
        }
        /// <summary>
        /// Chamado quando o estado de ativação da janela muda.
        /// Fecha a janela quando ela perde o foco.
        /// </summary>
        private void Window_Activated(object sender, WindowActivatedEventArgs args)
        {
            if (args.WindowActivationState == WindowActivationState.Deactivated)
            {
                // A janela perdeu o foco, então a fechamos.
                this.Close();
            }
        }
    }
}
