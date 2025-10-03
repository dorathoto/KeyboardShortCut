using KeyboardShortcut.Models;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Text.Json;
using System.Threading.Tasks;

namespace KeyboardShortcut.Services
{
    /// <summary>
    /// Gerencia o carregamento e a criação do arquivo de configuração menu.json.
    /// </summary>
    public class SettingsService
    {
        private readonly string _settingsFilePath;

        public SettingsService()
        {
            // Define o caminho do arquivo para estar na mesma pasta do executável.
            _settingsFilePath = Path.Combine(AppContext.BaseDirectory, "menu.json");
        }

        /// <summary>
        /// Carrega a lista de atalhos do menu.json. Se o arquivo não existir, cria um de exemplo.
        /// </summary>
        public async Task<List<Shortcut>> LoadShortcutsAsync()
        {
            if (!File.Exists(_settingsFilePath))
            {
                await CreateDefaultSettingsFileAsync();
            }

            var json = await File.ReadAllTextAsync(_settingsFilePath);
            var shortcuts = JsonSerializer.Deserialize<List<Shortcut>>(json);
            return shortcuts ?? new List<Shortcut>();
        }

        /// <summary>
        /// Cria um arquivo menu.json padrão com exemplos.
        /// </summary>
        private async Task CreateDefaultSettingsFileAsync()
        {
            var defaultShortcuts = new List<Shortcut>
            {
                new Shortcut
                {
                    Slot = 1,
                    Name = "Notepad",
                    Path = "C:\\Windows\\System32\\notepad.exe"
                },
                new Shortcut
                {
                    Slot = 2,
                    Name = "Calculator",
                    Path = "calc.exe"
                }
            };

            var options = new JsonSerializerOptions { WriteIndented = true };
            var json = JsonSerializer.Serialize(defaultShortcuts, options);

            await File.WriteAllTextAsync(_settingsFilePath, json);
        }
    }
}