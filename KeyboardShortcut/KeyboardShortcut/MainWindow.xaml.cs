using KeyboardShortcut.Models;
using KeyboardShortcut.Services;
using Microsoft.UI;
using Microsoft.UI.Windowing;
using Microsoft.UI.Xaml;
using Microsoft.UI.Xaml.Controls;
using Microsoft.UI.Xaml.Input;
using System;
using System.Collections.ObjectModel;
using System.Diagnostics;
using System.Linq;
using System.Runtime.InteropServices;
using System.Threading.Tasks;
using Windows.System;
using WinUIEx;

namespace KeyboardShortcut
{
    /// <summary>
    /// Janela principal da aplicação que exibe a barra de atalhos.
    /// </summary>
    public sealed partial class MainWindow : WinUIEx.WindowEx
    {
        public ObservableCollection<Shortcut> Shortcuts { get; } = new ObservableCollection<Shortcut>();

        public MainWindow()
        {
            this.InitializeComponent();

            this.Activated += Window_Activated;
            (this.Content as UIElement).KeyDown += MainWindow_KeyDown;

            SetupWindow();
            _ = LoadShortcutsAsync();
        }

        public void ShowAndActivate()
        {
            if (!this.AppWindow.IsVisible)
                this.Show();

            this.Activate();
            WindowExtensions.SetForegroundWindow(this);
            this.Content.Focus(FocusState.Programmatic);
        }

        /// <summary>
        /// Carrega os atalhos do arquivo menu.json e popula a cole��o.
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
        /// Manipulador de evento para o clique em um bot�o de atalho.
        /// </summary>
        private void ShortcutButton_Click(object sender, RoutedEventArgs e)
        {
            if ((sender as Button)?.Tag is Shortcut shortcut)
            {
                LaunchShortcut(shortcut);
            }
        }


        /// <summary>
        /// Manipulador de evento para o pressionamento de teclas na janela.
        /// </summary>
        private void MainWindow_KeyDown(object sender, KeyRoutedEventArgs e)
        {
            int slot = -1;
            switch (e.Key)
            {
                case VirtualKey.Number1: case VirtualKey.NumberPad1: slot = 1; break;
                case VirtualKey.Number2: case VirtualKey.NumberPad2: slot = 2; break;
                case VirtualKey.Number3: case VirtualKey.NumberPad3: slot = 3; break;
                case VirtualKey.Number4: case VirtualKey.NumberPad4: slot = 4; break;
                case VirtualKey.Number5: case VirtualKey.NumberPad5: slot = 5; break;
                case VirtualKey.Number6: case VirtualKey.NumberPad6: slot = 6; break;
                case VirtualKey.Number7: case VirtualKey.NumberPad7: slot = 7; break;
                case VirtualKey.Number8: case VirtualKey.NumberPad8: slot = 8; break;
                case VirtualKey.Number9: case VirtualKey.NumberPad9: slot = 9; break;
            }

            if (slot != -1)
            {
                var shortcut = Shortcuts.FirstOrDefault(s => s.Slot == slot);
                if (shortcut != null)
                {
                    LaunchShortcut(shortcut);
                }
            }
        }


        /// <summary>
        /// Inicia o processo de um atalho e fecha a janela.
        /// </summary>
        /// <param name="shortcut">O atalho a ser executado.</param>
        private void LaunchShortcut(Shortcut shortcut)
        {
            if (shortcut == null) return;

            try
            {
                Process.Start(new ProcessStartInfo(shortcut.Path) { UseShellExecute = true });
            }
            catch (Exception ex)
            {
                Debug.WriteLine($"Failed to start process for {shortcut.Name}: {ex.Message}");
            }
            finally
            {
                this.Hide();
            }
        }


        /// <summary>
        /// Configura a apar�ncia e o posicionamento da janela principal.
        /// </summary>
        private void SetupWindow()
        {
            this.AppWindow.SetIcon("Assets/store.ico");

            // Aplica bordas arredondadas
            var windowHandle = Win32Interop.GetWindowFromWindowId(AppWindow.Id);
            int cornerPreference = 2;
            NativeApi.DwmSetWindowAttribute(windowHandle, 33, ref cornerPreference, Marshal.SizeOf<int>());
            const int windowWidth = 500;
            const int windowHeight = 110;

            this.SetIsMaximizable(false);
            this.SetIsMinimizable(false);
            this.SetIsResizable(false);
            this.SetIsAlwaysOnTop(true);
            this.SetIsShownInSwitchers(false); // Essencial para não aparecer no Alt+Tab
                                               // this.SetWindowStyle(WindowStyle.None); // Remove todas as bordas e barra de título WindowStyle.None não existe

            this.SetWindowStyle(WindowStyle.ThickFrame);
            var screenWidth = DisplayArea.Primary.WorkArea.Width;
            var screenHeight = DisplayArea.Primary.WorkArea.Height;
            var newX = (screenWidth - windowWidth) / 2;
            var newY = screenHeight - windowHeight - 40;
            this.MoveAndResize(newX, newY, windowWidth, windowHeight);
            this.SetTaskBarIcon(null);

            this.SystemBackdrop = new WinUIEx.TransparentTintBackdrop()
            {
                TintColor = Windows.UI.Color.FromArgb(127, 0, 0, 0)
            };
        }
        /// <summary>
        /// Chamado quando o estado de ativa��o da janela muda.

        /// </summary>
        private void Window_Activated(object sender, WindowActivatedEventArgs args)
        {
            if (args.WindowActivationState == WindowActivationState.Deactivated)
            {
                this.Hide();
            }
        }
    }
    public class NativeApi
    {
        [DllImport("dwmapi.dll")]
        public static extern int DwmSetWindowAttribute(IntPtr hwnd, int attribute, ref int value, int size);
    }
}
