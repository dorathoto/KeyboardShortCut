using H.NotifyIcon;
using H.NotifyIcon.Core;
using Microsoft.UI.Xaml;
using Microsoft.UI.Xaml.Controls;
using System;
using System.Threading;
using System.Threading.Tasks;
using WinUIEx;

namespace KeyboardShortcut
{
    public partial class App : Application
    {
        private static EventWaitHandle? _eventWaitHandle;
        private static readonly Mutex _mutex = new Mutex(true, "{1F61A9A2-E55A-4E2F-B5D4-2D49F8C838A6}");
        private MainWindow? _mainWindow;
        private TaskbarIcon? _trayIcon;

        public App()
        {
            InitializeComponent();
        }

        protected override void OnLaunched(LaunchActivatedEventArgs args)
        {


            // Verifica se outra instância já está rodando.
            if (!_mutex.WaitOne(TimeSpan.Zero, true))
            {
                try
                {
                    _eventWaitHandle = EventWaitHandle.OpenExisting("{2F61A9A2-E55A-4E2F-B5D4-2D49F8C838A7}");
                    _eventWaitHandle?.Set();
                }
                finally
                {
                    Exit();
                }
                return;
            }

            _eventWaitHandle = new EventWaitHandle(false, EventResetMode.AutoReset, "{2F61A9A2-E55A-4E2F-B5D4-2D49F8C838A7}");

            _mainWindow = new MainWindow();
            _mainWindow.DispatcherQueue.ShutdownStarting += (s, e) =>
            {
                _mutex.ReleaseMutex();
                _eventWaitHandle?.Close();
                _trayIcon?.Dispose();
            };

            _trayIcon = (TaskbarIcon)Resources["TrayIcon"];
            _trayIcon.LeftClickCommand = new RelayCommand(() => _mainWindow.ShowAndActivate());

            if (_trayIcon.ContextFlyout is MenuFlyout contextMenu)
            {
                var showMenuItem = (MenuFlyoutItem)contextMenu.Items[0];
                showMenuItem.Click += (s, e) => _mainWindow.ShowAndActivate();

                var exitMenuItem = (MenuFlyoutItem)contextMenu.Items[2]; // Índice 2 por causa do Separator
                exitMenuItem.Click += (s, e) => Exit();
            }
            StartListener();


            _mainWindow.Activate();
        }

        private void StartListener()
        {
            Task.Run(() =>
            {
                // Fica em um loop, esperando por sinais.
                while (_eventWaitHandle!.WaitOne())
                {
                    // Quando um sinal é recebido, volta para a thread da UI para mostrar a janela.
                    _mainWindow?.DispatcherQueue.TryEnqueue(() =>
                    {
                        _mainWindow.ShowAndActivate();
                    });
                }
            });
        }
    }
    public class RelayCommand : System.Windows.Input.ICommand
    {
        private readonly Action _execute;
        public RelayCommand(Action execute) => _execute = execute;
        public event EventHandler? CanExecuteChanged;
        public bool CanExecute(object? parameter) => true;
        public void Execute(object? parameter) => _execute();
    }

}
