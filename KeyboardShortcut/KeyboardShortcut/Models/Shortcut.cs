using Microsoft.UI.Xaml.Media.Imaging;
using System.Text.Json.Serialization;

namespace KeyboardShortcut.Models;

/// <summary>
/// Representa um único atalho no arquivo menu.json.
/// </summary>
public class Shortcut
{
    [JsonPropertyName("Slot")]
    public int Slot { get; set; }

    [JsonPropertyName("Name")]
    public string Name { get; set; }

    [JsonPropertyName("Path")]
    public string Path { get; set; }

    [JsonIgnore]
    public BitmapImage Icon { get; set; }
}
