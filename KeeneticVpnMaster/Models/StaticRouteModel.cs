using System.Text.Json.Serialization;

namespace KeeneticVpnMaster.Models;

/// <summary>
/// Представляет модель статического маршрута для конфигурации сети.
/// </summary>
public class StaticRouteModel
{
    /// <summary>
    /// Определяет, исключать маршрут или нет.
    /// </summary>
    [JsonPropertyName("no")]
    public bool No { get; set; } = false;
    
    /// <summary>
    /// Сетевой адрес маршрута.
    /// </summary>
    [JsonPropertyName("network")]
    public string Network { get; set; } = string.Empty;
    
    /// <summary>
    /// Маска подсети маршрута.
    /// </summary>
    [JsonPropertyName("mask")]
    public string Mask { get; set; } = string.Empty;
    
    /// <summary>
    /// Интерфейс, через который проходит маршрут.
    /// </summary>
    [JsonPropertyName("interface")]
    public string Interface { get; set; } = string.Empty;

    /// <summary>
    /// Определяет, включён ли маршрут автоматически.
    /// </summary>
    [JsonPropertyName("auto")]
    public bool Auto { get; set; } = true;
    
    /// <summary>
    /// Комментарий к маршруту(категория).
    /// </summary>
    [JsonPropertyName("comment")]
    public string Comment { get; set; } = "Keenetic Vpn Master";
    
}